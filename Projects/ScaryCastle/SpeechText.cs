using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ScaryCastle
{
    /// <summary>
    /// SpeechText
    /// </summary>
    public sealed class SpeechText : GameObject
    {
        #region Constants

        private const string exclamationLow = "!";
        private const string exclamationMedium = "!!";
        private const string exclamationHigh = "!!!";
        private const int maxWidth = 80;
        private static readonly Vector2 textPadding = new(3, 2);

        #endregion

        #region Private fields

        private static readonly List<SpeechText> activeTexts = [];
        private readonly Sprite arrowImage;
        private readonly FloatTween arrowTween = new();
        private int autoHideCooldown;
        private RectangleF bubbleArea;
        private readonly Sprite bubbleImage;
        private readonly Sprite bubbleImage2;
        private int inputCooldown;
        private readonly Sprite pipe;
        private readonly float pipeHeight;
        private readonly FloatTween pipeTween = new();
        private readonly Vector2Tween shakeTween = new();
        private readonly TextSprite text;
        private readonly TextSprite title;

        #endregion

        #region Constructor

        // Constructor
        public SpeechText(Actor actor)
        {
            this.Actor = actor;

            // Arrow
            this.arrowImage = new Sprite(Atlases.UI.SpeechTextArrow)
            {
                Scale = ScaleInfo.UIElement.Medium
            };

            // Bubble 1
            this.bubbleImage = new Sprite(Atlases.UI.Pixel);

            // Bubble 2
            this.bubbleImage2 = new Sprite(Atlases.UI.Pixel);

            // Pipe
            pipe = new Sprite(Atlases.UI.SpeechTextPipe)
            {
                Color = ColorPalette.SpeechText.Fill,
                PivotOrigin = RectanglePoint.Bottom
            };
            pipeHeight = pipe.BoundingBox.Height;

            // Text
            this.text = new TextSprite(Fonts.Common)
            {
                Color = ColorPalette.SpeechText.Text,
                MaximumWidth = maxWidth,
                PivotOrigin = RectanglePoint.LeftTop,
                ShadowColor = Color.Black * .3f,
                ShadowOffset = new(0, .3f)
            };

            // Title
            this.title = new TextSprite(Fonts.Common)
            {
                Color = ColorPalette.SpeechText.Title,
                MaximumWidth = maxWidth,
                PivotOrigin = RectanglePoint.LeftTop,
                Scale = ScaleInfo.Text.Medium
            };
        }

        #endregion

        #region Private members

        // DrawBubble
        private void DrawBubble(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Actor.Session.Camera, SamplerState.PointClamp);

            // Draw bubble shadow
            bubbleImage.Color = ColorPalette.SpeechText.Shadow;
            bubbleImage2.Color = ColorPalette.SpeechText.Shadow;
            bubbleImage.Position += Vector2.One;
            bubbleImage2.Position += Vector2.One;
            bubbleImage2.Draw(gameTime);
            bubbleImage.Draw(gameTime);
            bubbleImage.Position -= Vector2.One;
            bubbleImage2.Position -= Vector2.One;

            // Draw bubble
            bubbleImage.Color = ColorPalette.SpeechText.Fill;
            bubbleImage2.Color = ColorPalette.SpeechText.Fill;
            bubbleImage2.Draw(gameTime);
            bubbleImage.Draw(gameTime);

            if (text.TypingState == RunningState.Running)
            {
                pipe.ScaleY += pipeTween.CurrentValue;
                pipe.Y += Math.Abs(pipeHeight - pipe.BoundingBox.Height);
            }

            // Shadow
            if (!pipe.IsFlippedVertically)
            {
                pipe.Color = ColorPalette.SpeechText.Shadow;
                pipe.Position += Vector2.One;
                pipe.Draw(gameTime);
                pipe.Position -= Vector2.One;
                pipe.Color = ColorPalette.SpeechText.Fill;
            }

            pipe.Draw(gameTime);

            if (text.TypingState == RunningState.Running)
            {
                pipe.ScaleY -= pipeTween.CurrentValue;
                pipe.Y -= Math.Abs(pipeHeight - pipe.BoundingBox.Height);
            }

            //arrowImage.Draw(gameTime);

            Game.SpriteBatch.End();
        }

        // GetBubbleArea
        private RectangleF GetBubbleArea(ref Vector2 origin)
        {
            float w = Math.Max(text.BoundingBox.Width, title.BoundingBox.Width);
            float h = title.BoundingBox.Height + text.MeasureDisplayText().Y + 2;

            bubbleArea = new RectangleF(origin.X - (w / 2), origin.Y - h - pipe.BoundingBox.Height + 1, w, h);
            bubbleArea.Inflate(textPadding);

            return bubbleArea;
        }

        // Layout
        private void Layout()
        {
            var vp = Actor.Session.Camera.VisibleBox;
            vp.Inflate(-10, -10);

            // Origin
            var origin = Actor.GetOverheadPosition();
            origin.Y -= 1;
            //origin.X = Actor.X;

            pipe.Effects = SpriteEffects.None;
            pipe.PivotOrigin = RectanglePoint.Bottom;
            pipe.Position = origin;
            bubbleArea = GetBubbleArea(ref origin);

            // Test overlapping (left side)
            if (bubbleArea.Left < vp.Left)
                bubbleArea.X = vp.X;

            // Test overlapping (right side)
            else if (bubbleArea.Right > vp.Right)
                bubbleArea.X = vp.Right - bubbleArea.Width;

            // Test overlapping (top side)
            if (bubbleArea.Top < vp.Top)
                bubbleArea.Y = vp.Y;

            // Test overlapping (bottom side)
            else if (bubbleArea.Bottom > vp.Bottom)
                bubbleArea.Y = vp.Bottom - bubbleArea.Height;

            bubbleImage.Position = bubbleArea.GetPoint(RectanglePoint.LeftTop, 1, 1);
            bubbleImage.Scale = new Vector2(bubbleArea.Width - 2, bubbleArea.Height - 2);

            var bbox = bubbleImage.BoundingBox;

            bubbleImage2.Position = bbox.GetPoint(RectanglePoint.LeftTop, -1, 1);
            bubbleImage2.Scale = new Vector2(bbox.Width + 2, bbox.Height - 2);

            var pipeBox = pipe.BoundingBox;

            bbox = bubbleImage2.BoundingBox;

            // Limit pipe (horz)
            if (pipeBox.Left < bbox.Left + 1)
                pipe.X = bbox.Left + (pipeBox.Width / 2) + 1;
            else if (pipeBox.Right > bbox.Right)
                pipe.X = bbox.Right - (pipeBox.Width / 2) - 2;

            // Limit pipe (vert)
            pipe.Y = bubbleArea.Bottom + pipeBox.Height - 1.2f;

            pipeBox = pipe.BoundingBox;
            if (pipeBox.Bottom > origin.Y + 3)
            {
                pipe.Effects = SpriteEffects.FlipVertically;
                pipe.Y = Actor.BoundingBox.Bottom + pipeBox.Height + 2;
                pipeBox = pipe.BoundingBox;
                bubbleImage.Y = pipeBox.Bottom;
                bbox = bubbleImage.BoundingBox;
                bubbleImage2.Position = bbox.GetPoint(RectanglePoint.LeftTop, -1, 1);
                bubbleImage2.Scale = new Vector2(bbox.Width + 2, bbox.Height - 2);
            }

            title.Position = bubbleImage.BoundingBox.GetPoint(RectanglePoint.LeftTop, 2, 2);
            text.Position = title.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 0, -.5f);
        }

        // Shake
        private void Shake(string text)
        {
            shakeTween.Stop();
            var bounces = 0;
            float force = 0;

            if (text.Contains(exclamationMedium) || text.Contains(exclamationHigh))
            {
                force = .4f;
                bounces = 9;
            }
            else if (text.Contains(exclamationLow))
            {
                force = .1f;
                bounces = 6;
            }

            if (force > 0)
                shakeTween.Start(TweenStyle.Linear, new Vector2(-force), new Vector2(force), 50, bounces);
        }

        // UpdateState
        private void UpdateState(GameTime gameTime)
        {
            if (State == SpeechTextState.Typing)
            {
                if (text.TypingState == RunningState.Stopped)
                {
                    //if (Actor.IsStandingOrMoving)
                    Actor.StopTalking();

                    // Made one last scale tween, so pipe ends in a 1:1 scale
                    if (pipeTween.IsRunning)
                        pipeTween.Start(pipeTween.Style, pipe.Scale.Y, 1, pipeTween.Duration);

                    State = SpeechTextState.Idle;
                }
                else if (AwaitInput && (InputBindings.SpeechText.IsPressed(0) || InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed()) && inputCooldown <= 0)
                {
                    if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.Mouse)
                        MouseCursor.PerformClick();

                    State = SpeechTextState.Idle;
                    text.StopTyping();

                    //if (Actor.IsStandingOrMoving)
                    Actor.StopTalking();

                    Layout();
                }
            }

            else if (State == SpeechTextState.Idle)
            {
                if (autoHideCooldown > 0)
                {
                    autoHideCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                    if (autoHideCooldown <= 0)
                        Hide();
                }
                else if (AwaitInput && (InputBindings.SpeechText.IsPressed(0) || InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed()))
                {
                    if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.Mouse)
                        MouseCursor.PerformClick();

                    Hide();
                }
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            DrawBubble(gameTime);

            Game.SpriteBatch.Begin(Actor.Session.Camera, SamplerState.LinearClamp);

            title.Draw(gameTime);

            if (shakeTween.IsRunning)
                text.Position += shakeTween.CurrentValue;

            text.Draw(gameTime);

            if (shakeTween.IsRunning)
                text.Position -= shakeTween.CurrentValue;

            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (inputCooldown > 0)
                inputCooldown -= gameTime.ElapsedGameTime.Milliseconds;

            UpdateState(gameTime);
            arrowTween.Update(gameTime);
            pipeTween.Update(gameTime);
            shakeTween.Update(gameTime);
            text.Update(gameTime);
            title.Update(gameTime);

            if (State == SpeechTextState.Typing)
                Layout();

            arrowImage.Position = bubbleImage.BoundingBox.GetPoint(RectanglePoint.RightBottom, -2.5f, -2.5f - arrowTween.CurrentValue);
        }

        #endregion

        // Actor
        public Actor Actor { get; }

        // AwaitInput
        public bool AwaitInput { get; private set; }

        // DrawSpeechTexts
        public static void DrawSpeechTexts(GameTime gameTime)
        {
            for (int i = 0; i < VisibleBubbles.Count; i++)
            {
                VisibleBubbles[i].Draw(gameTime);
            }
        }

        // Hide
        public void Hide()
        {
            activeTexts.Remove(this);

            if (ModalInstance == this)
                ModalInstance = null;

            AwaitInput = false;
            text.Clear();
            title.Clear();
            text.StopTyping();
            shakeTween.Stop();
            arrowTween.Stop();
            State = SpeechTextState.Hidden;
        }

        // ModalInstance
        public static SpeechText? ModalInstance { get; private set; }

        // Show
        public void Show(string title, string text, bool awaitInput)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            if (awaitInput && ModalInstance != null)
            {
                ModalInstance.Hide();
                ModalInstance = null;
            }

            AwaitInput = awaitInput;

            if (awaitInput)
                ModalInstance = this;

            if (!activeTexts.Contains(this))
                activeTexts.Add(this);

            this.text.Text = text;
            this.title.Text = title;

            if (awaitInput)
                autoHideCooldown = 0;
            else
                autoHideCooldown = text.Length * this.text.TypingSpeed;

            this.text.Scale = ScaleInfo.Text.Large;

            // Typing
            if (SpeechTextSettings.Typing && awaitInput)
            {
                if (SpeechTextSettings.TypingSound)
                    this.text.StartTyping(Actor.SpeechSound?.PopInstance());
                else
                    this.text.StartTyping();

                State = SpeechTextState.Typing;
                pipeTween.Start(TweenStyle.Linear, 0, .25f, 100, -1);
            }

            Layout();

            Shake(text);

            arrowTween.Start(TweenStyle.QuinticIn, 0, .3f, 150, -1);

            //if (Actor.IsStandingOrMoving)
            Actor.StartTalking();

            inputCooldown = 100;
        }

        // State
        public SpeechTextState State { get; private set; }

        // Text
        public string? Text => text.Text;

        // VisibleTexts
        public static ReadOnlyCollection<SpeechText> VisibleBubbles { get; } = new ReadOnlyCollection<SpeechText>(activeTexts);
    }
}
