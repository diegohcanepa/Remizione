using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
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
        private int inputCooldown;
        private readonly Vector2Tween shakeTween = new();
        private readonly TextSprite text;

        private bool isPositionedBelow;

        #endregion

        #region Constructor

        public SpeechText(Actor actor)
        {
            this.Actor = actor;

            // Text
            this.text = new TextSprite(Fonts.CommonOutline)
            {
                Color = ColorPalette.SpeechText.Text,
                MaximumWidth = maxWidth,
                PivotOrigin = RectanglePoint.LeftTop,
            };
        }

        #endregion

        #region Private members

        // Layout
        private void Layout()
        {
            var vp = Actor.Session.Camera.VisibleBox;
            vp.Inflate(-10, -10);

            var origin = Actor.GetOverheadPosition();
            origin.Y -= 1;

            float totalHeight = text.MeasureDisplayText().Y;
            float totalWidth = this.text.BoundingBox.Width;

            // Determinar posición Y (arriba o abajo del actor dependiendo de la cámara)
            float expectedY = origin.Y - totalHeight - 2;
            isPositionedBelow = expectedY < vp.Top;

            if (isPositionedBelow)
            {
                expectedY = Actor.BoundingBox.Bottom + 4; // Margen debajo de los pies
            }

            // Determinar posición X (centrado, pero respetando los bordes de la cámara)
            float expectedX = origin.X - (totalWidth / 2f);

            if (expectedX < vp.Left)
                expectedX = vp.Left;
            else if (expectedX + totalWidth > vp.Right)
                expectedX = vp.Right - totalWidth;

            text.Position = new Vector2(expectedX, expectedY);
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
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (inputCooldown > 0)
                inputCooldown -= gameTime.ElapsedGameTime.Milliseconds;

            UpdateState(gameTime);
            shakeTween.Update(gameTime);
            text.Update(gameTime);

            if (State == SpeechTextState.Typing)
                Layout();
        }

        #endregion

        public Actor Actor { get; }

        public bool AwaitInput { get; private set; }

        public static void DrawSpeechTexts(GameTime gameTime)
        {
            for (int i = 0; i < VisibleBubbles.Count; i++)
            {
                VisibleBubbles[i].Draw(gameTime);
            }
        }

        public void Hide()
        {
            activeTexts.Remove(this);

            if (ModalInstance == this)
                ModalInstance = null;

            AwaitInput = false;
            text.Clear();
            text.StopTyping();
            shakeTween.Stop();
            State = SpeechTextState.Hidden;
        }

        public static SpeechText? ModalInstance { get; private set; }

        public void Show(string text, bool awaitInput)
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
            this.text.Scale = ScaleInfo.Text.Large;

            if (awaitInput)
                autoHideCooldown = 0;
            else
                autoHideCooldown = text.Length * this.text.TypingSpeed;

            // Typing
            if (SpeechTextSettings.Typing && awaitInput)
            {
                if (SpeechTextSettings.TypingSound)
                {
                    this.text.StartTyping(Actor.SpeechSound?.PopInstance());
                }
                else
                {
                    this.text.StartTyping();
                }

                State = SpeechTextState.Typing;
            }

            Layout();
            Shake(text);
            Actor.StartTalking();
            inputCooldown = 100;
        }

        public SpeechTextState State { get; private set; }

        public string? Text => text.Text;

        // Se mantiene el nombre de la colección por si la llamas desde fuera
        public static ReadOnlyCollection<SpeechText> VisibleBubbles { get; } = new ReadOnlyCollection<SpeechText>(activeTexts);
    }
}