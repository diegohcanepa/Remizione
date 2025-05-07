
using Engendro;
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
        private bool glow;
        private string? imageName;
        private readonly Vector2 imagePadding = new(2, .5f);
        private readonly ImageSprite image;
        private readonly ImageSprite imageShadow;
        private ControlImageSource imageSource;
        private InputBinding? inputBinding;
        private readonly TextSprite label;
        private InputMethod lastKnownInputMethod;
        private RectanglePoint pivotOrigin;
        private Vector2 position;
        private bool shadow;
        private UIControlSize size;

        #endregion

        #region Constructors

        // Constructor
        public UIControl(EngendroGame game, InputBinding? inputBinding)
            : this(game, inputBinding, null)
        {
        }

        // Constructor
        public UIControl(EngendroGame game, InputBinding? inputBinding, string? text, bool outlineFont = false)
            : base(game)
        {
            // Label
            this.label = new TextSprite(game, outlineFont ? Fonts.MainOutline : Fonts.Main)
            {
                Color = ColorPalette.TextDepracated.Dark,
                ShadowColor = ColorPalette.UIControlShadow,
                Text = text
            };

            // Container
            this.containerPattern = new ImageSprite(Game)
            {
                PivotOrigin = RectanglePoint.Right,
                Scale = new Vector2(.75f)
            };

            // ContainerEdge
            this.containerEdge = new ImageSprite(Game)
            {
                PivotOrigin = RectanglePoint.Right,
                Scale = new Vector2(.75f)
            };

            this.image = new ImageSprite(game);
            this.imageShadow = new ImageSprite(game) { Color = ColorPalette.UIControlShadow };
            this.inputBinding = inputBinding;
            this.label.Text = inputBinding == null ? string.Empty : LocalizationHelper.GetInputBinding(inputBinding);

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

            if (ImageSource == ControlImageSource.ImageName)
            {
                imageName = this.ImageName;
            }
            else if (InputBinding != null)
            {
                if (ImageSource == ControlImageSource.InputBindingName)
                {
                    imageName = InputBinding.Name;
                }
                else if (!InputBinding.IsEmpty)
                {
                    if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.GamePad)
                    {
                        imageName = InputBinding.Button.ToString();
                    }
                    else if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.Keyboard)
                    {
                        if (InputBinding.Keys.Length > 0)
                            imageName = InputBinding.Keys[0].ToString();
                    }
                    else if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.Mouse)
                    {
                        if (InputBinding.MouseButton != MouseButton.None)
                            imageName = InputBinding.MouseButton.ToString();
                    }
                }
            }

            if (imageName != null && InputBinding != null)
                imageName = gamePad ? GamePadDevice.Style.ToString() + imageName : "Keyboard" + imageName;

            return string.IsNullOrWhiteSpace(imageName) ? atlas.MissingInputBinding : atlas.GetImage(imageName);
        }

        // Invalidate
        private void Invalidate()
        {
            // Image
            image.Image = GetInputBindingImage();
            imageShadow.Image = image.Image;
            lastKnownInputMethod = InputManager.DefaultPlayer.LastInputMethod;

            // Update bounding box
            InvalidateBoundingBox();
            if (BoundingBox.IsEmpty)
                return;

            // Image scale & position
            image.PivotOrigin = pivotOrigin;
            image.Position = position;

            image.Scale = Size == UIControlSize.Small ? new Vector2(.55f) : new Vector2(.75f);
            label.Scale = Size == UIControlSize.Small ? ScaleInfo.Text.Large : ScaleInfo.Text.VeryLarge;

            /*
            containerPattern.Image = Size == UIControlSize.Small ? Atlases.UI.UIControlContainerPatternSmall : Atlases.UI.UIControlContainerPatternLarge;
            containerEdge.Image = Size == UIControlSize.Small ? Atlases.UI.UIControlContainerEdgeSmall : Atlases.UI.UIControlContainerEdgeLarge;
            */

            if (displayMode == UIControlDisplayMode.ImageOnly)
            {
                image.Position = Position;
            }
            else
            {
                if (PivotOrigin == RectanglePoint.Right || PivotOrigin == RectanglePoint.RightBottom || PivotOrigin == RectanglePoint.RightTop)
                {
                    label.PivotOrigin = RectanglePoint.Right;
                    label.Position = image.IsEmpty ? position : image.BoundingBox.GetPoint(RectanglePoint.Left, -imagePadding.X, imagePadding.Y);
                }
                else
                {
                    label.PivotOrigin = RectanglePoint.Left;
                    label.Position = image.IsEmpty ? position : image.BoundingBox.GetPoint(RectanglePoint.Right, imagePadding.X, imagePadding.Y);

                    if (PivotOrigin == RectanglePoint.Bottom || PivotOrigin == RectanglePoint.Top || PivotOrigin == RectanglePoint.Middle)
                    {
                        var halfWidth = image.BoundingBox.Width - imagePadding.X + label.BoundingBox.Width / 2;

                        image.X -= halfWidth;
                        label.X -= halfWidth;
                    }
                }
            }

            if (AllowContainer)
            {
                containerPattern.ScaleX = TextBoundingBox.Width + 7;
                containerPattern.Y = ImageBoundingBox.GetPoint(RectanglePoint.Middle, 0, Size == UIControlSize.Small ? 0 : -.5f).Y;
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

            imageShadow.MatchTransform(image);
            imageShadow.Y += 1;
        }

        #endregion

        #region Protected members

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
                BoundingBox = new RectangleF(bbox.Left, bbox.Top, bbox.Width + imagePadding.X, bbox.Height + imagePadding.Y);
            }
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (BoundingBox.IsEmpty)
                return;

            if (!image.IsEmpty)
            {
                Effect? shader = null;
                if (IsMouseOver())
                {
                    RemizioneGame.Effects.ColorSaturation.SetColor(.7f, .7f, .7f, 1);
                    shader = RemizioneGame.Effects.ColorSaturation.Effect;
                }

                var currentImageOpacity = image.Opacity;
                Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp, shader);

                if (allowContainer)
                {
                    containerPattern.Draw(gameTime);
                    containerEdge.Draw(gameTime);
                }

                if (shadow)
                    imageShadow.Draw(gameTime);

                image.Draw(gameTime);

                Game.SpriteBatch.End();

                image.Opacity = currentImageOpacity;
            }

            if (displayMode == UIControlDisplayMode.ImageAndText)
            {
                Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp, Shader);
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

            if (glow)
                label.Opacity = image.Opacity;
            else
                label.Opacity = 1;
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
                    Shadow = allowContainer;
                    Invalidate();
                }
            }
        }

        // Beat
        public void Beat(int duration)
        {
            if (duration <= 0)
            {
                if (image.Tweens.ScaleTween != null)
                {
                    var scale = image.Tweens.ScaleTween.StartValue;
                    image.Tweens.ScaleTween = null;
                    image.Scale = scale;
                }
            }
            else
            {
                image.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.Linear, image.Scale, image.Scale * 1.1f, duration, -1);
            }

            imageShadow.Scale = image.Scale;
        }

        // BoundingBox
        public RectangleF BoundingBox { get; private set; }

        // ButtonBoundingBox
        public RectangleF ButtonBoundingBox => image.BoundingBox;

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

        // Glow
        public bool Glow
        {
            get => glow;
            set
            {
                if (value != glow)
                {
                    glow = value;
                    if (glow)
                        image.Tweens.OpacityTween = FloatTween.Create(TweenStyle.CubicInOut, 1, .9f, 700, -1);
                    else
                        image.Tweens.OpacityTween?.Stop();
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

        // ImageSource
        public ControlImageSource ImageSource
        {
            get => imageSource;
            set
            {
                if (value != imageSource)
                {
                    imageSource = value;
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

        // IsMouseOver
        public bool IsMouseOver()
        {
            return BoundingBox.Contains(InputManager.DefaultPlayer.Mouse.VirtualPosition);
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

        // Shader
        public Effect? Shader { get; set; }

        // Shadow
        public bool Shadow
        {
            get => shadow;
            set
            {
                if (value != shadow)
                {
                    this.shadow = value;
                    label.ShadowOffset = value ? new Vector2(0, .5f) : Vector2.Zero;
                }
            }
        }

        // Size
        public UIControlSize Size
        {
            get => size;
            set
            {
                if (value != size)
                {
                    size = value;
                    Invalidate();
                }
            }
        }

        // TestPressed
        public bool TestPressed(PlayerIndex playerIndex)
        {
            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed() && IsMouseOver())
                return true;

            if (inputBinding != null && inputBinding.IsPressed(playerIndex))
                return true;

            return false;
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
            set => label.Color = value;
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
