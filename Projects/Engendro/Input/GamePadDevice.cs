using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engendro.Input
{
    /// <summary>
    /// GamePadDevice
    /// </summary>
    public sealed class GamePadDevice : InputDevice
    {
        #region Private fields

        private static bool allowVibration = true;
        private GamePadState previousState;
        private int suspendVibrationInterval;
        private readonly Timer vibrationTimer = new();

        #endregion

        #region Constructor

        // Constructor
        internal GamePadDevice(PlayerInputManager player)
            : base(player)
        {
        }

        #endregion

        #region Private members

        // SwapButtonAccordingly
        private static Buttons SwapButtonAccordingly(Buttons button)
        {
            if (EngendroGame.RunningPlatform == RunningPlatform.NintendoSwitch)
            {
                if (button == Buttons.A)
                {
                    return Buttons.B;
                }
                else if (button == Buttons.B)
                {
                    return Buttons.A;
                }
                else if (button == Buttons.X)
                {
                    return Buttons.Y;
                }
                else if (button == Buttons.Y)
                {
                    return Buttons.X;
                }
            }

            return button;
        }

        #endregion

        #region Protected members

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Player.AssignedGamepad.HasValue)
            {
                previousState = State;
                State = GamePad.GetState(Player.AssignedGamepad.Value, DeadZone);

                if (vibrationTimer.IsRunning)
                {
                    vibrationTimer.Update(gameTime);
                    if (!vibrationTimer.IsRunning)
                        StopVibration();
                }
            }
            else
            {
                vibrationTimer.Stop();
            }
        }

        #endregion

        // AllowVibration
        public static bool AllowVibration
        {
            get => allowVibration;
            set
            {
                if (value != allowVibration)
                {
                    allowVibration = value;
                    if (!allowVibration)
                    {
                        foreach (var player in InputManager.Players)
                        {
                            player.GamePad.StopVibration();
                        }
                    }
                }
            }
        }

        // GamePadDeadZone
        public GamePadDeadZone DeadZone { get; set; } = GamePadDeadZone.IndependentAxes;

        // HasInput
        public override bool HasInput()
        {
            // Disconnected
            if (!State.IsConnected)
                return false;

            // Check buttons
            if (State.Buttons.A == ButtonState.Pressed ||
                State.Buttons.B == ButtonState.Pressed ||
                State.Buttons.X == ButtonState.Pressed ||
                State.Buttons.Y == ButtonState.Pressed ||
                State.Buttons.Start == ButtonState.Pressed ||
                State.Buttons.Back == ButtonState.Pressed ||
                State.Buttons.LeftShoulder == ButtonState.Pressed ||
                State.Buttons.RightShoulder == ButtonState.Pressed ||
                State.Buttons.LeftStick == ButtonState.Pressed ||
                State.Buttons.RightStick == ButtonState.Pressed ||
                State.Buttons.BigButton == ButtonState.Pressed) // Botón Xbox/Home
                return true;

            // Check triggers
            if (State.Triggers.Left > 0.0f || State.Triggers.Right > 0.0f)
                return true;

            // Check sticks
            if (State.ThumbSticks.Left.Length() > .1f || State.ThumbSticks.Right.Length() > .1f)
                return true;

            // Check DPad
            if (State.DPad.Up == ButtonState.Pressed || State.DPad.Down == ButtonState.Pressed ||
                State.DPad.Left == ButtonState.Pressed || State.DPad.Right == ButtonState.Pressed)
                return true;

            return false;
        }

        // IsButtonDown
        public bool IsButtonDown(Buttons button)
        {
            if (InputManager.IsSuspended)
                return false;

            button = SwapButtonAccordingly(button);

            return State.IsButtonDown(button);
        }

        // IsButtonPressed
        public bool IsButtonPressed(Buttons button)
        {
            if (InputManager.IsSuspended)
                return false;

            button = SwapButtonAccordingly(button);

            return State.IsButtonDown(button) && previousState.IsButtonUp(button);
        }

        // IsButtonUp
        public bool IsButtonUp(Buttons button)
        {
            if (InputManager.IsSuspended)
                return false;

            button = SwapButtonAccordingly(button);

            return State.IsButtonUp(button) && previousState.IsButtonDown(button);
        }

        // IsConnected
        public bool IsConnected => State.IsConnected;

        // LeftThumbStickPosition
        public Vector2 LeftThumbStickPosition => State.ThumbSticks.Left;

        // LeftTriggerPosition
        public float LeftTriggerPosition => State.Triggers.Left;

        // Reset
        public override void Reset()
        {
            State = new GamePadState();
            previousState = new GamePadState();
        }

        // RightThumbStickPosition
        public Vector2 RightThumbStickPosition => State.ThumbSticks.Right;

        // RightTriggerPosition
        public float RightTriggerPosition => State.Triggers.Right;

        // State
        public GamePadState State { get; private set; }

        // StopVibration
        public void StopVibration()
        {
            vibrationTimer.Stop();

            if (Player.AssignedGamepad.HasValue)
                GamePad.SetVibration(Player.AssignedGamepad.Value, 0, 0);
        }

        // Style
        public static GamePadStyle Style { get; set; }

        // SuspendVibration
        public void SuspendVibration(int duration) => suspendVibrationInterval = duration;

        // Vibrate
        public void Vibrate(GamePadVibrationSettings settings)
        {
            Vibrate(settings.Duration, settings.LeftMotor, settings.RightMotor, settings.LeftTrigger, settings.RightTrigger);
        }

        // Vibrate
        public bool Vibrate(int duration, float leftMotor, float rightMotor)
        {
            return Vibrate(duration, leftMotor, rightMotor, 0, 0);
        }

        // Vibrate
        public bool Vibrate(int duration, float leftMotor, float rightMotor, float leftTrigger, float rightTrigger)
        {
            if (!AllowVibration || suspendVibrationInterval > 0 || !Player.AssignedGamepad.HasValue)
                return false;

            GamePad.SetVibration(Player.AssignedGamepad.Value, leftMotor, rightMotor, leftTrigger, rightTrigger);
            vibrationTimer.Start(duration);
            return true;
        }
    }
}
