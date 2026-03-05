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
        private static readonly TextSprite textSprite;

        #endregion

        #region Constructor

        // Constructor
        static MouseCursor()
        {
            // Cursor sprite
            cursorSprite = new Sprite(EngendroGame.Instance)
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
            cursorSprite.RenderImage = CustomImage ?? cursorImages[(int)State];
            cursorSprite.Scale = CustomImage != null ? ScaleInfo.InventoryHeldItem : defaultScale;
            cursorSprite.PivotOrigin = (State is MouseCursorState.Arrow or MouseCursorState.Hand) && CustomImage == null ? RectanglePoint.LeftTop : RectanglePoint.Center;
        }

        #endregion

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

            if (State == MouseCursorState.Cross || CustomImage != null)
            {
                EngendroGame.Instance.SpriteBatch.Begin(EngendroGame.Instance.Camera);
                textSprite.Draw(gameTime);
                EngendroGame.Instance.SpriteBatch.End();
            }
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
            textSprite.Color = ColorPalette.Text.Sentence;
            CustomImage = null;
            HightlightColor = null;
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
            cursorSprite.Position = InputManager.DefaultPlayer.Mouse.VirtualPosition;
            cursorSprite.Update(gameTime);
            shakeTween.Update(gameTime);
            ClampTextToScreen();

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