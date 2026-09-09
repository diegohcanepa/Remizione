using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remizione.Effects;
using System;

namespace Remizione
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
        private static readonly TextSprite subTextSprite = new(Fonts.CommonOutline) { Color = ColorPalette.Text.Terra, Scale = ScaleInfo.Text.Medium };
        private static readonly TextSprite tooltipSprite;

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
            var names = Enum.GetNames<MouseCursorIcon>();

            cursorImages = new AtlasImage[names.Length];
            for (var i = 0; i < cursorImages.Length; i++)
            {
                var imageName = $"{prefix}{names[i]}Icon";
                cursorImages[i] = Atlases.UI.GetImage(imageName);
            }

            // Text sprite
            tooltipSprite = new(Fonts.CommonOutline)
            {
                PivotOrigin = RectanglePoint.LeftTop,
                Multiline = true,
                Scale = ScaleInfo.Text.VeryLarge
            };


            Reset();

            InvalidateCursorImage();
        }

        #endregion

        #region Private members

        // CanShowText
        private static bool CanShowText
        {
            get
            {
                if (CustomImage != null)
                {
                    return true;
                }
                else if (Icon is MouseCursorIcon.Up or MouseCursorIcon.Down or
                            MouseCursorIcon.Right or MouseCursorIcon.Left or
                            MouseCursorIcon.Cross or MouseCursorIcon.Wait)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        // ClampTextToScreen
        private static void ClampTextToScreen()
        {
            if (tooltipSprite.IsEmpty)
                return;

            var offset = CustomImage == null ? new Vector2(-1, 1) : new Vector2(-2, 1);

            tooltipSprite.PivotOrigin = RectanglePoint.Left;
            tooltipSprite.Position = cursorSprite.BoundingBox.GetPoint(RectanglePoint.Right, -offset.X, offset.Y);

            if (!subTextSprite.IsEmpty)
            {
                subTextSprite.PivotOrigin = RectanglePoint.LeftTop;
                subTextSprite.Position = tooltipSprite.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 0, -1);
            }

            if (!tooltipSprite.BoundingBox.IsInside(EngendroGame.Instance.Camera.VisibleBox) ||
                (!subTextSprite.IsEmpty && !subTextSprite.BoundingBox.IsInside(EngendroGame.Instance.Camera.VisibleBox)))
            {
                tooltipSprite.PivotOrigin = RectanglePoint.Right;
                tooltipSprite.Position = cursorSprite.BoundingBox.GetPoint(RectanglePoint.Left, offset.X, offset.Y);

                if (!subTextSprite.IsEmpty)
                {
                    subTextSprite.PivotOrigin = RectanglePoint.RightTop;
                    subTextSprite.Position = tooltipSprite.BoundingBox.GetPoint(RectanglePoint.RightBottom, 0, -1);
                }
            }
        }

        // InvalidateCursorImage
        private static void InvalidateCursorImage()
        {
            cursorSprite.RenderImage = CustomImage ?? cursorImages[(int)Icon];
            cursorSprite.Scale = defaultScale;

            if (Icon == MouseCursorIcon.Hand && CustomImage == null)
            {
                cursorSprite.PivotOrigin = RectanglePoint.Top;
            }
            else
            {
                cursorSprite.PivotOrigin = RectanglePoint.Center;
            }
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
                        Icon = MouseCursorIcon.Cross;

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
            EngendroGame.Instance.SpriteBatch.End();

            if (CanShowText)
            {
                EngendroGame.Instance.SpriteBatch.Begin(EngendroGame.Instance.Camera, SamplerState.PointClamp);
                tooltipSprite.Draw(gameTime);
                subTextSprite.Draw(gameTime);
                EngendroGame.Instance.SpriteBatch.End();
            }
        }

        // HightlightColor
        public static Vector4? HightlightColor { get; set; }

        // Icon
        public static MouseCursorIcon Icon
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

        // PerformClick
        public static void PerformClick(bool animate = true)
        {
            if (animate)
            {
                scaleTween.Start(TweenStyle.QuadraticIn, cursorSprite.Scale * .9f, cursorSprite.Scale, 150);
                cursorSprite.Tweens.ScaleTween = scaleTween;
            }
        }

        // Reset
        public static void Reset()
        {
            tooltipSprite.Text = null;
            subTextSprite.Text = null;
            cursorSprite.Color = Color.White;
            cursorSprite.Scale = defaultScale;
            CustomImage = null;
            HightlightColor = null;
            tooltipSprite.Color = ColorPalette.MouseCursor.Tooltip;
            subTextSprite.Color = ColorPalette.MouseCursor.SubText;
            Icon = MouseCursorIcon.Cross;
        }

        // Shake
        public static void Shake()
        {
            shakeTween.Start(TweenStyle.CubicInOut, 0, 1, 50, 4);
            Sound.Play(SoundNames.Error);
        }

        // SubText
        public static string? SubText
        {
            get => subTextSprite.Text;
            set => subTextSprite.Text = value;
        }

        // SubTextColor
        public static Color SubTextColor
        {
            get => subTextSprite.Color;
            set => subTextSprite.Color = value;
        }

        // Tooltip
        public static string? Tooltip
        {
            get => tooltipSprite.Text;
            set => tooltipSprite.Text = value;
        }

        // TooltipColor
        public static Color TooltipColor
        {
            get => tooltipSprite.Color;
            set => tooltipSprite.Color = value;
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

            effect = CustomImage != null && HightlightColor.HasValue ? RemizioneGame.Effects.Outline : null;

            if (effect != null && HightlightColor.HasValue && cursorSprite.RenderImage?.Atlas != null)
            {
                effect.Color.SetValue(HightlightColor.Value);
                effect.TextureSize.SetValue(new Vector2(cursorSprite.RenderImage.Atlas.Texture.Width, cursorSprite.RenderImage.Atlas.Texture.Height));
                effect.Thickness.SetValue(.5f);
            }
        }
    }
}