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
        private string formattedText = string.Empty;
        private bool customSpacing;
        private const string ellipsesValue = "...";
        private bool hasTypingSoundControl;
        private string lastWord = string.Empty;
        private int previousLineSpacing;
        private float previousSpacing;
        private readonly StringBuilder _renderBuffer = new();
        private const string space = " ";
        private static readonly StringBuilder stringBuilder = new();
        private string? text;
        private int textRepositoryLoadCount;
        private Vector2 textSize;
        private Timer? textTimer;
        private int _typingIndex;
        private SoundInstance? typingSound;
        private float typingSoundVolume;

        #endregion

        #region Constructor

        // Constructor
        public TextSprite(Font? font)
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
            Game.SpriteBatch.DrawString(font, _renderBuffer, pos, Color * Opacity * OpacityFactor, Rotation, Pivot.Position, Scale, SpriteEffects.None, 0);
        }

        // Invalidate
        private void Invalidate()
        {
            // Bloque de seguridad: Si el texto es nulo, reseteamos todo.
            if (text == null)
            {
                LineCount = 0;
                textSize = Vector2.Zero;
                formattedText = string.Empty;
                _renderBuffer.Clear();
                return;
            }

            // 1. Calculamos el texto final con el wrapping aplicado
            string? processedText;
            if (MaximumWidth == 0)
            {
                processedText = text;
                LineCount = 1;
            }
            else
            {
                processedText = WrapText(text);
            }

            // Ajuste del punto final si es necesario
            if (processedText != null && HideEndingPeriod && processedText.EndsWith('.'))
            {
                processedText = processedText.Substring(0, processedText.Length - 1);
            }

            // FIX: Null Coalescing para asegurar que _formattedText nunca sea null
            formattedText = processedText ?? string.Empty;

            // 2. Medimos el tamaño total basado en el texto completo (para evitar saltos del Pivot)
            if (Font?.SpriteFont != null)
            {
                textSize = Font.SpriteFont.MeasureString(formattedText);
            }
            else
            {
                textSize = Vector2.Zero;
            }

            // 3. Si NO estamos escribiendo, mostramos todo inmediatamente
            if (TypingState == RunningState.Stopped)
            {
                _renderBuffer.Clear();
                _renderBuffer.Append(formattedText);
                _typingIndex = formattedText.Length;
            }
            // Si estamos escribiendo, el Update se encargará de llenar el buffer

            IsBoundingBoxDirty = true;
        }

        // InvalidateLocalizableText
        private void InvalidateLocalizableText()
        {
            if (TextRepositoryKey == null || textRepositoryLoadCount == TextRepository.LoadCount)
                return;

            textRepositoryLoadCount = TextRepository.LoadCount;

            if (TextRepositoryKey == null)
                text = null;
            else
                text = TextRepository.GetValue(TextRepositoryKey);

            Invalidate();

            LocalizedTextChanged?.Invoke(this, EventArgs.Empty);
        }

        // MeasureScaledString
        private Vector2 MeasureScaledString(string text)
        {
            return Font?.SpriteFont == null ? Vector2.Zero : Font.SpriteFont.MeasureString(text) * Scale;
        }

        // TryTypeText - OPTIMIZADO y SEGURO
        private bool TryTypeText(out int duration)
        {
            duration = 0;

            // GUARDIA: Verificamos nulidad (aunque Invalidate lo previene) y límites
            if (string.IsNullOrEmpty(formattedText) || _typingIndex >= formattedText.Length)
                return false;

            // Obtenemos caracter actual
            char currentChar = formattedText[_typingIndex];

            // Avanzamos índice y añadimos al buffer visual
            _typingIndex++;
            _renderBuffer.Append(currentChar);

            // --- Lógica de Pausas y Palabras ---

            bool isFirstCharacter = _typingIndex == 1;
            string charStr = currentChar.ToString();

            if (charStr != space)
            {
                lastWord += charStr;
            }
            else
            {
                lastWord = string.Empty;
            }

            // Si terminamos después de este caracter
            if (_typingIndex >= formattedText.Length)
            {
                duration = TypingSpeed;
                return true;
            }

            // Colon
            if (currentChar == ':' && !isFirstCharacter)
            {
                duration = PauseOnPunctuationMarks ? TextTypingSettings.ColonPauseDuration : 0;
            }
            // Semicolon
            else if (currentChar == ';' && !isFirstCharacter)
            {
                duration = PauseOnPunctuationMarks ? TextTypingSettings.SemicolonPauseDuration : 0;
            }
            // Dot
            else if (currentChar == '.')
            {
                if (Abbreviations.Contains(lastWord))
                {
                    lastWord = string.Empty;
                    duration = 0; // No pausa en abreviaciones
                }
                else
                {
                    // Lógica de Elipsis (Look-ahead sin crear basura)
                    int dotCount = 1;
                    int lookAhead = _typingIndex;

                    // Miramos hacia adelante en el string original para ver si hay más puntos
                    while (lookAhead < formattedText.Length && formattedText[lookAhead] == '.')
                    {
                        // Agregamos los puntos extra inmediatamente al buffer visual para que aparezcan juntos
                        _renderBuffer.Append('.');
                        lookAhead++;
                        _typingIndex++; // Sincronizamos el índice principal
                        dotCount++;
                    }

                    if (dotCount >= 2) // Asumiendo que 2 o más puntos pausan más
                    {
                        duration = PauseOnPunctuationMarks ? TextTypingSettings.EllipsisPauseDuration : 0;
                    }
                    else
                    {
                        duration = PauseOnPunctuationMarks ? TextTypingSettings.DotPauseDuration : 0;
                    }
                }
            }
            else
            {
                duration = TypingSpeed;
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

            // ---------------------------------------------------------
            // 1. Lógica para Texto Asiático
            // ---------------------------------------------------------
            if (Multiline && ContainsAsianSymbols(text))
            {
                var wrappedLines = WrapAsianLines(lines);

                // Determinamos cuántas líneas vamos a mostrar realmente
                int limit = wrappedLines.Length;
                bool truncated = false;

                if (MaximumLines > 0 && limit > MaximumLines)
                {
                    limit = MaximumLines;
                    truncated = true;
                }

                LineCount = limit;

                for (var i = 0; i < limit; i++)
                {
                    if (string.IsNullOrEmpty(wrappedLines[i]))
                    {
                        stringBuilder.Append(Environment.NewLine);
                    }
                    else
                    {
                        // Si es la última línea permitida y hubo truncamiento, agregamos '...'
                        if (truncated && i == limit - 1)
                        {
                            stringBuilder.Append(wrappedLines[i] + ellipsesValue);
                        }
                        else
                        {
                            stringBuilder.Append(wrappedLines[i]);
                        }

                        // Agregamos salto de línea solo si NO es la última línea
                        if (i < limit - 1)
                        {
                            stringBuilder.Append(Environment.NewLine);
                        }
                    }
                }

                return stringBuilder.ToString();
            }

            // ---------------------------------------------------------
            // 2. Lógica para Texto Occidental (por palabras)
            // ---------------------------------------------------------
            for (var j = 0; j < lines.Length; j++)
            {
                float lineWidth = 0;
                var words = lines[j].Split(' ');

                for (var i = 0; i < words.Length; i++)
                {
                    var word = words[i];
                    var size = MeasureScaledString(word);
                    var addSpace = !(i == words.Length - 1);

                    // Verifica si la palabra cabe en la línea actual
                    if (lineWidth + size.X <= MaximumWidth)
                    {
                        stringBuilder.Append(word + (addSpace ? space : string.Empty));
                        lineWidth += size.X + spaceWidth;
                    }
                    else
                    {
                        // La palabra NO cabe, necesitamos una nueva línea.

                        // CASO A: Estamos en el límite de líneas permitido (o no es multilinea)
                        if (!Multiline || (MaximumLines > 0 && LineCount >= MaximumLines))
                        {
                            stringBuilder.Append(ellipsesValue);
                            ellipses = true; // Forzamos la salida del bucle de palabras
                            break;
                        }

                        // CASO B: Aún tenemos espacio para más líneas
                        stringBuilder.Append(Environment.NewLine + word + (addSpace ? space : string.Empty));
                        lineWidth = size.X + spaceWidth;
                        LineCount++;
                    }
                }

                // Si ya cortamos con elipsis, salimos del bucle de parrafos también
                if (ellipses)
                {
                    break;
                }

                // Si no es el último párrafo original y aún no alcanzamos el límite de líneas
                if (j < lines.Length - 1)
                {
                    if (MaximumLines > 0 && LineCount >= MaximumLines)
                    {
                        // Si agregar el salto de línea del párrafo nos pasaría del límite,
                        // agregamos elipsis al final de la línea actual y salimos.
                        stringBuilder.Append(ellipsesValue);
                        break;
                    }

                    stringBuilder.AppendLine();
                    LineCount++;
                }
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
                    // Lógica principal de tipeo optimizada
                    if (TryTypeText(out var duration))
                    {
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
        public string? DisplayText => _renderBuffer.ToString();

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

        // MaximumLines
        public int MaximumLines
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
            // Medimos el buffer actual (lo que se ve en pantalla)
            return _renderBuffer.Length == 0 ? Vector2.Zero : MeasureScaledString(_renderBuffer.ToString());
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
            // Checkeo de _formattedText para evitar arrancar con nada o basura
            if (IsEmpty || string.IsNullOrWhiteSpace(formattedText) || TypingSpeed == 0)
            {
                return;
            }

            if (sound != null && sound.IsDisposed)
            {
                sound = null;
            }

            lastWord = string.Empty;

            this.typingSound = sound;

            // REINICIO: Limpiamos buffer y cursor
            _renderBuffer.Clear();
            _typingIndex = 0;

            textTimer ??= new Timer();

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
                // Mostramos todo el texto restante
                _renderBuffer.Clear();
                _renderBuffer.Append(formattedText);
                _typingIndex = formattedText.Length;

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