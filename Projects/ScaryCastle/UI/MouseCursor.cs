using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// MouseCursor
    /// </summary>
    public sealed class MouseCursor : GameObject
    {
        #region Private fields

        private readonly Vector2Tween attackTween = Vector2Tween.Create(TweenStyle.CubicInOut, ScaleInfo.UIElement.Medium, ScaleInfo.UIElement.Medium * .8f, 130, -1);
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

            this.cursorImage = new ImageSprite(game) { PivotOrigin = RectanglePoint.Center, Scale = ScaleInfo.UIElement.Medium };
        }

        #endregion

        #region Private members

        // Invalidate
        private void Invalidate()
        {
            if (State == MouseCursorState.Arrow)
                cursorImage.Image = Atlases.UI.MouseCursorArrow;

            else if (State == MouseCursorState.Cross)
                cursorImage.Image = Atlases.UI.MouseCursorCross;

            else if (State == MouseCursorState.CrossOn)
                cursorImage.Image = Atlases.UI.MouseCursorCrossOn;

            else if (State == MouseCursorState.CustomImage)
                cursorImage.Image = CustomImage;

            else if (State == MouseCursorState.Wait)
                cursorImage.Image = Atlases.UI.MouseCursorWait;

            cursorImage.Scale = ScaleInfo.UIElement.Medium;
            cursorImage.PivotOrigin = State == MouseCursorState.Arrow ? RectanglePoint.LeftTop : RectanglePoint.Center;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (State == MouseCursorState.None)
                return;

            Game.SpriteBatch.Begin(Game.Camera);
            cursorImage.X += shakeTween.IsRunning ? shakeTween.CurrentValue : 0;
            cursorImage.Draw(gameTime);
            cursorImage.X -= shakeTween.IsRunning ? shakeTween.CurrentValue : 0;
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            attackTween.Update(gameTime);

            this.Position = InputManager.DefaultPlayer.Mouse.VirtualPosition;

            if (cursorImage.Image == null)
                Invalidate();

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

        // CustomImage
        public AtlasImage? CustomImage { get; set; }

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
        public void Shake()
        {
            shakeTween.Start(TweenStyle.CubicInOut, 0, 1, 50, 4);
        }

        // State
        public MouseCursorState State
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
    }
}
