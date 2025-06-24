using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione
{
    /// <summary>
    /// UITextButton
    /// </summary>
    public sealed class UITextButton : GameObject, IBoundingBox
    {
        #region Private fields

        private readonly ImageSprite containerPattern;
        private readonly ImageSprite containerEdgeLeft;
        private const float horzImagePadding = 1.5f;
        private string? imageName;
        private readonly ImageSprite image;
        private bool isEnabled = true;
        private InputBinding? inputBinding;
        private readonly TextSprite label;
        private const string KeyboardPrefix = "Keyboard";
        private InputMethod lastKnownInputMethod;
        private RectanglePoint pivotOrigin;
        private Vector2 position;
        private bool small;

        #endregion

        #region Constructors

        // Constructor
        public UITextButton(EngendroGame game, InputBinding? inputBinding = null)
            : base(game)
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
                ShadowOffset = new(.5f)
            };

            this.image = new ImageSprite(game);
            this.inputBinding = inputBinding;
            this.label.Text = inputBinding == null ? string.Empty : Localization.GetValue(inputBinding);

            Invalidate();
        }

        #endregion

        #region Private members

        // Invalidate
        private void Invalidate()
        {
            // Image
            image.Image = UIButton.GetInputBindingImage(ImageName, InputBinding);
            image.Scale = small ? ScaleInfo.UIElement.Tiny : ScaleInfo.UIElement.Medium;
            lastKnownInputMethod = InputManager.DefaultPlayer.LastInputMethod;

            image.PivotOrigin = pivotOrigin;
            label.Scale = ScaleInfo.Text.Large;

            containerPattern.Image = small ? Atlases.UI.UIControlContainerPatternSmall : Atlases.UI.UIControlContainerPatternLarge;
            containerEdgeLeft.Image = small ? Atlases.UI.UIControlContainerEdgeSmall : Atlases.UI.UIControlContainerEdgeLarge;

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

            containerPattern.ScaleX = label.BoundingBox.Width + 7;
            containerPattern.Y = ImageBoundingBox.GetPoint(RectanglePoint.Middle, 0, -.3f).Y;
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
        }

        // InvalidateBoundingBox
        private void InvalidateBoundingBox()
        {
            var labelBBox = label.BoundingBox;

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
            else
            {
                BoundingBox = RectangleF.Union(image.BoundingBox, containerPattern.BoundingBox, containerEdgeLeft.BoundingBox);
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
            if (InputManager.DefaultPlayer.LastInputMethod != lastKnownInputMethod)
                Invalidate();

            image.Update(gameTime);
            label.Update(gameTime);

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

        // BoundingBox
        public RectangleF BoundingBox { get; private set; }

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

            if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.GamePad)
            {
                if (inputBinding != null && inputBinding.IsPressed(playerIndex))
                    result = true;
            }
            else
            {
                if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed() && IsMouseOver)
                    result = true;
            }

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
