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
        private GamePadState currentState;
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

        // CanUpdate
        protected override bool CanUpdate => InputManager.AllowGamePad && base.CanUpdate;

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Player.AssignedGamepad.HasValue)
            {
                previousState = currentState;
                currentState = GamePad.GetState(Player.AssignedGamepad.Value, DeadZone);

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
            if (!currentState.IsConnected)
                return false;

            // Check buttons
            if (currentState.Buttons.A == ButtonState.Pressed ||
                currentState.Buttons.B == ButtonState.Pressed ||
                currentState.Buttons.X == ButtonState.Pressed ||
                currentState.Buttons.Y == ButtonState.Pressed ||
                currentState.Buttons.Start == ButtonState.Pressed ||
                currentState.Buttons.Back == ButtonState.Pressed ||
                currentState.Buttons.LeftShoulder == ButtonState.Pressed ||
                currentState.Buttons.RightShoulder == ButtonState.Pressed ||
                currentState.Buttons.LeftStick == ButtonState.Pressed ||
                currentState.Buttons.RightStick == ButtonState.Pressed ||
                currentState.Buttons.BigButton == ButtonState.Pressed) // Botón Xbox/Home
                return true;

            // Check triggers
            if (currentState.Triggers.Left > 0.0f || currentState.Triggers.Right > 0.0f)
                return true;

            // Check sticks
            if (currentState.ThumbSticks.Left.Length() > .1f || currentState.ThumbSticks.Right.Length() > .1f)
                return true;

            // Check DPad
            if (currentState.DPad.Up == ButtonState.Pressed || currentState.DPad.Down == ButtonState.Pressed ||
                currentState.DPad.Left == ButtonState.Pressed || currentState.DPad.Right == ButtonState.Pressed)
                return true;

            return false;
        }

        // IsButtonDown
        public bool IsButtonDown(Buttons button)
        {
            if (InputManager.IsSuspended)
                return false;

            button = SwapButtonAccordingly(button);

            return currentState.IsButtonDown(button);
        }

        // IsButtonPressed
        public bool IsButtonPressed(Buttons button)
        {
            if (InputManager.IsSuspended)
                return false;

            button = SwapButtonAccordingly(button);

            return currentState.IsButtonDown(button) && previousState.IsButtonUp(button);
        }

        // IsButtonUp
        public bool IsButtonUp(Buttons button)
        {
            if (InputManager.IsSuspended)
                return false;

            button = SwapButtonAccordingly(button);

            return currentState.IsButtonUp(button) && previousState.IsButtonDown(button);
        }

        // IsConnected
        public bool IsConnected => currentState.IsConnected;

        // LeftThumbStickPosition
        public Vector2 LeftThumbStickPosition => currentState.ThumbSticks.Left;

        // LeftTriggerPosition
        public float LeftTriggerPosition => currentState.Triggers.Left;

        // Reset
        public override void Reset()
        {
            currentState = new GamePadState();
            previousState = new GamePadState();
        }

        // RightThumbStickPosition
        public Vector2 RightThumbStickPosition => currentState.ThumbSticks.Right;

        // RightTriggerPosition
        public float RightTriggerPosition => currentState.Triggers.Right;

        // Style
        public static GamePadStyle Style { get; set; }

        // StopVibration
        public void StopVibration()
        {
            vibrationTimer.Stop();

            if (Player.AssignedGamepad.HasValue)
                GamePad.SetVibration(Player.AssignedGamepad.Value, 0, 0);
        }

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
