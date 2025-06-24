using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione
{
    /// <summary>
    /// UITextButton
    /// </summary>
    public sealed class UITextButton : UIControl
    {
        #region Private fields

        private RectangleF boundingBox;
        private readonly ImageSprite containerPattern;
        private readonly ImageSprite containerEdgeLeft;
        private const float horzImagePadding = 1.5f;
        private string? imageName;
        private readonly ImageSprite image;
        private readonly TextSprite label;
        private RectanglePoint pivotOrigin;

        #endregion

        #region Constructors

        // Constructor
        public UITextButton(EngendroGame game, InputBinding? inputBinding = null)
            : base(game, inputBinding)
        {
            // Container
            this.containerPattern = new ImageSprite(Game)
            {
                PivotOrigin = RectanglePoint.Right,
                Scale = ScaleInfo.UIElement.Medium
            };

            // ContainerEdgeLeft
            this.containerEdgeLeft = new ImageSprite(Game)
            {
                PivotOrigin = RectanglePoint.Right,
                Scale = ScaleInfo.UIElement.Medium
            };

            // Label
            this.label = new TextSprite(game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                //ShadowOffset = new(.5f)
            };

            this.image = new ImageSprite(game);
            this.label.Text = inputBinding == null ? string.Empty : Localization.GetValue(inputBinding);

            Invalidate();
        }

        #endregion

        #region Private members

        // Invalidate
        protected override void Invalidate()
        {
            // Image
            image.Image = GetInputBindingImage(ImageName, InputBinding);
            image.Scale = Small ? ScaleInfo.UIElement.Tiny : ScaleInfo.UIElement.Medium;

            image.PivotOrigin = pivotOrigin;
            label.Scale = ScaleInfo.Text.Large;

            containerPattern.Image = Small ? Atlases.UI.UIControlContainerPatternSmall : Atlases.UI.UIControlContainerPatternLarge;
            containerEdgeLeft.Image = Small ? Atlases.UI.UIControlContainerEdgeSmall : Atlases.UI.UIControlContainerEdgeLarge;

            if (!image.IsEmpty)
            {
                if (PivotOrigin == RectanglePoint.Right || PivotOrigin == RectanglePoint.RightBottom || PivotOrigin == RectanglePoint.RightTop)
                {
                    image.Position = Position;
                    label.PivotOrigin = RectanglePoint.Right;
                    label.Position = image.BoundingBox.GetPoint(RectanglePoint.Left, -horzImagePadding, Small ? .4f : .8f);
                }
                else
                {
                    label.Position = Position;
                    label.PivotOrigin = pivotOrigin;
                    label.X += image.BoundingBox.Width / 2;
                    image.PivotOrigin = RectanglePoint.Right;
                    image.Position = label.BoundingBox.GetPoint(RectanglePoint.Left, -horzImagePadding, Small ? .4f : -.8f);
                }
            }

            containerPattern.ScaleX = label.BoundingBox.Width + 7;
            containerPattern.Y = ImageBoundingBox.GetPoint(RectanglePoint.Middle, 0, -.1f).Y;
            containerEdgeLeft.Y = containerPattern.Y;

            if (PivotOrigin == RectanglePoint.Right || PivotOrigin == RectanglePoint.RightBottom || PivotOrigin == RectanglePoint.RightTop)
            {
                containerEdgeLeft.Effects = SpriteEffects.None;
                containerEdgeLeft.PivotOrigin = RectanglePoint.Right;
                containerPattern.PivotOrigin = RectanglePoint.Right;
                containerPattern.X = ImageBoundingBox.GetPoint(RectanglePoint.Left).X + 5;
                containerEdgeLeft.X = containerPattern.BoundingBox.GetPoint(RectanglePoint.Left).X;
            }
            else
            {
                containerEdgeLeft.Effects = SpriteEffects.FlipHorizontally;
                containerEdgeLeft.PivotOrigin = RectanglePoint.Left;
                containerPattern.PivotOrigin = RectanglePoint.Left;
                containerPattern.X = ImageBoundingBox.GetPoint(RectanglePoint.Right).X - 5;
                containerEdgeLeft.X = containerPattern.BoundingBox.GetPoint(RectanglePoint.Right).X;
            }

            InvalidateBoundingBox();

            label.OpacityFactor = IsEnabled ? 1 : .3f;
        }

        // InvalidateBoundingBox
        private void InvalidateBoundingBox()
        {
            var labelBBox = label.BoundingBox;

            if (image.IsEmpty && labelBBox.IsEmpty)
            {
                boundingBox = RectangleF.Empty;
            }
            else if (image.IsEmpty && !labelBBox.IsEmpty)
            {
                boundingBox = label.BoundingBox;
            }
            else if (!image.IsEmpty && labelBBox.IsEmpty)
            {
                boundingBox = image.BoundingBox;
            }
            else
            {
                boundingBox = RectangleF.Union(image.BoundingBox, containerPattern.BoundingBox, containerEdgeLeft.BoundingBox);
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (BoundingBox.IsEmpty)
                return;

            if (!image.IsEmpty)
            {
                Effect? shader = null;

                if (IsMouseOver)
                {
                    RemizioneGame.Effects.ColorSaturation.SetColor(.7f, .7f, .7f, 1);
                    shader = RemizioneGame.Effects.ColorSaturation.Effect;
                }

                Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp, shader);

                containerPattern.Draw(gameTime);
                containerEdgeLeft.Draw(gameTime);

                image.Draw(gameTime);

                Game.SpriteBatch.End();
            }

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            label.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            image.Update(gameTime);
            label.Update(gameTime);

            if (IsMouseOver)
                label.Color = ColorPalette.Text.Hover;
        }

        #endregion

        // AllowContainer
        public bool AllowContainer { get; set; } = true;

        // BoundingBox
        public override RectangleF BoundingBox => boundingBox;

        // HasText
        public bool HasText => !label.IsEmpty;

        // ImageBoundingBox
        public RectangleF ImageBoundingBox => image.BoundingBox;

        // ImageName
        public string? ImageName
        {
            get => imageName;
            set
            {
                if (value != imageName)
                {
                    imageName = value;
                    Invalidate();
                }
            }
        }

        // PivotOrigin
        public RectanglePoint PivotOrigin
        {
            get => pivotOrigin;
            set
            {
                if (value != pivotOrigin)
                {
                    pivotOrigin = value;
                    Invalidate();
                }
            }
        }

        // Text
        public string? Text
        {
            get => label.Text;
            set
            {
                if (value != label.Text)
                {
                    label.Text = value;
                    Invalidate();
                }
            }
        }

        // TextColor
        public Color TextColor
        {
            get => label.Color;
            set
            {
                if (value != label.Color)
                    label.Color = value;
            }
        }
    }
}
