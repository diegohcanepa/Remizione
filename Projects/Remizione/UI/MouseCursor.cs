using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// MouseCursor
    /// </summary>
    public sealed class MouseCursor : GameObject
    {
        #region Private fields

        private readonly ImageSprite cursorImage;
        private Vector2 position;
        private readonly Vector2Tween scaleTween = new();
        private readonly FloatTween shakeTween = new();

        #endregion

        #region Constructor

        // Constructor
        public MouseCursor(EngendroGame game)
            : base(game)
        {
            if (Instance != null)
                throw new InvalidOperationException("This class cannot be instantiated twice.");
            else
                Instance = this;

            this.cursorImage = new ImageSprite(game) { Scale = ScaleInfo.UIElement.Medium };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            cursorImage.X += shakeTween.IsRunning ? shakeTween.CurrentValue : 0;
            cursorImage.Draw(gameTime);
            cursorImage.X -= shakeTween.IsRunning ? shakeTween.CurrentValue : 0;
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (cursorImage.Image == null)
                cursorImage.Image = Atlases.UI.MouseCursorArrow;

            this.Position = InputManager.DefaultPlayer.Mouse.VirtualPosition;
            cursorImage.Update(gameTime);
            shakeTween.Update(gameTime);
        }

        #endregion

        // AnimateClick
        public void AnimateClick()
        {
            scaleTween.Start(TweenStyle.QuadraticIn, ScaleInfo.UIElement.Small, ScaleInfo.UIElement.Medium, 150);
            cursorImage.Tweens.ScaleTween = scaleTween;
        }

        // AnimateSwitch
        public void AnimateSwitch()
        {
            scaleTween.Start(TweenStyle.QuadraticIn, new(.2f), ScaleInfo.UIElement.Medium, 100);
            cursorImage.Tweens.ScaleTween = scaleTween;
        }

        // Instance
        public static MouseCursor Instance { get; private set; } = null!;

        // Position
        public Vector2 Position
        {
            get => position;
            set
            {
                this.position = value;
                cursorImage.Position = value;
            }
        }

        // Shake
        public void Shake() => shakeTween.Start(TweenStyle.CubicInOut, 0, 1, 50, 4);
    }
}
