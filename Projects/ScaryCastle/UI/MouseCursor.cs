using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Globalization;

namespace ScaryCastle
{
    /// <summary>
    /// MouseCursor
    /// </summary>
    public static class MouseCursor
    {
        #region Private fields

        private static readonly FloatTween crossOpacityTween = FloatTween.Create(TweenStyle.CubicInOut, 1, .7f, 500, -1);
        private static readonly ColorTween customImageColorTween = ColorTween.Create(TweenStyle.CubicInOut, Color.White, new(210, 210, 210), 500, -1);
        private static readonly AtlasImage?[] cursorImages;
        private static readonly Sprite cursorSprite;
        private static readonly Vector2 defaultScale = ScaleInfo.UIElement.Large;
        private static readonly TextSprite healthTextSprite;
        private static readonly Sprite heartIcon = new(Atlases.UI.HeartIcon) { PivotOrigin = RectanglePoint.LeftTop, Scale = ScaleInfo.UIElement.Small };
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
                Scale = ScaleInfo.UISentence
            };

            // Health sprite
            healthTextSprite = new(Fonts.CommonOutline)
            {
                PivotOrigin = RectanglePoint.Left,
                Scale = ScaleInfo.Text.Medium
            };

            Reset();
        }

        #endregion

        #region Private members

        // ClampToScreen
        private static void ClampToScreen()
        {
            if (textSprite.IsEmpty)
                return;

            var offset = CustomImage == null ? new Vector2(3, 7) : new Vector2(-2, 1);

            textSprite.PivotOrigin = RectanglePoint.Left;
            textSprite.Position = cursorSprite.BoundingBox.GetPoint(RectanglePoint.Right, -offset.X, offset.Y);

            if (HealthAmount > 0)
            {
                heartIcon.PivotOrigin = RectanglePoint.LeftTop;
                heartIcon.Position = textSprite.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 0, -.5f);
                healthTextSprite.PivotOrigin = textSprite.PivotOrigin;
                healthTextSprite.Position = heartIcon.BoundingBox.GetPoint(RectanglePoint.Right, .5f, .5f);
            }

            if (!textSprite.BoundingBox.IsInside(EngendroGame.Instance.Camera.VisibleBox))
            {
                textSprite.PivotOrigin = RectanglePoint.Right;
                textSprite.Position = cursorSprite.BoundingBox.GetPoint(RectanglePoint.Left, offset.X, offset.Y);

                if (HealthAmount > 0)
                {
                    heartIcon.PivotOrigin = RectanglePoint.RightTop;
                    heartIcon.Position = textSprite.BoundingBox.GetPoint(RectanglePoint.RightBottom, -1, 0);

                    healthTextSprite.PivotOrigin = textSprite.PivotOrigin;
                    healthTextSprite.Position = heartIcon.BoundingBox.GetPoint(RectanglePoint.Right, -.5f, 0);
                }
            }

            if (textSprite.BoundingBox.Bottom >= Screen.NativeHeight)
            {
                textSprite.Y -= 10;
                if (HealthAmount > 0)
                {
                    heartIcon.Y -= 10;
                    healthTextSprite.Y -= 10;
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
            if (cursorSprite.Position.X < 0 || cursorSprite.Position.Y < 0)
                return;

            EngendroGame.Instance.SpriteBatch.Begin(EngendroGame.Instance.Camera, SamplerState.PointClamp);
            cursorSprite.X += shakeTween.IsRunning ? shakeTween.CurrentValue : 0;
            cursorSprite.Draw(gameTime);
            cursorSprite.X -= shakeTween.IsRunning ? shakeTween.CurrentValue : 0;
            EngendroGame.Instance.SpriteBatch.End();

            if (State == MouseCursorState.Cross || CustomImage != null)
            {
                EngendroGame.Instance.SpriteBatch.Begin(EngendroGame.Instance.Camera, SamplerState.LinearClamp);
                textSprite.Draw(gameTime);
                if (HealthAmount > 0)
                    healthTextSprite.Draw(gameTime);
                EngendroGame.Instance.SpriteBatch.End();

                if (HealthAmount > 0)
                {
                    EngendroGame.Instance.SpriteBatch.Begin(EngendroGame.Instance.Camera, SamplerState.LinearClamp);
                    heartIcon.Draw(gameTime);
                    EngendroGame.Instance.SpriteBatch.End();
                }
            }
        }

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
                }
            }
        } = true;

        // FlipCustomImage
        public static bool FlipCustomImage { get; set; }

        // HealthAmount
        public static int HealthAmount
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    healthTextSprite.Text = field <= 0 ? null : field.ToString(CultureInfo.InvariantCulture);
                }
            }
        }

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
            healthTextSprite.Color = ColorPalette.Text.Highlight;
            CustomImage = null;
            FlipCustomImage = false;
            HealthAmount = 0;
            IsEnabled = true;
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
            crossOpacityTween.Update(gameTime);
            customImageColorTween.Update(gameTime);
            cursorSprite.Position = InputManager.DefaultPlayer.Mouse.VirtualPosition;
            cursorSprite.Effects = FlipCustomImage && CustomImage != null ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            cursorSprite.Update(gameTime);
            shakeTween.Update(gameTime);
            ClampToScreen();

            if (IsEnabled)
            {
                if (CustomImage != null)
                {
                    cursorSprite.Color = customImageColorTween.CurrentValue;
                    cursorSprite.Opacity = 1;
                }
                else if (State == MouseCursorState.Cross)
                {
                    cursorSprite.Opacity = crossOpacityTween.CurrentValue;
                }
                else
                {
                    cursorSprite.Color = Color.White;
                    cursorSprite.Opacity = 1;
                }
            }
        }
    }
}