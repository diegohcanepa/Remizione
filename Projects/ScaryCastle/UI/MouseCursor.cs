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
        private static readonly TextSprite textSprite;

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

            textSprite = new(EngendroGame.Instance, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.OrangeLight,
                PivotOrigin = RectanglePoint.LeftTop,
                Scale = ScaleInfo.UISentence
            };
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

        // BoundingBox
        public static RectangleF BoundingBox => GetActiveCursor().BoundingBox;

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

            EngendroGame.Instance.SpriteBatch.Begin(EngendroGame.Instance.Camera);
            if (State == MouseCursorState.CrossOn)
                textSprite.Draw(gameTime);
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

        // Text
        public static string? Text
        {
            get => textSprite.Text;
            set => textSprite.Text = value;
        }

        // Update
        public static void Update(GameTime gameTime)
        {
            GetActiveCursor().Position = InputManager.DefaultPlayer.Mouse.VirtualPosition;
            GetActiveCursor().Update(gameTime);

            if (!textSprite.IsEmpty && State == MouseCursorState.CrossOn)
            {
                textSprite.PivotOrigin = RectanglePoint.LeftTop;
                textSprite.Position = BoundingBox.GetPoint(RectanglePoint.RightBottom, -2, -2);

                if (!textSprite.BoundingBox.IsInside(EngendroGame.Instance.Camera.VisibleBox))
                {
                    textSprite.PivotOrigin = RectanglePoint.RightTop;
                    textSprite.Position = BoundingBox.GetPoint(RectanglePoint.LeftBottom, 2, -2);
                }

                if (textSprite.BoundingBox.Bottom >= Screen.NativeHeight)
                    textSprite.Y -= 10;
            }

            shakeTween.Update(gameTime);
        }
    }
}
