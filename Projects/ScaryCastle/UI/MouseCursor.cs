using Engendro;
using Engendro.Audio;
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

        private static readonly AtlasImage?[] cursorImages;
        private static readonly ImageSprite cursorSprite;
        private static readonly Vector2 defaultScale = ScaleInfo.UIElement.Large;
        private static readonly FloatTween opacityTween = FloatTween.Create(TweenStyle.CubicInOut, 1, .5f, 700, -1);
        private static readonly Vector2Tween scaleTween = new();
        private static readonly FloatTween shakeTween = new();
        private static readonly TextSprite textSprite;

        #endregion

        #region Constructor

        // Constructor
        static MouseCursor()
        {
            // Cursor sprite
            cursorSprite = new ImageSprite(EngendroGame.Instance)
            {
                PivotOrigin = RectanglePoint.Center,
                Scale = defaultScale
            };

            const string prefix = "MouseCursor";
            var names = Enum.GetNames<MouseCursorState>();

            cursorImages = new AtlasImage[names.Length];
            for (var i = 0; i < cursorImages.Length; i++)
            {
                var imageName = $"{prefix}{names[i]}";
                cursorImages[i] = Atlases.UI.FindImage(imageName);
            }

            // Text sprite
            textSprite = new(EngendroGame.Instance, Fonts.CommonOutline)
            {
                PivotOrigin = RectanglePoint.LeftTop,
                Scale = ScaleInfo.UISentence
            };

            Reset();
        }

        #endregion

        #region Private members

        // ClampTextToScreen
        private static void ClampTextToScreen()
        {
            if (textSprite.IsEmpty)
                return;

            var offset = CustomImage == null ? 4 : 2;

            textSprite.PivotOrigin = RectanglePoint.LeftTop;
            textSprite.Position = cursorSprite.BoundingBox.GetPoint(RectanglePoint.RightBottom, -offset, -offset);

            if (!textSprite.BoundingBox.IsInside(EngendroGame.Instance.Camera.VisibleBox))
            {
                textSprite.PivotOrigin = RectanglePoint.RightTop;
                textSprite.Position = cursorSprite.BoundingBox.GetPoint(RectanglePoint.LeftBottom, offset, -offset);
            }

            if (textSprite.BoundingBox.Bottom >= Screen.NativeHeight)
                textSprite.Y -= 10;
        }

        // InvalidateCursorImage
        private static void InvalidateCursorImage()
        {
            cursorSprite.Image = CustomImage ?? cursorImages[(int)State];
            cursorSprite.Scale = CustomImage != null ? ScaleInfo.UIElement.Large : defaultScale;
            cursorSprite.PivotOrigin = (State is MouseCursorState.Arrow or MouseCursorState.Hand) && CustomImage == null ? RectanglePoint.LeftTop : RectanglePoint.Center;
        }

        #endregion

        // AnimateClick
        public static void AnimateClick()
        {
            scaleTween.Start(TweenStyle.QuadraticIn, cursorSprite.Scale * .9f, cursorSprite.Scale, 150);
            cursorSprite.Tweens.ScaleTween = scaleTween;
        }

        // CustomImage
        public static AtlasImage? CustomImage
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    if (field != null)
                        State = MouseCursorState.Cross;

                    InvalidateCursorImage();
                }
            }
        }

        // Draw
        public static void Draw(GameTime gameTime)
        {
            OutlineEffect? effect = CustomImage != null && Hightlight ? ScaryCastleGame.Effects.Outline : null;

            if (effect != null && cursorSprite.Image?.Atlas != null)
            {
                effect.Color.SetValue(ColorPalette.MouseCursorHighlight);
                effect.TextureSize.SetValue(new Vector2(cursorSprite.Image.Atlas.Texture.Width, cursorSprite.Image.Atlas.Texture.Height));
                effect.Thickness.SetValue(1);
            }

            EngendroGame.Instance.SpriteBatch.Begin(EngendroGame.Instance.Camera, SamplerState.PointClamp, effect?.Effect);
            cursorSprite.X += shakeTween.IsRunning ? shakeTween.CurrentValue : 0;
            cursorSprite.Draw(gameTime);
            cursorSprite.X -= shakeTween.IsRunning ? shakeTween.CurrentValue : 0;
            EngendroGame.Instance.SpriteBatch.End();

            if (State == MouseCursorState.Cross || State == MouseCursorState.Hit || CustomImage != null)
            {
                EngendroGame.Instance.SpriteBatch.Begin(EngendroGame.Instance.Camera);
                textSprite.Draw(gameTime);
                EngendroGame.Instance.SpriteBatch.End();
            }
        }

        // Hightlight
        public static bool Hightlight { get; set; }

        // IsArrow
        public static bool IsArrow => State is MouseCursorState.Up or MouseCursorState.Down or
                                      MouseCursorState.Right or MouseCursorState.Left;

        // PerformClick
        public static void PerformClick()
        {
            AnimateClick();
            Sound.Play(SoundNames.Interact);
        }

        // Reset
        public static void Reset()
        {
            textSprite.Color = ColorPalette.Text.Sentence;
            CustomImage = null;
            Hightlight = false;
            State = MouseCursorState.Arrow;
            Text = null;
        }

        // Shake
        public static void Shake()
        {
            shakeTween.Start(TweenStyle.CubicInOut, 0, 1, 50, 4);
            Sound.Play(SoundNames.Error);
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
                    InvalidateCursorImage();
                }
            }
        }

        // Text
        public static string? Text
        {
            get => textSprite.Text;
            set => textSprite.Text = value;
        }

        // TextColor
        public static Color TextColor
        {
            get => textSprite.Color;
            set => textSprite.Color = value;
        }

        // Update
        public static void Update(GameTime gameTime)
        {
            opacityTween.Update(gameTime);

            if (State == MouseCursorState.Cross && CustomImage == null)
                cursorSprite.Opacity = opacityTween.CurrentValue;
            else
                cursorSprite.Opacity = 1;

            cursorSprite.Position = InputManager.DefaultPlayer.Mouse.VirtualPosition;
            cursorSprite.Update(gameTime);
            shakeTween.Update(gameTime);
            ClampTextToScreen();
        }
    }
}