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

        private static readonly ColorTween customImageColorTween = ColorTween.Create(TweenStyle.CubicInOut, Color.White, new(210, 210, 210), 500, -1);
        private static readonly AtlasImage?[] cursorImages;
        private static readonly Sprite cursorSprite;
        private static readonly Vector2 defaultScale = ScaleInfo.UIElement.Large;
        private static OutlineEffect? effect;
        private static readonly Vector2Tween scaleTween = new();
        private static readonly FloatTween shakeTween = new();
        private static readonly TextSprite textSprite;

        #endregion

        #region Constructor

        // Constructor
        static MouseCursor()
        {
            // Cursor sprite
            cursorSprite = new Sprite()
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
                cursorImages[i] = Atlases.UI.GetImage(imageName);
            }

            // Text sprite
            textSprite = new(Fonts.CommonOutline)
            {
                PivotOrigin = RectanglePoint.LeftTop,
                Multiline = true,
                Scale = ScaleInfo.Text.Large
            };


            Reset();

            InvalidateCursorImage();
        }

        #endregion

        #region Private members

        // ClampTextToScreen
        private static void ClampTextToScreen()
        {
            if (textSprite.IsEmpty)
                return;

            var offset = CustomImage == null ? new Vector2(1, 7) : new Vector2(-2, 1);

            textSprite.PivotOrigin = RectanglePoint.Left;
            textSprite.Position = cursorSprite.BoundingBox.GetPoint(RectanglePoint.Right, -offset.X, offset.Y);

            if (!textSprite.BoundingBox.IsInside(EngendroGame.Instance.Camera.VisibleBox))
            {
                textSprite.PivotOrigin = RectanglePoint.Right;
                textSprite.Position = cursorSprite.BoundingBox.GetPoint(RectanglePoint.Left, offset.X, offset.Y);
            }

            if (textSprite.BoundingBox.Bottom >= Screen.NativeHeight)
                textSprite.Y -= 10;
        }

        // InvalidateCursorImage
        private static void InvalidateCursorImage()
        {
            cursorSprite.RenderImage = CustomImage ?? cursorImages[(int)State];
            cursorSprite.Scale = CustomImage != null ? ScaleInfo.UIElement.Medium : defaultScale;
            cursorSprite.PivotOrigin = (State is MouseCursorState.Hand) && CustomImage == null ? RectanglePoint.Top : RectanglePoint.Center;
        }

        #endregion

        // Color
        public static Color Color
        {
            get => cursorSprite.Color;
            set => cursorSprite.Color = value;
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
            if (cursorSprite.Position.X < 0 || cursorSprite.Position.Y < 0)
                return;

            EngendroGame.Instance.SpriteBatch.Begin(EngendroGame.Instance.Camera, SamplerState.PointClamp, effect?.Effect);
            cursorSprite.X += shakeTween.IsRunning ? shakeTween.CurrentValue : 0;
            cursorSprite.Draw(gameTime);
            cursorSprite.X -= shakeTween.IsRunning ? shakeTween.CurrentValue : 0;

            if (State != MouseCursorState.Cross && !IsArrow)
                textSprite.Draw(gameTime);

            EngendroGame.Instance.SpriteBatch.End();
        }

        // HightlightColor
        public static Vector4? HightlightColor { get; set; }

        // IsArrow
        public static bool IsArrow => State is MouseCursorState.Up or MouseCursorState.Down or
                                      MouseCursorState.Right or MouseCursorState.Left;

        // PerformClick
        public static void PerformClick(bool animate = true)
        {
            if (animate)
            {
                scaleTween.Start(TweenStyle.QuadraticIn, cursorSprite.Scale * .9f, cursorSprite.Scale, 150);
                cursorSprite.Tweens.ScaleTween = scaleTween;
            }

            Sound.Play(SoundNames.Interact);
        }

        // Reset
        public static void Reset()
        {
            cursorSprite.Color = Color.White;
            cursorSprite.Scale = defaultScale;
            CustomImage = null;
            HightlightColor = null;
            textSprite.Color = ColorPalette.Text.MouseCursor;
            State = MouseCursorState.Cross;
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
            customImageColorTween.Update(gameTime);
            cursorSprite.Position = InputManager.DefaultPlayer.Mouse.VirtualPosition;
            cursorSprite.Update(gameTime);
            shakeTween.Update(gameTime);

            ClampTextToScreen();

            if (CustomImage != null)
            {
                cursorSprite.Color = customImageColorTween.CurrentValue;
                cursorSprite.Opacity = 1;
            }
            else
            {
                cursorSprite.Color = Color.White;
                cursorSprite.Opacity = 1;
            }

            effect = CustomImage != null && HightlightColor.HasValue ? ScaryCastleGame.Effects.Outline : null;

            if (effect != null && HightlightColor.HasValue && cursorSprite.RenderImage?.Atlas != null)
            {
                effect.Color.SetValue(HightlightColor.Value);
                effect.TextureSize.SetValue(new Vector2(cursorSprite.RenderImage.Atlas.Texture.Width, cursorSprite.RenderImage.Atlas.Texture.Height));
                effect.Thickness.SetValue(1);
            }
        }
    }
}