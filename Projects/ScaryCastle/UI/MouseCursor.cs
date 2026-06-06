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
        private static readonly Sprite cursorSprite;
        private static readonly Vector2 defaultScale = ScaleInfo.UIElement.Large;
        private static OutlineEffect? effect;
        private static readonly FloatTween opacityTween = FloatTween.Create(TweenStyle.CubicInOut, 1, .5f, 500, -1);
        private static readonly Vector2Tween scaleTween = new();
        private static readonly FloatTween shakeTween = new();
        private static readonly TextSprite subTextSprite;
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
                Scale = ScaleInfo.UISentence
            };

            // Text sprite 2
            subTextSprite = new(Fonts.CommonOutline)
            {
                Scale = ScaleInfo.Text.Large
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

            if (!subTextSprite.IsEmpty)
            {
                subTextSprite.PivotOrigin = textSprite.PivotOrigin;
                subTextSprite.Position = textSprite.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 0, -1.5f);
            }

            if (!textSprite.BoundingBox.IsInside(EngendroGame.Instance.Camera.VisibleBox))
            {
                textSprite.PivotOrigin = RectanglePoint.RightTop;
                textSprite.Position = cursorSprite.BoundingBox.GetPoint(RectanglePoint.LeftBottom, offset, -offset);

                if (!subTextSprite.IsEmpty)
                {
                    subTextSprite.PivotOrigin = textSprite.PivotOrigin;
                    subTextSprite.Position = textSprite.BoundingBox.GetPoint(RectanglePoint.RightBottom, 0, -1.5f);
                }
            }

            if (textSprite.BoundingBox.Bottom >= Screen.NativeHeight)
            {
                textSprite.Y -= 10;

                if (!subTextSprite.IsEmpty)
                {
                    subTextSprite.Y -= 7 + textSprite.BoundingBox.Height + subTextSprite.BoundingBox.Height;
                }
            }
        }

        // InvalidateCursorImage
        private static void InvalidateCursorImage()
        {
            cursorSprite.RenderImage = CustomImage ?? cursorImages[(int)State];
            cursorSprite.Scale = CustomImage != null ? ScaleInfo.InventoryHeldItem : defaultScale;
            cursorSprite.PivotOrigin = (State is MouseCursorState.Arrow or MouseCursorState.Hand) && CustomImage == null ? RectanglePoint.LeftTop : RectanglePoint.Center;
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
            EngendroGame.Instance.SpriteBatch.Begin(EngendroGame.Instance.Camera, SamplerState.PointClamp, effect?.Effect);
            cursorSprite.X += shakeTween.IsRunning ? shakeTween.CurrentValue : 0;
            cursorSprite.Draw(gameTime);
            cursorSprite.X -= shakeTween.IsRunning ? shakeTween.CurrentValue : 0;
            EngendroGame.Instance.SpriteBatch.End();

            //if (State == MouseCursorState.Cross || CustomImage != null)
            {
                EngendroGame.Instance.SpriteBatch.Begin(EngendroGame.Instance.Camera);
                textSprite.Draw(gameTime);
                subTextSprite.Draw(gameTime);

                EngendroGame.Instance.SpriteBatch.End();
            }
        }

        // HightlightColor
        public static Vector4? HightlightColor { get; set; }

        // IsArrow
        public static bool IsArrow => State is MouseCursorState.Up or MouseCursorState.Down or
                                      MouseCursorState.Right or MouseCursorState.Left;

        // IsEnabled
        public static bool IsEnabled
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    cursorSprite.Opacity = value ? 1 : .4f;
                    textSprite.Opacity = cursorSprite.Opacity;
                    subTextSprite.Opacity = cursorSprite.Opacity;
                }
            }
        } = true;

        // FlipCustomImage
        public static bool FlipCustomImage { get; set; }

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
            textSprite.Color = ColorPalette.Text.Sentence;
            subTextSprite.Color = ColorPalette.Text.Gold;
            CustomImage = null;
            FlipCustomImage = false;
            HightlightColor = null;
            IsEnabled = true;
            State = MouseCursorState.Arrow;
            Text = null;
            SubText = null;
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

        // SubText
        public static string? SubText
        {
            get => subTextSprite.Text;
            set => subTextSprite.Text = value;
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
            cursorSprite.Position = InputManager.DefaultPlayer.Mouse.VirtualPosition;
            cursorSprite.Effects = FlipCustomImage && CustomImage != null ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            cursorSprite.Update(gameTime);
            shakeTween.Update(gameTime);
            ClampTextToScreen();

            if (IsEnabled)
            {
                effect = CustomImage != null && HightlightColor.HasValue ? ScaryCastleGame.Effects.Outline : null;

                if (effect != null && HightlightColor.HasValue && cursorSprite.RenderImage?.Atlas != null)
                {
                    effect.Color.SetValue(HightlightColor.Value * opacityTween.CurrentValue);
                    effect.TextureSize.SetValue(new Vector2(cursorSprite.RenderImage.Atlas.Texture.Width, cursorSprite.RenderImage.Atlas.Texture.Height));
                    effect.Thickness.SetValue(1);
                }
            }
        }
    }
}