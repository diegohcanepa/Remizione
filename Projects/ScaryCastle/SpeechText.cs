using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private const int maxWidth = 90;

        #endregion

        #region Private fields

        private static readonly List<SpeechText> activeTexts = [];
        private int autoHideCooldown;
        private readonly Sprite arrowImage;
        private readonly FloatTween arrowTween = new();
        private int inputCooldown;
        private readonly Vector2Tween shakeTween = new();
        private readonly TextSprite text;
        private RectangleF textArea;

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

            // Text
            this.text = new TextSprite(Fonts.CommonOutline)
            {
                MaximumWidth = maxWidth,
                PivotOrigin = RectanglePoint.LeftTop
            };
        }

        #endregion

        #region Private members

        // CalculateTextArea
        private RectangleF CalculateTextArea(ref Vector2 origin)
        {
            float w = text.BoundingBox.Width;
            float h = text.MeasureDisplayText().Y + 2;

            return new(origin.X - (w / 2), origin.Y - h, w, h);
        }

        // Layout
        private void Layout()
        {
            var vp = Actor.Session.Camera.VisibleBox;
            vp.Inflate(-10, -10);

            // Origin
            var origin = Actor.GetOverheadPosition();
            origin.Y -= 1;

            textArea = CalculateTextArea(ref origin);

            // Test overlapping (left side)
            if (textArea.Left < vp.Left)
                textArea.X = vp.X;

            // Test overlapping (right side)
            else if (textArea.Right > vp.Right)
                textArea.X = vp.Right - textArea.Width;

            // Test overlapping (top side)
            if (textArea.Top < vp.Top)
                textArea.Y = vp.Y;

            // Test overlapping (bottom side)
            else if (textArea.Bottom > vp.Bottom)
                textArea.Y = vp.Bottom - textArea.Height;

            text.Position = textArea.GetPoint(RectanglePoint.LeftTop, 2, -2);
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
                    Actor.StopTalking();
                    State = SpeechTextState.Idle;
                }
                else if (AwaitInput && (InputBindings.SpeechText.IsPressed(0) || InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed()) && inputCooldown <= 0)
                {
                    if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.Mouse)
                        MouseCursor.PerformClick();

                    State = SpeechTextState.Idle;
                    text.StopTyping();
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
            Game.SpriteBatch.Begin(Actor.Session.Camera, SamplerState.LinearClamp);

            if (shakeTween.IsRunning)
                text.Position += shakeTween.CurrentValue;

            text.Draw(gameTime);

            if (shakeTween.IsRunning)
                text.Position -= shakeTween.CurrentValue;

            Game.SpriteBatch.End();

            if (!text.IsTyping)
            {
                Game.SpriteBatch.Begin(Actor.Session.Camera);
                arrowImage.Draw(gameTime);
                Game.SpriteBatch.End();
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (inputCooldown > 0)
                inputCooldown -= gameTime.ElapsedGameTime.Milliseconds;
            
            arrowTween.Update(gameTime);
            UpdateState(gameTime);
            shakeTween.Update(gameTime);
            text.Update(gameTime);

            if (State == SpeechTextState.Typing)
                Layout();

            if (!text.IsTyping)
            {
                arrowImage.Position = text.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, -arrowTween.CurrentValue);
                arrowImage.X = Actor.GetOverheadPosition().X;
            }
        }

        #endregion

        // Actor
        public Actor Actor { get; }

        // AwaitInput
        public bool AwaitInput { get; private set; }

        // DrawSpeechTexts
        public static void DrawSpeechTexts(GameTime gameTime)
        {
            for (int i = 0; i < VisibleTexts.Count; i++)
            {
                VisibleTexts[i].Draw(gameTime);
            }
        }

        // Hide
        public void Hide()
        {
            activeTexts.Remove(this);

            if (ModalInstance == this)
                ModalInstance = null;

            AwaitInput = false;
            arrowTween.Stop();
            text.Clear();
            text.StopTyping();
            shakeTween.Stop();
            State = SpeechTextState.Hidden;
        }

        // ModalInstance
        public static SpeechText? ModalInstance { get; private set; }

        // Show
        public void Show(string text, Color color, bool awaitInput)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            arrowImage.Color = color;

            arrowTween.Start(TweenStyle.QuinticIn, 0, .3f, 150, -1);

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
            this.text.Color = color;

            if (awaitInput)
                autoHideCooldown = 0;
            else
                autoHideCooldown = text.Length * this.text.TypingSpeed;

            this.text.Scale = ScaleInfo.Text.Medium;

            // Typing
            if (SpeechTextSettings.Typing && awaitInput)
            {
                if (SpeechTextSettings.TypingSound)
                    this.text.StartTyping(Actor.SpeechSound?.PopInstance());
                else
                    this.text.StartTyping();

                State = SpeechTextState.Typing;
            }

            Layout();

            Shake(text);

            Actor.StartTalking();

            inputCooldown = 100;
        }

        // State
        public SpeechTextState State { get; private set; }

        // Text
        public string? Text => text.Text;

        // VisibleTexts
        public static ReadOnlyCollection<SpeechText> VisibleTexts { get; } = new ReadOnlyCollection<SpeechText>(activeTexts);
    }
}
