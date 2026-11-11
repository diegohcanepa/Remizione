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

        private static readonly Vector2 clickScale = new(.75f);
        private static readonly Vector2Tween clickScaleTween = new();
        private static readonly ColorTween customImageColorTween = ColorTween.Create(TweenStyle.CubicInOut, Color.White, new(210, 210, 210), 500, -1);
        private static readonly AtlasImage?[] cursorImages;
        private static readonly Sprite cursorSprite;
        private static OutlineEffect? effect;
        private static readonly FloatTween shakeTween = new();
        private static readonly TextSprite subTextSprite = new(Fonts.CommonOutline) { Scale = ScaleInfo.Text.VeryLarge };
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
                Scale = ScaleInfo.UIElement.Large
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
                subTextSprite.Position = tooltipSprite.BoundingBox.GetPoint(RectanglePoint.LeftBottom);
            }

            if (!tooltipSprite.BoundingBox.IsInside(EngendroGame.Instance.Camera.VisibleBox) ||
                (!subTextSprite.IsEmpty && !subTextSprite.BoundingBox.IsInside(EngendroGame.Instance.Camera.VisibleBox)))
            {
                tooltipSprite.PivotOrigin = RectanglePoint.Right;
                tooltipSprite.Position = cursorSprite.BoundingBox.GetPoint(RectanglePoint.Left, offset.X, offset.Y);

                if (!subTextSprite.IsEmpty)
                {
                    subTextSprite.PivotOrigin = RectanglePoint.RightTop;
                    subTextSprite.Position = tooltipSprite.BoundingBox.GetPoint(RectanglePoint.RightBottom);
                }
            }
        }

        // InvalidateCursorImage
        private static void InvalidateCursorImage()
        {
            cursorSprite.RenderImage = CustomImage ?? cursorImages[(int)Icon];
            clickScaleTween.Stop();

            if (Icon == MouseCursorIcon.Hand && CustomImage == null)
            {
                cursorSprite.PivotOrigin = RectanglePoint.Top;
            }
            else if (Icon == MouseCursorIcon.Sack && CustomImage == null)
            {
                cursorSprite.PivotOrigin = RectanglePoint.LeftTop;
            }
            else
            {
                cursorSprite.PivotOrigin = RectanglePoint.Center;
            }
        }

        #endregion

        // Click
        public static void Click(bool silent = false)
        {
            clickScaleTween.Start(TweenStyle.Linear, Vector2.One, clickScale, 60, 2);
            
            if (!silent)
                Sound.Play(SoundNames.Interact);
        }

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
            if (!IsVisible || cursorSprite.Position.X < 0 || cursorSprite.Position.Y < 0)
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

        // InvalidClick
        public static void InvalidClick(bool silent = false)
        {
            shakeTween.Start(TweenStyle.CubicInOut, 0, 1, 50, 4);
            
            if (!silent)
                Sound.Play(SoundNames.Error);
        }

        // IsVisible
        public static bool IsVisible { get; set; } = true;

        // Reset
        public static void Reset()
        {
            tooltipSprite.Text = null;
            subTextSprite.Text = null;
            cursorSprite.Color = Color.White;
            CustomImage = null;
            HightlightColor = null;
            tooltipSprite.Color = Colors.MouseCursor.Tooltip;
            subTextSprite.Color = Colors.MouseCursor.SubText;
            Icon = MouseCursorIcon.Cross;
            clickScaleTween.Stop();
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
            clickScaleTween.Update(gameTime);
            cursorSprite.ScaleFactor = clickScaleTween.IsRunning ? clickScaleTween.CurrentValue : Vector2.One;
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
                effect.Thickness.SetValue(.75f);
            }
        }
    }
}