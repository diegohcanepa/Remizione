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

        private readonly Vector2Tween attackTween = Vector2Tween.Create(TweenStyle.CubicInOut, ScaleInfo.UIElement.Medium, ScaleInfo.UIElement.Medium * .8f, 130, -1);
        private readonly ImageSprite cursorImage;
        private Vector2 position;
        private readonly Vector2Tween scaleTween = new();
        private readonly FloatTween shakeTween = new();
        private MouseCursorState state;

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

            this.cursorImage = new ImageSprite(game) { PivotOrigin = RectanglePoint.Middle, Scale = ScaleInfo.UIElement.Medium };
        }

        #endregion

        #region Private members

        // Invalidate
        private void Invalidate()
        {
            if (state == MouseCursorState.Arrow)
                cursorImage.Image = Atlases.UI.MouseCursorArrow;

            else if (state == MouseCursorState.Cross)
                cursorImage.Image = Atlases.UI.MouseCursorCross;

            else if (state == MouseCursorState.CrossOn)
                cursorImage.Image = Atlases.UI.MouseCursorCrossOn;

            else if (state == MouseCursorState.Target)
                cursorImage.Image = Atlases.UI.MouseCursorTarget;

            else if (state == MouseCursorState.TargetOn)
                cursorImage.Image = Atlases.UI.MouseCursorTargetOn;

            else if (state == MouseCursorState.Wait)
                cursorImage.Image = Atlases.UI.MouseCursorWait;

            cursorImage.Scale = ScaleInfo.UIElement.Medium;
            cursorImage.PivotOrigin = state == MouseCursorState.Arrow ? RectanglePoint.LeftTop : RectanglePoint.Middle;
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
            attackTween.Update(gameTime);

            this.Position = InputManager.DefaultPlayer.Mouse.VirtualPosition;

            if (cursorImage.Image == null)
                Invalidate();

            cursorImage.Update(gameTime);

            shakeTween.Update(gameTime);

            if (state == MouseCursorState.TargetOn && !scaleTween.IsRunning && !shakeTween.IsRunning)
                cursorImage.Scale = attackTween.CurrentValue;
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

        // State
        public MouseCursorState State
        {
            get => state;
            set
            {
                if (value != state)
                {
                    if (state == MouseCursorState.Cross)
                    {
                        if (value == MouseCursorState.Target || value == MouseCursorState.TargetOn)
                            AnimateSwitch();
                    }

                    state = value;
                    Invalidate();
                }
            }
        }
    }
}
