using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// UIControl
    /// </summary>
    public abstract class UIControl : GameObject, IBoundingBox
    {
        #region Private fields

        private InputBinding? inputBinding;
        private bool isEnabled = true;
        private const string KeyboardPrefix = "Keyboard";
        private InputMethod lastKnownInputMethod;
        private Vector2 position;
        private bool small;

        #endregion

        #region Constructors

        // Constructor
        public UIControl(EngendroGame game, InputBinding? inputBinding = null)
            : base(game)
        {
            this.inputBinding = inputBinding;
        }

        #endregion

        #region Private members

        // GetInputBindingImage
        protected AtlasImage? GetInputBindingImage(string? sourceImageName, InputBinding? inputBinding)
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

        #endregion

        #region Protected members

        // Invalidate
        protected virtual void Invalidate()
        {
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
        }

        #endregion

        // BoundingBox
        public abstract RectangleF BoundingBox { get; }

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
