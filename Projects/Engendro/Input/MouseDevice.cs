using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engendro.Input
{
    /// <summary>
    /// MouseDevice
    /// </summary>
    public sealed class MouseDevice : InputDevice
    {
        #region Private fields

        private MouseState previousState;
        private MouseState state;

        #endregion

        #region Constructor

        // Constructor
        internal MouseDevice(PlayerInputManager player)
            : base(player)
        {
        }

        #endregion

        #region Private members

        // IsButtonPressed
        private static bool IsButtonPressed(ButtonState currentState, ButtonState previousState)
        {
            return InputManager.AllowMouse && !InputManager.IsSuspended && currentState == ButtonState.Released && previousState == ButtonState.Pressed;
        }

        // IsButtonDown
        private static bool IsButtonDown(ButtonState buttonState)
        {
            return InputManager.AllowMouse && !InputManager.IsSuspended && buttonState == ButtonState.Pressed;
        }

        // IsButtonUp
        private static bool IsButtonUp(ButtonState buttonState)
        {
            return InputManager.AllowMouse && !InputManager.IsSuspended && buttonState == ButtonState.Released;
        }

        #endregion

        #region Protected members

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            previousState = state;
            state = Mouse.GetState();

            // 1. Left button hold
            if (state.LeftButton == ButtonState.Pressed)
            {
                LeftButtonHoldTime += gameTime.ElapsedGameTime.Milliseconds;
            }
            else
            {
                LeftButtonHoldTime = 0;
            }

            // 2. Right button hold
            if (state.RightButton == ButtonState.Pressed)
            {
                RightButtonHoldTime += gameTime.ElapsedGameTime.Milliseconds;
            }
            else
            {
                RightButtonHoldTime = 0;
            }

            if (state.LeftButton == ButtonState.Pressed ||
                state.RightButton == ButtonState.Pressed ||
                state.MiddleButton == ButtonState.Pressed ||
                state.XButton1 == ButtonState.Pressed ||
                state.XButton2 == ButtonState.Pressed)
            {
                InputManager.AllowMouse = true;
            }
        }

        #endregion

        // HasInput
        public override bool HasInput()
        {
            return state != previousState;
        }

        // IsLeftButtonDown
        public bool IsLeftButtonDown()
        {
            return IsButtonDown(state.LeftButton);
        }

        // IsLeftButtonHeld
        public bool IsLeftButtonHeld(int threshold = 500)
        {
            return InputManager.AllowMouse && !InputManager.IsSuspended && LeftButtonHoldTime >= threshold;
        }

        // IsLeftButtonPressed
        public bool IsLeftButtonPressed()
        {
            return IsButtonPressed(state.LeftButton, previousState.LeftButton);
        }

        // IsLeftButtonUp
        public bool IsLeftButtonUp()
        {
            return IsButtonUp(state.LeftButton);
        }

        // IsRightButtonDown
        public bool IsRightButtonDown()
        {
            return IsButtonDown(state.RightButton);
        }

        // IsRightButtonHeld
        public bool IsRightButtonHeld(int threshold = 500)
        {
            return InputManager.AllowMouse && !InputManager.IsSuspended && RightButtonHoldTime >= threshold;
        }

        // IsRightButtonPressed
        public bool IsRightButtonPressed()
        {
            return IsButtonPressed(state.RightButton, previousState.RightButton);
        }

        // IsRightButtonUp
        public bool IsRightButtonUp()
        {
            return IsButtonUp(state.RightButton);
        }

        // LeftButtonHoldTime
        public int LeftButtonHoldTime { get; private set; }

        // Position
        public Point Position => state.Position;

        // PreviousState
        public MouseState PreviousState => previousState;

        // Reset
        public override void Reset()
        {
            previousState = state;
            LeftButtonHoldTime = 0;
            RightButtonHoldTime = 0;
        }

        // RightButtonHoldTime
        public int RightButtonHoldTime { get; private set; }

        // Tag
        public object? Tag { get; set; }

        // VirtualPosition
        public Vector2 VirtualPosition => EngendroGame.Instance.ViewportAdapter.ToVirtual(Position);

        // WorldPosition
        public Vector2 WorldPosition(Camera camera)
        {
            return (VirtualPosition / camera.Zoom) + camera.Offset;
        }

        // X
        public int X => state.X;

        // Y
        public int Y => state.Y;
    }
}
