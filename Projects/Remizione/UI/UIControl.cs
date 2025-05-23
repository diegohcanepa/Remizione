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

        private readonly ImageSprite container;
        private UIControlDisplayMode displayMode;
        private string? imageName;
        private readonly ImageSprite image;
        private bool isEnabled = true;
        private readonly float horzImagePadding = 1;
        private InputBinding? inputBinding;
        private readonly TextSprite label;
        private InputMethod lastKnownInputMethod;
        private RectanglePoint pivotOrigin;
        private Vector2 position;
        private Color textColor = ColorPalette.Text.Default;

        #endregion

        #region Constructors

        // Constructor
        public UIControl(EngendroGame game, InputBinding? inputBinding = null)
            : base(game)
        {
            // Container
            this.container = new ImageSprite(game)
            {
                PivotOrigin = RectanglePoint.Middle,
                Scale = ScaleInfo.UIElement.Medium
            };

            // Label
            this.label = new TextSprite(game, Fonts.CommonOutline)
            {
                ShadowColor = ColorPalette.UIControlShadow,
                Scale = ScaleInfo.Text.Large
            };

            this.image = new ImageSprite(game) { Scale = ScaleInfo.UIElement.Small };
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
            lastKnownInputMethod = InputManager.DefaultPlayer.LastInputMethod;

            if (DisplayMode == UIControlDisplayMode.ImageOnly)
            {
                image.PivotOrigin = pivotOrigin;
                image.Position = position;
                BoundingBox = image.BoundingBox;
            }
            else
            {
                label.PivotOrigin = pivotOrigin;
                label.Position = position;

                if (!image.IsEmpty)
                {
                    if (PivotOrigin == RectanglePoint.Right || PivotOrigin == RectanglePoint.RightBottom || PivotOrigin == RectanglePoint.RightTop)
                    {
                        label.X -= image.BoundingBox.Width / 2;
                        image.PivotOrigin = RectanglePoint.Left;
                        image.Position = label.BoundingBox.GetPoint(RectanglePoint.Right, horzImagePadding, 0);
                    }
                    else
                    {
                        label.X += image.BoundingBox.Width / 2;
                        image.PivotOrigin = RectanglePoint.Right;
                        image.Position = label.BoundingBox.GetPoint(RectanglePoint.Left, -horzImagePadding, -.5f);
                    }
                }

                BoundingBox = RectangleF.Union(image.BoundingBox, label.BoundingBox);

                if (image.IsEmpty)
                    container.Position = label.BoundingBox.Center;
                else
                    container.Position = BoundingBox.Center;
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (BoundingBox.IsEmpty)
                return;

            if (!container.IsEmpty)
            {
                Game.SpriteBatch.Begin(Game.Camera);
                container.Draw(gameTime);
                Game.SpriteBatch.End();
            }

            if (!image.IsEmpty)
            {
                Effect? shader = null;
                
                if (IsMouseOver())
                {
                    RemizioneGame.Effects.ColorSaturation.SetColor(.7f, .7f, .7f, 1);
                    shader = RemizioneGame.Effects.ColorSaturation.Effect;
                }

                Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp, shader);
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
            label.Color = textColor;
            image.Update(gameTime);
            label.Update(gameTime);

            if (InputManager.DefaultPlayer.LastInputMethod != lastKnownInputMethod)
                Invalidate();

            if (IsEnabled)
            {
                if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.Mouse)
                {
                    if (IsMouseOver())
                        label.Color = ColorPalette.Text.Hover;
                }
            }
        }

        #endregion

        // BoundingBox
        public RectangleF BoundingBox { get; private set; }

        // Container
        public AtlasImage? ContainerImage
        {
            get => container.Image;
            set
            {
                container.Image = value;
                Invalidate();
            }
        }

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

        // ImageScale
        public Vector2 ImageScale
        {
            get => image.Scale;
            set
            {
                if (value != image.Scale)
                {
                    image.Scale = value;
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

        // TestPressed
        public bool TestPressed(PlayerIndex playerIndex)
        {
            if (!IsEnabled)
                return false;

            var result = false;

            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed() && IsMouseOver())
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
            get => textColor;
            set
            {
                if (value != textColor)
                    textColor = value;
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
