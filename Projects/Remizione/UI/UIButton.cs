using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione
{
    /// <summary>
    /// UIButton
    /// </summary>
    public sealed class UIButton : GameObject, IBoundingBox
    {
        #region Private fields

        private string? imageName;
        private readonly ImageSprite image;
        private bool isEnabled = true;
        private InputBinding? inputBinding;
        private const string KeyboardPrefix = "Keyboard";
        private InputMethod lastKnownInputMethod;
        private bool small;

        #endregion

        #region Constructors

        // Constructor
        public UIButton(EngendroGame game, InputBinding? inputBinding = null)
            : base(game)
        {
            this.image = new ImageSprite(game);
            this.inputBinding = inputBinding;
            Invalidate();
        }

        #endregion

        #region Private members

        // GetInputBindingImage
        internal static AtlasImage? GetInputBindingImage(string? sourceImageName, InputBinding? inputBinding)
        {
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
            image.Scale = small ? ScaleInfo.UIElement.Tiny : ScaleInfo.UIElement.Medium;
            lastKnownInputMethod = InputManager.DefaultPlayer.LastInputMethod;
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
                image.Draw(gameTime);
                Game.SpriteBatch.End();
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            image.Update(gameTime);

            if (InputManager.DefaultPlayer.LastInputMethod != lastKnownInputMethod)
                Invalidate();

            IsMouseOver = false;

            if (IsEnabled)
            {
                if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.Mouse)
                    IsMouseOver = BoundingBox.Contains(InputManager.DefaultPlayer.Mouse.VirtualPosition);
            }
        }

        #endregion

        // BoundingBox
        public RectangleF BoundingBox => image.BoundingBox;

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
                    isEnabled = value;
            }
        }

        // IsMouseOver
        public bool IsMouseOver { get; private set; }

        // PivotOrigin
        public RectanglePoint PivotOrigin
        {
            get => image.PivotOrigin;
            set
            {
                if (value != image.PivotOrigin)
                {
                    image.PivotOrigin = value;
                    Invalidate();
                }
            }
        }

        // Position
        public Vector2 Position
        {
            get => image.Position;
            set
            {
                if (value != image.Position)
                {
                    image.Position = value;
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

        // X
        public float X
        {
            get => image.X;
            set
            {
                if (value != image.X)
                {
                    image.X = value;
                    Invalidate();
                }
            }
        }

        // Y
        public float Y
        {
            get => image.Y;
            set
            {
                if (value != image.Y)
                {
                    image.Y = value;
                    Invalidate();
                }
            }
        }
    }
}
