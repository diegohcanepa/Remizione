using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Remizione
{
    /// <summary>
    /// UIImageButton
    /// </summary>
    public sealed class UIImageButton : UIButton
    {
        private readonly Sprite hoverImage = new();
        private readonly Sprite image = new();

        #region Constructor

        // Constructor
        public UIImageButton(AtlasImage image, AtlasImage? hoverImage = null)
        {
            this.Image = image;
            this.HoverImage = hoverImage;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (IsMouseOver && hoverImage != null)
                hoverImage.Draw(gameTime);
            else
                image.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            image.Update(gameTime);
            hoverImage?.Update(gameTime);
        }

        #endregion

        // BoundingBox
        public override RectangleF BoundingBox => image.BoundingBox;

        // CreateBackButton
        public static UIImageButton CreateBackButton()
        {
            return new(Atlases.UI.BackButton, Atlases.UI.BackButtonHot)
            {
                Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftTop)
            };
        }

        // CreateCloseButton
        public static UIImageButton CreateCloseButton()
        {
            return new(Atlases.UI.CloseButton, Atlases.UI.CloseButtonHot)
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftTop)
            };
        }

        // HoverImage
        public AtlasImage? HoverImage
        {
            get => hoverImage.RenderImage;
            set => hoverImage.RenderImage = value;
        }

        // Image
        public AtlasImage Image
        {
            get => image.RenderImage!;
            set => image.RenderImage = value;
        }

        // PivotOrigin
        public override RectanglePoint PivotOrigin
        {
            get => image.PivotOrigin;
            set
            {
                if (value != image.PivotOrigin)
                {
                    image.PivotOrigin = value;
                    hoverImage.PivotOrigin = value;
                }
            }
        }

        // Position
        public override Vector2 Position
        {
            get => image.Position;
            set
            {
                if (value != image.Position)
                {
                    image.Position = value;
                    hoverImage.Position = value;
                }
            }
        }

        // Scale
        public override Vector2 Scale
        {
            get => image.Scale;
            set
            {
                if (value != image.Scale)
                {
                    image.Scale = value;
                    hoverImage.Scale = value;
                }
            }
        }

        // X
        public override float X
        {
            get => image.X;
            set
            {
                if (value != image.X)
                {
                    image.X = value;
                    hoverImage.X = value;
                }
            }
        }

        // Y
        public override float Y
        {
            get => image.Y;
            set
            {
                if (value != image.Y)
                {
                    image.Y = value;
                    hoverImage.Y = value;
                }
            }
        }
    }
}
