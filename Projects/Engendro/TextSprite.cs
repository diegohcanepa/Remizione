using Engendro.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Engendro
{
    /// <summary>
    /// TextSprite
    /// </summary>
    public class TextSprite : Sprite
    {
        #region Private fields

        private const string asianPunctuationSymbols = "。，？！”；：、）—》";
        private List<char>? characters;
        private bool customSpacing;
        private string displayTextCopy = string.Empty;
        private const string ellipsesValue = "...";
        private bool hasTypingSoundControl;
        private string lastWord = string.Empty;
        private int previousLineSpacing;
        private float previousSpacing;
        private const string space = " ";
        private static readonly StringBuilder stringBuilder = new();
        private string? text;
        private int textRepositoryLoadCount;
        private Vector2 textSize;
        private Timer? textTimer;
        private SoundInstance? typingSound;
        private float typingSoundVolume;

        #endregion

        #region Constructor

        // Constructor
        public TextSprite(EngendroGame game, Font? font)
            : base(game)
        {
            this.Font = font;
        }

        #endregion

        #region Private members

        // ContainsAsianSymbols
        private static bool ContainsAsianSymbols(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            for (var i = 0; i < text.Length; i++)
            {
                if (char.GetUnicodeCategory(text[i]) == UnicodeCategory.OtherLetter)
                {
                    return true;
                }
            }

            return false;
        }

        // DrawCore
        private void DrawCore(SpriteFont font)
        {
            var pos = Position;
            if (VisualParent != null)
                pos = GetAbsolutePosition();

            Game.SpriteBatch.DrawString(font, DisplayText, pos, Color * Opacity * OpacityFactor, Rotation, Pivot.Position, Scale, SpriteEffects.None, 0);
        }

        // Invalidate
        private void Invalidate()
        {
            if (text == null)
            {
                LineCount = 0;
                textSize = Vector2.Zero;
                DisplayText = null;
                return;
            }

            if (MaximumWidth == 0)
            {
                DisplayText = text;
                LineCount = 1;
            }
            else
            {
                DisplayText = WrapText(text);
            }

            if (DisplayText != null && HideEndingPeriod && DisplayText.EndsWith('.'))
            {
                DisplayText = DisplayText.Substring(0, DisplayText.Length - 1);
            }

            textSize = DisplayText == null || Font?.SpriteFont == null ? Vector2.Zero : Font.SpriteFont.MeasureString(DisplayText);

            IsBoundingBoxDirty = true;
        }

        // InvalidateLocalizableText
        private void InvalidateLocalizableText()
        {
            if (TextRepositoryKey == null || textRepositoryLoadCount == TextRepository.LoadCount)
            {
                return;
            }

            textRepositoryLoadCount = TextRepository.LoadCount;

            if (TextRepositoryKey == null)
            {
                text = null;
            }
            else
            {
                text = TextRepository.GetValue(TextRepositoryKey);
            }

            Invalidate();

            LocalizedTextChanged?.Invoke(this, EventArgs.Empty);
        }

        // MeasureScaledString
        private Vector2 MeasureScaledString(string text)
        {
            return Font?.SpriteFont == null ? Vector2.Zero : Font.SpriteFont.MeasureString(text) * Scale;
        }

        // TryTypeText
        private bool TryTypeText(out string text, out int duration)
        {
            text = string.Empty;
            duration = 0;

            // No remaining characters
            if (characters == null || characters.Count == 0)
            {
                return false;
            }

            var isFirstCharacter = displayTextCopy.Length - characters.Count == 0;
            text = characters[0].ToString(CultureInfo.InvariantCulture);
            characters.RemoveAt(0);

            // No more characters
            if (characters.Count == 0)
            {
                duration = TypingSpeed;
                return true;
            }

            if (text != space)
            {
                lastWord += text;
            }
            else
            {
                lastWord = string.Empty;
            }

            // Colon
            if (text == ":" && !isFirstCharacter)
            {
                duration = PauseOnPunctuationMarks ? TextTypingSettings.ColonPauseDuration : 0;
            }

            // Semicolon
            else if (text == ";" && !isFirstCharacter)
            {
                duration = PauseOnPunctuationMarks ? TextTypingSettings.SemicolonPauseDuration : 0;
            }

            // Dot
            else if (text == ".")
            {
                if (Abbreviations.Contains(lastWord))
                {
                    lastWord = string.Empty;
                }
                else
                {
                    // Remove repeating characters
                    var dotCount = 0;
                    while (characters.Count > 0)
                    {
                        if (!char.IsLetter(characters[0]) && characters[0] != '¿' && characters[0] != '¡')
                        {
                            if (characters[0] == '.')
                            {
                                dotCount++;
                            }

                            text += characters[0].ToString(CultureInfo.InvariantCulture);
                            characters.RemoveAt(0);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (characters.Count > 0 && !isFirstCharacter)
                    {
                        if (dotCount >= 2)
                        {
                            duration = PauseOnPunctuationMarks ? TextTypingSettings.EllipsisPauseDuration : 0;
                        }
                        else
                        {
                            duration = PauseOnPunctuationMarks ? TextTypingSettings.DotPauseDuration : 0;
                        }
                    }
                    else
                    {
                        duration = 0;
                    }
                }
            }

            else
            {
                duration = TypingSpeed;
            }

            if (characters.Count == 0)
            {
                lastWord = string.Empty;
            }

            return true;
        }

        // WrapAsianLine
        private string[] WrapAsianLine(string text)
        {
            List<string> result = [];

            Vector2 size;
            var line = string.Empty;
            float lineWidth = 0;
            for (var i = 0; i < text.Length; i++)
            {
                var symbol = text[i].ToString();
                size = MeasureScaledString(symbol);
                line += symbol;
                lineWidth += size.X;

                if (lineWidth >= MaximumWidth)
                {
                    // Ensure punctuation symbols
                    if (i + 1 < text.Length)
                    {
                        for (var j = i + 1; j < text.Length; j++)
                        {
                            if (asianPunctuationSymbols.Contains(text[j].ToString(), StringComparison.InvariantCulture))
                            {
                                line += text[j].ToString();
                                i++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (!Multiline)
                    {
                        line += ellipsesValue;
                    }

                    result.Add(line);

                    if (Multiline)
                    {
                        lineWidth = 0;
                        line = string.Empty;
                    }
                    else
                    {
                        return [.. result];
                    }
                }
            }

            if (!string.IsNullOrEmpty(line))
            {
                result.Add(line);
            }

            return [.. result];
        }

        // WrapAsianLines
        private string[] WrapAsianLines(string[] lines)
        {
            List<string> result = [];

            for (var i = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    result.Add(string.Empty);
                }
                else if (MeasureScaledString(lines[i]).X > MaximumWidth)
                {
                    result.AddRange(WrapAsianLine(lines[i]));
                }
                else
                {
                    result.Add(lines[i]);
                }
            }

            return [.. result];
        }

        // WrapText
        private string WrapText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                LineCount = 0;
                return text;
            }

            var lines = text.Split([Environment.NewLine], StringSplitOptions.None);

            stringBuilder.Clear();

            LineCount = 1;

            var ellipses = false;
            var spaceWidth = MeasureScaledString(space).X;

            // Asian text
            if (Multiline && ContainsAsianSymbols(text))
            {
                var wrappedLines = WrapAsianLines(lines);
                LineCount = wrappedLines.Length;

                for (var i = 0; i < wrappedLines.Length; i++)
                {
                    if (string.IsNullOrEmpty(wrappedLines[i]))
                    {
                        stringBuilder.Append(Environment.NewLine);
                    }
                    else
                    {
                        stringBuilder.Append(wrappedLines[i]);
                        if (i < wrappedLines.Length - 1)
                        {
                            stringBuilder.Append(Environment.NewLine);
                        }
                    }
                }

                return stringBuilder.ToString();
            }

            for (var j = 0; j < lines.Length; j++)
            {
                float lineWidth = 0;
                var words = lines[j].Split(' ');

                for (var i = 0; i < words.Length; i++)
                {
                    var word = words[i];
                    var size = MeasureScaledString(word);

                    //size.X += (font.Spacing * -1) * Scale.X * word.Length;

                    var addSpace = !(i == words.Length - 1);

                    if (lineWidth + size.X <= MaximumWidth)
                    {
                        stringBuilder.Append(word + (addSpace ? space : string.Empty));
                        lineWidth += size.X + spaceWidth;
                    }
                    else
                    {
                        if (Multiline)
                        {
                            stringBuilder.Append(Environment.NewLine + word + (addSpace ? space : string.Empty));
                            lineWidth = size.X + spaceWidth;
                            LineCount++;
                        }
                        else
                        {
                            stringBuilder.Append(ellipsesValue);
                            ellipses = true;
                            break;
                        }
                    }
                }

                if (!ellipses && j < lines.Length - 1)
                {
                    stringBuilder.AppendLine();
                }

                if (ellipses)
                {
                    break;
                }
            }

            if (Multiline && LineCount < lines.Length)
            {
                LineCount = lines.Length;
            }

            return stringBuilder.ToString();
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (IsEmpty || Font?.SpriteFont == null)
                return;

            InvalidateLocalizableText();

            if (Text == null)
                return;

            if (customSpacing)
            {
                previousLineSpacing = Font.SpriteFont.LineSpacing;
                previousSpacing = Font.SpriteFont.Spacing;

                if (LineSpacing != int.MinValue)
                    Font.SpriteFont.LineSpacing = LineSpacing;

                if (Spacing != float.MinValue)
                    Font.SpriteFont.Spacing = Spacing;
            }

            if (ShadowOffset != Vector2.Zero)
            {
                var currentColor = Color;
                var currentPosition = Position;
                Color = ShadowColor;
                Position += ShadowOffset;
                DrawCore(Font.SpriteFont);
                Color = currentColor;
                Position = currentPosition;
            }

            DrawCore(Font.SpriteFont);

            if (customSpacing)
            {
                Font.SpriteFont.LineSpacing = previousLineSpacing;
                Font.SpriteFont.Spacing = previousSpacing;
            }
        }

        // OnTransform
        protected override void OnTransform(TransformChange transformChange)
        {
            if (transformChange == TransformChange.Scale)
                Invalidate();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (TypingState == RunningState.Stopped)
            {
                return;
            }

            if (textTimer != null)
            {
                textTimer.Update(gameTime);

                if (!textTimer.IsRunning)
                {
                    if (TryTypeText(out var text, out var duration))
                    {
                        DisplayText += text;

                        if (duration == this.TypingSpeed)
                        {
                            if (typingSound != null && typingSound.State == SoundState.Stopped && hasTypingSoundControl)
                            {
                                typingSound.Volume.Current = typingSoundVolume;
                                typingSound.Play();
                                hasTypingSoundControl = false;
                            }
                            TypingState = RunningState.Running;
                        }
                        else
                        {
                            TypingState = RunningState.Paused;
                            typingSound?.Stop();
                            hasTypingSoundControl = true;
                        }

                        textTimer.Start(duration);
                    }
                    else
                    {
                        StopTyping();
                    }
                }
            }
        }

        #endregion

        // Abbreviations
        public static HashSet<string> Abbreviations { get; } = [];

        // Clear
        public void Clear()
        {
            Text = string.Empty;
        }

        // DisplayText
        public string? DisplayText { get; private set; }

        // Font
        public Font? Font
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    Invalidate();
                }
            }
        }

        // Height
        public override int Height => (int)Math.Round(textSize.Y, 0);

        // HideEndingPeriod
        public bool HideEndingPeriod { get; set; }

        // IsEmpty
        public override bool IsEmpty => Font?.SpriteFont == null || string.IsNullOrEmpty(text);

        // IsTyping
        public bool IsTyping => TypingState != RunningState.Stopped;

        // Length
        public int Length => Text == null ? 0 : Text.Length;

        // LineCount
        public int LineCount { get; private set; }

        // LineSpacing
        public int LineSpacing
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    customSpacing = field != int.MinValue || Spacing != float.MinValue;
                }
            }
        } = int.MinValue;

        // LocalizedTextChanged
        public event EventHandler? LocalizedTextChanged;

        // MaximumWidth
        public int MaximumWidth
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Invalidate();
                }
            }
        }

        // MeasureDisplayText
        public Vector2 MeasureDisplayText()
        {
            return DisplayText == null ? Vector2.Zero : MeasureScaledString(DisplayText);
        }

        // Multiline
        public bool Multiline
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Invalidate();
                }
            }
        } = true;

        // PauseDuration
        public int PauseDuration { get; set; } = 400;

        // PauseOnPunctuationMarks
        public bool PauseOnPunctuationMarks { get; set; }

        // ShadowColor
        public Color ShadowColor { get; set; } = Color.Black;

        // ShadowOffset
        public Vector2 ShadowOffset { get; set; }

        // Spacing
        public float Spacing
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    customSpacing = LineSpacing != int.MinValue || field != float.MinValue;
                }
            }
        } = float.MinValue;

        // StartTyping
        public void StartTyping()
        {
            StartTyping(null);
        }

        // StartTyping
        public void StartTyping(SoundInstance? sound)
        {
            if (IsEmpty || string.IsNullOrWhiteSpace(DisplayText) || TypingSpeed == 0)
            {
                return;
            }

            if (sound != null && sound.IsDisposed)
            {
                sound = null;
            }

            lastWord = string.Empty;

            this.typingSound = sound;
            //typingSound?.Volume.Reset();

            if (characters == null)
                characters = [];

            characters.Clear();
            characters.AddRange(DisplayText.ToCharArray());
            displayTextCopy = DisplayText;
            DisplayText = string.Empty;

            if (textTimer == null)
            {
                textTimer = new Timer();
            }

            textTimer.Start(TypingSpeed);
            if (typingSound != null)
            {
                typingSoundVolume = typingSound.Volume.Current;
                typingSound.IsLooped = true;
                typingSound.Play();
            }

            TypingState = RunningState.Running;
        }

        // StopTyping
        public void StopTyping()
        {
            if (TypingState != RunningState.Stopped)
            {
                characters?.Clear();
                DisplayText = displayTextCopy;
                textTimer?.Stop();
                typingSound?.Stop();
                TypingState = RunningState.Stopped;
            }
        }

        // Tag
        public object? Tag { get; set; }

        // Text
        public string? Text
        {
            get => text;
            set
            {
                if (TypingState != RunningState.Stopped)
                    StopTyping();

                if (value != text)
                {
                    if (value != null && TextRepository.IsKeyReference(value))
                    {
                        TextRepositoryKey = value;
                        textRepositoryLoadCount = TextRepository.LoadCount;
                    }
                    else
                    {
                        TextRepositoryKey = null;
                        textRepositoryLoadCount = 0;
                    }

                    if (value == null)
                    {
                        text = null;
                    }
                    else
                    {
                        text = TextRepositoryKey != null ? TextRepository.GetValue(value.Substring(1)) : value;
                    }

                    Invalidate();
                }
            }
        }

        // TextRepositoryKey
        public string? TextRepositoryKey { get; private set; }

        // ToString
        public override string ToString()
        {
            return text ?? string.Empty;
        }

        // TypingSpeed
        public int TypingSpeed { get; set; } = TextTypingSettings.TypingSpeed;

        // TypingState
        public RunningState TypingState { get; private set; }

        // Width
        public override int Width => (int)Math.Round(textSize.X, 0);
    }
}
