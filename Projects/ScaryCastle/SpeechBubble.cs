using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ScaryCastle
{
    /// <summary>
    /// SpeechBubble
    /// </summary>
    public sealed class SpeechBubble : GameObject
    {
        #region Constants

        private const string exclamationLow = "!";
        private const string exclamationMedium = "!!";
        private const string exclamationHigh = "!!!";
        private const int maxWidth = 80;

        #endregion

        #region Private fields

        private static readonly List<SpeechBubble> activeBubbles = [];
        private int autoHideCooldown;
        private RectangleF bubbleArea;
        private int inputCooldown;
        private readonly Vector2Tween shakeTween = new();
        private readonly TextSprite text;

        #endregion

        #region Constructor

        // Constructor
        public SpeechBubble(Actor actor)
        {
            this.Actor = actor;

            // Text
            this.text = new TextSprite(Fonts.CommonOutline)
            {
                MaximumWidth = maxWidth,
                PivotOrigin = RectanglePoint.LeftTop
            };
        }

        #endregion

        #region Private members

        // CalculateBubbleArea
        private RectangleF CalculateBubbleArea(ref Vector2 origin)
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

            bubbleArea = CalculateBubbleArea(ref origin);

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

            text.Position = bubbleArea.GetPoint(RectanglePoint.LeftTop, 2, -2);
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
            if (State == SpeechBubbleState.Typing)
            {
                if (text.TypingState == RunningState.Stopped)
                {
                    Actor.StopTalking();
                    State = SpeechBubbleState.Idle;
                }
                else if (AwaitInput && (InputBindings.SpeechBubble.IsPressed(0) || InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed()) && inputCooldown <= 0)
                {
                    if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.Mouse)
                        MouseCursor.PerformClick();

                    State = SpeechBubbleState.Idle;
                    text.StopTyping();
                    Actor.StopTalking();
                    Layout();
                }
            }

            else if (State == SpeechBubbleState.Idle)
            {
                if (autoHideCooldown > 0)
                {
                    autoHideCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                    if (autoHideCooldown <= 0)
                        Hide();
                }
                else if (AwaitInput && (InputBindings.SpeechBubble.IsPressed(0) || InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed()))
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
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (inputCooldown > 0)
                inputCooldown -= gameTime.ElapsedGameTime.Milliseconds;

            UpdateState(gameTime);
            shakeTween.Update(gameTime);
            text.Update(gameTime);

            if (State == SpeechBubbleState.Typing)
                Layout();
        }

        #endregion

        // Actor
        public Actor Actor { get; }

        // AwaitInput
        public bool AwaitInput { get; private set; }

        // DrawSpeechBubbles
        public static void DrawSpeechBubbles(GameTime gameTime)
        {
            for (int i = 0; i < VisibleBubbles.Count; i++)
            {
                VisibleBubbles[i].Draw(gameTime);
            }
        }

        // Hide
        public void Hide()
        {
            activeBubbles.Remove(this);

            if (ModalInstance == this)
                ModalInstance = null;

            AwaitInput = false;
            text.Clear();
            text.StopTyping();
            shakeTween.Stop();
            State = SpeechBubbleState.Hidden;
        }

        // ModalInstance
        public static SpeechBubble? ModalInstance { get; private set; }

        // Show
        public void Show(string text, Color color, bool awaitInput)
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

            if (!activeBubbles.Contains(this))
                activeBubbles.Add(this);

            this.text.Text = text;
            this.text.Color = color;

            if (awaitInput)
                autoHideCooldown = 0;
            else
                autoHideCooldown = text.Length * this.text.TypingSpeed;

            this.text.Scale = ScaleInfo.SpeechBubble.Text;

            // Typing
            if (SpeechBubbleSettings.TextTyping && awaitInput)
            {
                if (SpeechBubbleSettings.TextTypingSound)
                    this.text.StartTyping(Actor.SpeechSound?.PopInstance());
                else
                    this.text.StartTyping();

                State = SpeechBubbleState.Typing;
            }

            Layout();

            Shake(text);

            Actor.StartTalking();

            inputCooldown = 100;
        }

        // State
        public SpeechBubbleState State { get; private set; }

        // Text
        public string? Text => text.Text;

        // VisibleBubbles
        public static ReadOnlyCollection<SpeechBubble> VisibleBubbles { get; } = new ReadOnlyCollection<SpeechBubble>(activeBubbles);
    }
}
