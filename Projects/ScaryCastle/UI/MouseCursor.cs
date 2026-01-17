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
    public static class MouseCursor
    {
        #region Private fields

        private static readonly AtlasImage[] cursorImages;
        private static readonly ImageSprite cursorSprite;
        private static readonly ImageSprite customCursorSprite;
        private static readonly Vector2Tween scaleTween = new();
        private static readonly FloatTween shakeTween = new();

        #endregion

        #region Constructor

        // Constructor
        static MouseCursor()
        {
            customCursorSprite = new ImageSprite(EngendroGame.Instance) { PivotOrigin = RectanglePoint.Center, Scale = ScaleInfo.UIElement.Medium };
            cursorSprite = new ImageSprite(EngendroGame.Instance) { PivotOrigin = RectanglePoint.Center, Scale = ScaleInfo.UIElement.Medium };

            const string prefix = "MouseCursor";
            var names = Enum.GetNames<MouseCursorState>();

            cursorImages = new AtlasImage[names.Length];
            for (var i = 0; i < cursorImages.Length; i++)
            {
                var imageName = $"{prefix}{names[i]}";
                cursorImages[i] = Atlases.UI.GetImage(imageName);
            }
        }

        #endregion

        #region Private members

        // GetActiveCursor
        private static ImageSprite GetActiveCursor()
        {
            return customCursorSprite.IsEmpty ? cursorSprite : customCursorSprite;
        }

        // Invalidate
        private static void Invalidate()
        {
            cursorSprite.Image = cursorImages[(int)State];
            cursorSprite.Scale = ScaleInfo.UIElement.Medium;
            cursorSprite.PivotOrigin = State == MouseCursorState.Arrow ? RectanglePoint.LeftTop : RectanglePoint.Center;
        }

        #endregion

        // AnimateClick
        public static void AnimateClick()
        {
            scaleTween.Start(TweenStyle.QuadraticIn, ScaleInfo.UIElement.Small, ScaleInfo.UIElement.Medium, 150);
            cursorSprite.Tweens.ScaleTween = scaleTween;
        }

        // AnimateSwitch
        public static void AnimateSwitch()
        {
            scaleTween.Start(TweenStyle.QuadraticIn, new(.2f), ScaleInfo.UIElement.Medium, 100);
            cursorSprite.Tweens.ScaleTween = scaleTween;
        }

        // CustomImageTag
        public static object? CustomImageTag { get; private set; }

        // Draw
        public static void Draw(GameTime gameTime)
        {
            var sprite = GetActiveCursor();
            OutlineEffect? effect = Highlight ? ScaryCastleGame.Effects.Outline : null;

            if (effect != null && sprite.Image?.Atlas != null)
            {
                effect.Color.SetValue(ColorPalette.MouseCursorOutline);
                effect.TextureSize.SetValue(new Vector2(sprite.Image.Atlas.Texture.Width, sprite.Image.Atlas.Texture.Height));
                effect.Thickness.SetValue(1.2f);
            }

            EngendroGame.Instance.SpriteBatch.Begin(EngendroGame.Instance.Camera, SamplerState.PointClamp, effect?.Effect);
            sprite.X += shakeTween.IsRunning ? shakeTween.CurrentValue : 0;
            sprite.Draw(gameTime);
            sprite.X -= shakeTween.IsRunning ? shakeTween.CurrentValue : 0;
            EngendroGame.Instance.SpriteBatch.End();
        }

        // Highlight
        public static bool Highlight { get; set; }

        // Reset
        public static void Reset()
        {
            Highlight = false;
            customCursorSprite.Image = null;
            CustomImageTag = null;
        }

        // Shake
        public static void Shake()
        {
            shakeTween.Start(TweenStyle.CubicInOut, 0, 1, 50, 4);
        }

        // SetCustomImage
        public static void SetCustomImage(AtlasImage image, object? tag)
        {
            customCursorSprite.Image = image;
            CustomImageTag = tag;
        }

        // State
        public static MouseCursorState State
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

        // Update
        public static void Update(GameTime gameTime)
        {
            GetActiveCursor().Position = InputManager.DefaultPlayer.Mouse.VirtualPosition;
            GetActiveCursor().Update(gameTime);
            shakeTween.Update(gameTime);
        }
    }
}
