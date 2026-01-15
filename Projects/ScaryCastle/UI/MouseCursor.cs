using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScaryCastle.Effects;
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
        private readonly ImageSprite cursorSprite;
        private readonly ImageSprite customCursorSprite;
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

            this.customCursorSprite = new ImageSprite(game) { PivotOrigin = RectanglePoint.Center, Scale = ScaleInfo.UIElement.Medium };
            this.cursorSprite = new ImageSprite(game) { PivotOrigin = RectanglePoint.Center, Scale = ScaleInfo.UIElement.Medium };
        }

        #endregion

        #region Private members

        // GetActiveCursor
        private ImageSprite GetActiveCursor()
        { 
            return customCursorSprite.IsEmpty? cursorSprite : customCursorSprite;
        }

        // Invalidate
        private void Invalidate()
        {
            if (State == MouseCursorState.Arrow)
                cursorSprite.Image = Atlases.UI.MouseCursorArrow;

            else if (State == MouseCursorState.Cross)
                cursorSprite.Image = Atlases.UI.MouseCursorCross;

            else if (State == MouseCursorState.CrossOn)
                cursorSprite.Image = Atlases.UI.MouseCursorCrossOn;

            else if (State == MouseCursorState.Wait)
                cursorSprite.Image = Atlases.UI.MouseCursorWait;

            cursorSprite.Scale = ScaleInfo.UIElement.Medium;
            cursorSprite.PivotOrigin = State == MouseCursorState.Arrow ? RectanglePoint.LeftTop : RectanglePoint.Center;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (State == MouseCursorState.None)
                return;

            var sprite = GetActiveCursor();
            OutlineEffect? effect = Highlight ? ScaryCastleGame.Effects.Outline : null;

            if (effect != null && sprite.Image?.Atlas != null)
            {
                effect.Color.SetValue(ColorPalette.MouseCursorOutline);
                effect.TextureSize.SetValue(new Vector2(sprite.Image.Atlas.Texture.Width, sprite.Image.Atlas.Texture.Height));
                effect.Thickness.SetValue(1);
            }

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp, effect?.Effect);
            sprite.X += shakeTween.IsRunning ? shakeTween.CurrentValue : 0;
            sprite.Draw(gameTime);
            sprite.X -= shakeTween.IsRunning ? shakeTween.CurrentValue : 0;
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            attackTween.Update(gameTime);

            GetActiveCursor().Position = InputManager.DefaultPlayer.Mouse.VirtualPosition;

            if (cursorSprite.Image == null)
                Invalidate();

            cursorSprite.Update(gameTime);

            shakeTween.Update(gameTime);
        }

        #endregion

        // AnimateClick
        public void AnimateClick()
        {
            scaleTween.Start(TweenStyle.QuadraticIn, ScaleInfo.UIElement.Small, ScaleInfo.UIElement.Medium, 150);
            cursorSprite.Tweens.ScaleTween = scaleTween;
        }

        // AnimateSwitch
        public void AnimateSwitch()
        {
            scaleTween.Start(TweenStyle.QuadraticIn, new(.2f), ScaleInfo.UIElement.Medium, 100);
            cursorSprite.Tweens.ScaleTween = scaleTween;
        }

        // CustomImageTag
        public object? CustomImageTag { get; private set; }

        // Highlight
        public bool Highlight { get; set; }

        // Instance
        public static MouseCursor Instance { get; private set; } = null!;

        // Reset
        public void Reset()
        {
            Highlight = false;
            customCursorSprite.Image = null;
            CustomImageTag = null;
        }

        // Shake
        public void Shake()
        {
            shakeTween.Start(TweenStyle.CubicInOut, 0, 1, 50, 4);
        }

        // SetCustomImage
        public void SetCustomImage(AtlasImage image, object? tag)
        {
            customCursorSprite.Image = image;
            CustomImageTag = tag;
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
