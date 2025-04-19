using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engendro.Input
{
    /// <summary>
    /// KeyboardDevice
    /// </summary>
    public sealed class KeyboardDevice : InputDevice
    {
        private KeyboardState previousState;
        private KeyboardState state;

        #region Constructor

        // Constructor
        internal KeyboardDevice(PlayerInputManager player)
            : base(player)
        {
        }

        #endregion

        #region Protected members

        // CanUpdate
        protected override bool CanUpdate => InputManager.AllowKeyboard && base.CanUpdate;

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            previousState = state;
            state = Keyboard.GetState();
        }

        #endregion

        // GetPressedKeys
        public Keys[] GetPressedKeys() => state.GetPressedKeys();

        // HasInput
        public override bool HasInput() => state.GetPressedKeys().Length > 0;

        // IsAltDown
        public bool IsAltDown()
        {
            return InputManager.AllowKeyboard && !InputManager.IsSuspended && (state.IsKeyDown(Keys.LeftAlt) || state.IsKeyDown(Keys.RightAlt));
        }

        // IsControlDown
        public bool IsControlDown()
        {
            return InputManager.AllowKeyboard && !InputManager.IsSuspended && (state.IsKeyDown(Keys.LeftControl) || state.IsKeyDown(Keys.RightControl));
        }

        // IsKeyDown
        public bool IsKeyDown(Keys key)
        {
            return InputManager.AllowKeyboard && key != Keys.None && !InputManager.IsSuspended && state.IsKeyDown(key);
        }

        // IsKeyPressed
        public bool IsKeyPressed(Keys key)
        {
            return InputManager.AllowKeyboard && key != Keys.None && !InputManager.IsSuspended && state.IsKeyDown(key) && previousState.IsKeyUp(key);
        }

        // IsKeyUp
        public bool IsKeyUp(Keys key)
        {
            return InputManager.AllowKeyboard && key != Keys.None && !InputManager.IsSuspended && state.IsKeyUp(key) && previousState.IsKeyDown(key);
        }

        // IsShiftDown
        public bool IsShiftDown()
        {
            return InputManager.AllowKeyboard && !InputManager.IsSuspended && (state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.RightShift));
        }

        // Reset
        public override void Reset()
        {
            previousState = new KeyboardState();
            state = new KeyboardState();
        }

        // State
        public KeyboardState State => state;
    }
}
