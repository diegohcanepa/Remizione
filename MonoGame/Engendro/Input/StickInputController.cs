using Microsoft.Xna.Framework;
using System;

namespace Engendro.Input
{
    /// <summary>
    /// StickInputController
    /// </summary>
    public sealed class StickInputController(GamePadThumbStick stick) : AnalogInputController()
    {
        #region Private members

        // GetThumbStickValue
        private Vector2 GetThumbStickValue(PlayerIndex playerIndex)
        {
            if (Stick == GamePadThumbStick.Left)
                return InputManager.Players[(int)playerIndex].GamePad.LeftThumbStickPosition;
            else
                return InputManager.Players[(int)playerIndex].GamePad.RightThumbStickPosition;
        }

        #endregion

        #region Protected members

        // OnToleranceChanged
        protected override void OnToleranceChanged(float value)
        {
            if (!value.IsBetween(-1, 1))
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Value range is -1 and 1.");
            }
        }

        #endregion

        // IsDown
        public bool IsDown(PlayerIndex playerIndex)
        {
            if (IsAutoRepeatCooldownRunning)
                return false;

            var y = GetThumbStickValue(playerIndex).Y * -1;
            if (y == 0)
                return false;

            var result = y > Tolerance;

            if (result)
                ResetAutoRepeatCooldown();

            return result;
        }

        // IsLeft
        public bool IsLeft(PlayerIndex playerIndex)
        {
            if (IsAutoRepeatCooldownRunning)
                return false;

            var x = GetThumbStickValue(playerIndex).X;
            if (x >= 0)
                return false;

            var result = x < Tolerance;

            if (result)
                ResetAutoRepeatCooldown();

            return result;
        }

        // IsRight
        public bool IsRight(PlayerIndex playerIndex)
        {
            if (IsAutoRepeatCooldownRunning)
                return false;

            var x = GetThumbStickValue(playerIndex).X;
            if (x <= 0)
                return false;

            var result = x > Tolerance;

            if (result)
                ResetAutoRepeatCooldown();

            return result;
        }

        // IsUp
        public bool IsUp(PlayerIndex playerIndex)
        {
            if (IsAutoRepeatCooldownRunning)
                return false;

            var y = GetThumbStickValue(playerIndex).Y * -1;
            if (y == 0)
                return false;

            var result = y < -Tolerance;

            if (result)
                ResetAutoRepeatCooldown();

            return result;
        }

        // Stick
        public GamePadThumbStick Stick { get; set; } = stick;
    }
}
