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

        private static readonly ColorTween customImageColorTween = ColorTween.Create(TweenStyle.CubicInOut, Color.White, new(210, 210, 210), 500, -1);
        private static readonly AtlasImage?[] cursorImages;
        private static readonly Sprite cursorSprite;
        private static readonly Vector2 defaultScale = ScaleInfo.UIElement.Large;
        private static readonly Vector2Tween scaleTween = new();
        private static readonly FloatTween shakeTween = new();

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

            Reset();

            InvalidateCursorImage();
        }

        #endregion

        #region Private members

        // InvalidateCursorImage
        private static void InvalidateCursorImage()
        {
            cursorSprite.RenderImage = CustomImage ?? cursorImages[(int)State];
            cursorSprite.Scale = CustomImage != null ? ScaleInfo.InventoryHeldItem : defaultScale;
            cursorSprite.PivotOrigin = (State is MouseCursorState.Hand) && CustomImage == null ? RectanglePoint.LeftTop : RectanglePoint.Center;
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

            /*
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
            */
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
            CustomImage = null;
            FlipCustomImage = false;
            IsEnabled = true;
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

        // Update
        public static void Update(GameTime gameTime)
        {
            customImageColorTween.Update(gameTime);
            cursorSprite.Position = InputManager.DefaultPlayer.Mouse.VirtualPosition;
            cursorSprite.Effects = FlipCustomImage && CustomImage != null ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            cursorSprite.Update(gameTime);
            shakeTween.Update(gameTime);

            if (IsEnabled)
            {
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
            }
        }
    }
}