using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione
{
    /// <summary>
    /// UIControl
    /// </summary>
    public sealed class UIControl : GameObject, IBoundingBox
    {
        #region Private fields

        private bool allowContainer;
        private readonly ImageSprite containerPattern;
        private readonly ImageSprite containerEdge;
        private UIControlDisplayMode displayMode;
        private string? imageName;
        private readonly ImageSprite image;
        private bool isEnabled = true;
        private const float horzImagePadding = 1.5f;
        private InputBinding? inputBinding;
        private readonly TextSprite label;
        private InputMethod lastKnownInputMethod;
        private RectanglePoint pivotOrigin;
        private Vector2 position;
        private bool small;

        #endregion

        #region Constructors

        // Constructor
        public UIControl(EngendroGame game, InputBinding? inputBinding = null)
            : base(game)
        {
            // Container
            this.containerPattern = new ImageSprite(Game)
            {
                PivotOrigin = RectanglePoint.Right,
                Scale = ScaleInfo.UIElement.Medium
            };

            // ContainerEdge
            this.containerEdge = new ImageSprite(Game)
            {
                PivotOrigin = RectanglePoint.Right,
                Scale = ScaleInfo.UIElement.Medium
            };

            // Label
            this.label = new TextSprite(game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default
            };

            this.image = new ImageSprite(game);
            this.inputBinding = inputBinding;
            this.label.Text = inputBinding == null ? string.Empty : Localization.EncodeKey(inputBinding);

            Invalidate();
        }

        #endregion

        #region Private members

        // GetInputBindingImage
        private AtlasImage? GetInputBindingImage()
        {
            if (Atlases.UI is not UIAtlas atlas)
                return null;

            var gamePad = InputManager.DefaultPlayer.LastInputMethod == InputMethod.GamePad;

            string? imageName = null;

            if (!string.IsNullOrWhiteSpace(ImageName))
            {
                imageName = this.ImageName;
            }
            else if (InputBinding != null && gamePad)
            {
                imageName = InputBinding.Button.ToString();
            }

            if (gamePad && imageName != null && InputBinding != null)
                imageName = GamePadDevice.Style.ToString() + imageName;

            return string.IsNullOrWhiteSpace(imageName) ? null : atlas.GetImage(imageName);
        }

        // Invalidate
        private void Invalidate()
        {
            // Image
            image.Image = GetInputBindingImage();
            image.Scale = small ? ScaleInfo.UIElement.Small : ScaleInfo.UIElement.Medium;
            lastKnownInputMethod = InputManager.DefaultPlayer.LastInputMethod;

            if (DisplayMode == UIControlDisplayMode.ImageOnly)
            {
                image.PivotOrigin = pivotOrigin;
                image.Position = position;
                BoundingBox = image.BoundingBox;
            }
            else
            {
                image.PivotOrigin = pivotOrigin;
                label.Scale = small ? ScaleInfo.Text.Medium : ScaleInfo.Text.Large;

                containerPattern.Image = small ? Atlases.UI.UIControlContainerPatternSmall : Atlases.UI.UIControlContainerPatternLarge;
                containerEdge.Image = small ? Atlases.UI.UIControlContainerEdgeSmall : Atlases.UI.UIControlContainerEdgeLarge;

                if (!image.IsEmpty)
                {
                    if (PivotOrigin == RectanglePoint.Right || PivotOrigin == RectanglePoint.RightBottom || PivotOrigin == RectanglePoint.RightTop)
                    {
                        image.Position = position;
                        label.PivotOrigin = RectanglePoint.Right;
                        label.Position = image.BoundingBox.GetPoint(RectanglePoint.Left, -horzImagePadding, small ? .4f : .8f);
                    }
                    else
                    {
                        label.Position = position;
                        label.PivotOrigin = pivotOrigin;
                        label.X += image.BoundingBox.Width / 2;
                        image.PivotOrigin = RectanglePoint.Right;
                        image.Position = label.BoundingBox.GetPoint(RectanglePoint.Left, -horzImagePadding, small ? .4f : -.8f);
                    }
                }

                if (AllowContainer && DisplayMode != UIControlDisplayMode.ImageOnly)
                {
                    containerPattern.ScaleX = TextBoundingBox.Width + 7;
                    containerPattern.Y = ImageBoundingBox.GetPoint(RectanglePoint.Middle, 0, small ? 0 : -.3f).Y;
                    containerEdge.Y = containerPattern.Y;

                    if (PivotOrigin == RectanglePoint.Right || PivotOrigin == RectanglePoint.RightBottom || PivotOrigin == RectanglePoint.RightTop)
                    {
                        containerEdge.Effects = SpriteEffects.None;
                        containerEdge.PivotOrigin = RectanglePoint.Right;
                        containerPattern.PivotOrigin = RectanglePoint.Right;
                        containerPattern.X = ImageBoundingBox.GetPoint(RectanglePoint.Left).X + 5;
                        containerEdge.X = containerPattern.BoundingBox.GetPoint(RectanglePoint.Left).X;
                    }
                    else
                    {
                        containerEdge.Effects = SpriteEffects.FlipHorizontally;
                        containerEdge.PivotOrigin = RectanglePoint.Left;
                        containerPattern.PivotOrigin = RectanglePoint.Left;
                        containerPattern.X = ImageBoundingBox.GetPoint(RectanglePoint.Right).X - 5;
                        containerEdge.X = containerPattern.BoundingBox.GetPoint(RectanglePoint.Right).X;
                    }
                }

                InvalidateBoundingBox();
            }
        }

        // InvalidateBoundingBox
        private void InvalidateBoundingBox()
        {
            var labelBBox = DisplayMode == UIControlDisplayMode.ImageOnly ? RectangleF.Empty : label.BoundingBox;

            if (image.IsEmpty && labelBBox.IsEmpty)
            {
                BoundingBox = RectangleF.Empty;
            }
            else if (image.IsEmpty && !labelBBox.IsEmpty)
            {
                BoundingBox = label.BoundingBox;
            }
            else if (!image.IsEmpty && labelBBox.IsEmpty)
            {
                BoundingBox = image.BoundingBox;
            }
            else if (allowContainer)
            {
                BoundingBox = RectangleF.Union(image.BoundingBox, containerPattern.BoundingBox, containerEdge.BoundingBox);
            }
            else
            {
                RectangleF bbox = RectangleF.Union(image.BoundingBox, labelBBox);
                BoundingBox = new RectangleF(bbox.Left, bbox.Top, bbox.Width + horzImagePadding, bbox.Height);
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

                if (AllowContainer && DisplayMode != UIControlDisplayMode.ImageOnly)
                {
                    containerPattern.Draw(gameTime);
                    containerEdge.Draw(gameTime);
                }

                image.Draw(gameTime);
                Game.SpriteBatch.End();
            }

            if (displayMode == UIControlDisplayMode.ImageAndText)
            {
                Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
                label.Draw(gameTime);
                Game.SpriteBatch.End();
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            image.Update(gameTime);
            label.Update(gameTime);

            if (InputManager.DefaultPlayer.LastInputMethod != lastKnownInputMethod)
                Invalidate();

            IsMouseOver = false;

            if (IsEnabled)
            {
                if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.Mouse)
                {
                    IsMouseOver = BoundingBox.Contains(InputManager.DefaultPlayer.Mouse.VirtualPosition);
                    if (IsMouseOver)
                        label.Color = ColorPalette.Text.Hover;
                }
            }
        }

        #endregion

        // AllowContainer
        public bool AllowContainer
        {
            get => allowContainer;
            set
            {
                if (value != allowContainer)
                {
                    allowContainer = value;
                    label.Font = value ? Fonts.Common : Fonts.CommonOutline;
                    Invalidate();
                }
            }
        }

        // BoundingBox
        public RectangleF BoundingBox { get; private set; }

        // DisplayMode
        public UIControlDisplayMode DisplayMode
        {
            get => displayMode;
            set
            {
                if (value != displayMode)
                {
                    displayMode = value;
                    Invalidate();
                }
            }
        }

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

        // InputBinding
        public InputBinding? InputBinding
        {
            get => inputBinding;
            set
            {
                if (value != inputBinding)
                {
                    inputBinding = value;
                    lastKnownInputMethod = InputMethod.None;
                    Invalidate();
                }
            }
        }

        // IsEnabled
        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                if (value != isEnabled)
                {
                    isEnabled = value;
                    label.OpacityFactor = isEnabled ? 1 : .3f;
                }
            }   
        }

        // IsMouseOver
        public bool IsMouseOver { get; private set; }

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

        // Position
        public Vector2 Position
        {
            get => position;
            set
            {
                if (value != position)
                {
                    position = value;
                    Invalidate();
                }
            }
        }

        // Small
        public bool Small
        {
            get => small;
            set
            {
                if (value != small)
                {
                    small = value;
                    Invalidate();
                }
            }
        }

        // Tag
        public object? Tag { get; set; }

        // TestPressed
        public bool TestPressed(PlayerIndex playerIndex)
        {
            if (!IsEnabled)
                return false;

            var result = false;

            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed() && IsMouseOver)
                result = true;

            if (inputBinding != null && inputBinding.IsPressed(playerIndex))
                result = true;

            if (result)
                Sound.Play(SoundNames.MenuSelect);

            return result;
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

        // TextBoundingBox
        public RectangleF TextBoundingBox => label.BoundingBox;

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

        // TextScale
        public Vector2 TextScale
        {
            get => label.Scale;
            set
            {
                if (value != label.Scale)
                {
                    label.Scale= value;
                    Invalidate();
                }
            }
        }

        // X
        public float X
        {
            get => position.X;
            set
            {
                if (value != position.X)
                {
                    position.X = value;
                    Invalidate();
                }
            }
        }

        // Y
        public float Y
        {
            get => position.Y;
            set
            {
                if (value != position.Y)
                {
                    position.Y = value;
                    Invalidate();
                }
            }
        }
    }
}
