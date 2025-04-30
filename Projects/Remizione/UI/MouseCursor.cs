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

            this.cursorImage = new ImageSprite(game) { PivotOrigin = RectanglePoint.Middle, Scale = ScaleInfo.UIIcon.Medium };
        }

        #endregion

        #region Private members

        // Invalidate
        private void Invalidate()
        {
            if (state == MouseCursorState.CombatMode)
                cursorImage.Image = Atlases.UI.MouseCursorCombatMode;

            else if (state == MouseCursorState.Default)
                cursorImage.Image = Atlases.UI.MouseCursorDefault;

            else if (state == MouseCursorState.Target)
                cursorImage.Image = Atlases.UI.MouseCursorTarget;

            else if (state == MouseCursorState.Wait)
                cursorImage.Image = Atlases.UI.MouseCursorWait;

            cursorImage.PivotOrigin = state == MouseCursorState.Default ? RectanglePoint.LeftTop : RectanglePoint.Middle;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            cursorImage.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            this.Position = InputManager.DefaultPlayer.Mouse.VirtualPosition;

            if (cursorImage.Image == null)
                Invalidate();

            cursorImage.Update(gameTime);
        }

        #endregion

        // AnimateClick
        public void AnimateClick()
        {
            scaleTween.Start(TweenStyle.QuadraticIn, ScaleInfo.UIIcon.Small, ScaleInfo.UIIcon.Medium, 150);
            cursorImage.Tweens.ScaleTween = scaleTween;
        }

        // AnimateSwitch
        public void AnimateSwitch()
        {
            scaleTween.Start(TweenStyle.QuadraticIn, new(.2f), ScaleInfo.UIIcon.Medium, 100);
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

        // State
        public MouseCursorState State
        {
            get => state;
            set
            {
                if (value != state)
                {
                    if (state == MouseCursorState.Default)
                    {
                        if (value == MouseCursorState.CombatMode || value == MouseCursorState.Target)
                            AnimateSwitch();
                    }

                    state = value;
                    Invalidate();
                }
            }
        }
    }
}
