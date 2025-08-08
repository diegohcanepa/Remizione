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
        private bool hideText;
        private const float horzImagePadding = 1.5f;
        private readonly ImageSprite image;
        private string? imageName;
        private InputBinding? inputBinding;
        private bool isEnabled = true;
        private readonly TextSprite label;
        private InputMethod lastKnownInputMethod;
        private RectanglePoint pivotOrigin;
        private Vector2 position;
        private readonly Vector2Tween scaleTween = new();

        #endregion

        #region Constructor

        // Constructor
        public UITextButton(EngendroGame game, InputBinding? inputBinding = null)
            : base(game)
        {
            this.inputBinding = inputBinding;
            this.Camera = game.Camera;

            // Container
            this.containerPattern = new ImageSprite(Game, Atlases.UI.UITextButtonContainerPattern)
            {
                PivotOrigin = RectanglePoint.Right,
                Scale = ScaleInfo.UIElement.Medium
            };

            // ContainerEdgeLeft
            this.containerEdgeLeft = new ImageSprite(Game, Atlases.UI.UITextButtonContainerEdge)
            {
                PivotOrigin = RectanglePoint.Right,
                Scale = ScaleInfo.UIElement.Medium
            };

            // Label
            this.label = new TextSprite(game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                Scale = ScaleInfo.Text.VeryLarge
            };

            // Image
            this.image = new ImageSprite(game)
            {
                Scale = ScaleInfo.UIElement.Medium,
            };

            this.label.Text = inputBinding == null ? string.Empty : Localization.GetValue(inputBinding);

            Invalidate();
        }

        #endregion

        #region Private members

        // GetInputBindingImage
        private static AtlasImage? GetInputBindingImage(string? sourceImageName, InputBinding? inputBinding)
        {
            const string KeyboardPrefix = "Keyboard";

            if (Atlases.UI is not UIAtlas atlas)
                return null;

            var gamePad = InputManager.DefaultPlayer.LastInputMethod == InputMethod.GamePad;

            string? imageName = null;

            if (!string.IsNullOrWhiteSpace(sourceImageName))
            {
                imageName = sourceImageName;
            }
            else if (inputBinding != null)
            {
                if (gamePad)
                    imageName = inputBinding.Button.ToString();
                else
                    imageName = inputBinding.Keys[0].ToString();
            }

            if (imageName != null && inputBinding != null)
            {
                if (gamePad)
                    imageName = GamePadDevice.Style.ToString() + imageName;
                else
                    imageName = KeyboardPrefix + imageName;
            }

            return string.IsNullOrWhiteSpace(imageName) ? null : atlas.GetImage(imageName);
        }

        // Invalidate
        private void Invalidate()
        {
            // Image
            image.Image = GetInputBindingImage(ImageName, InputBinding);
            image.PivotOrigin = pivotOrigin;
            image.Position = Position;

            if (HasText)
            {
                LayoutText();
                BoundingBox = RectangleF.Union(image.BoundingBox, containerPattern.BoundingBox, containerEdgeLeft.BoundingBox);
            }
            else
            {
                BoundingBox = image.BoundingBox;
            }

            label.OpacityFactor = IsEnabled ? 1 : .3f;

            if (pivotOrigin == RectanglePoint.Bottom || pivotOrigin == RectanglePoint.Top)
            {
                var offset = BoundingBox.Width / 2 - (image.BoundingBox.Width / 2);

                image.X -= offset;
                containerEdgeLeft.X -= offset;
                containerPattern.X -= offset;
                label.X -= offset;
            }
        }

        // LayoutText
        private void LayoutText()
        {
            if (image.IsEmpty)
                return;

            if (image.Pivot.AtRight)
            {
                label.PivotOrigin = RectanglePoint.Right;
                label.Position = image.BoundingBox.GetPoint(RectanglePoint.Left, -horzImagePadding, .7f);
            }
            else
            {
                label.PivotOrigin = RectanglePoint.Left;
                label.Position = image.BoundingBox.GetPoint(RectanglePoint.Right, horzImagePadding, .7f);
            }

            containerPattern.ScaleX = label.BoundingBox.Width + 6;
            containerPattern.Y = ImageBoundingBox.GetPoint(RectanglePoint.Center, 0, 0).Y;
            containerEdgeLeft.Y = containerPattern.Y;

            if (image.Pivot.AtRight)
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
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (image.IsEmpty)
                return;

            Effect? shader = null;

            if (IsMouseOver)
            {
                RemizioneGame.Effects.ColorSaturation.SetColor(.7f, .7f, .7f, 1);
                shader = RemizioneGame.Effects.ColorSaturation.Effect;
            }

            Game.SpriteBatch.Begin(Camera, SamplerState.PointClamp, shader);
            
            if (HasText)
            {
                containerPattern.Draw(gameTime);
                containerEdgeLeft.Draw(gameTime);
                label.Draw(gameTime);
            }

            image.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (InputManager.DefaultPlayer.LastInputMethod != lastKnownInputMethod)
            {
                lastKnownInputMethod = InputManager.DefaultPlayer.LastInputMethod;
                Invalidate();
            }

            IsMouseOver = false;

            if (IsEnabled)
            {
                if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.Mouse)
                    IsMouseOver = BoundingBox.Contains(InputManager.DefaultPlayer.Mouse.VirtualPosition);
            }

            image.Update(gameTime);
            label.Update(gameTime);

            if (IsMouseOver)
                label.Color = ColorPalette.Text.Hover;
        }

        #endregion

        // AllowPressEffect
        public bool AllowPressEffect { get; set; } = true;

        // AllowSound
        public bool AllowSound { get; set; } = true;

        // BoundingBox
        public RectangleF BoundingBox { get; private set; }

        // ButtonOpacity
        public float ButtonOpacity
        {
            get => image.OpacityFactor;
            set => image.OpacityFactor = value;
        }

        // Camera
        public Camera Camera { get; set; }

        // HasText
        public bool HasText => !HideText && !label.IsEmpty;

        // HideText
        public bool HideText
        {
            get => hideText;
            set
            {
                if (value != hideText)
                {
                    hideText = value;
                    Invalidate();
                }
            }
        }

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
                    Invalidate();
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

        // Sound
        public Sound? Sound { get; set; }

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

            else if (inputBinding != null && inputBinding.IsPressed(playerIndex))
                result = true;

            if (result && AllowSound)
            {
                if (Sound != null)
                    Sound.Play();
                else
                    Sound.Play(SoundNames.UISelectA);
            }

            if (AllowPressEffect)
            {
                if (result && !scaleTween.IsRunning)
                {
                    scaleTween.Start(TweenStyle.Linear, ScaleInfo.UIElement.Medium, image.Scale * .95f, 60, 2);
                    image.Tweens.ScaleTween = scaleTween;
                }
            }
            else
            {
                scaleTween.Stop();
                image.Scale = ScaleInfo.UIElement.Medium;
            }

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
