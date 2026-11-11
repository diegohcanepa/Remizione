using Microsoft.Xna.Framework;
using System;

namespace Engendro.Input
{
    /// <summary>
    /// TriggerInputController
    /// </summary>
    public sealed class TriggerInputController(AnalogButtonSide side) : AnalogInputController()
    {
        private bool waitForRelease;

        #region Protected members

        // OnToleranceChanged
        protected override void OnToleranceChanged(float value)
        {
            if (!value.IsBetween(0, 1))
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Value range is between 0 and 1.");
            }
        }

        #endregion

        // IsPressed
        public bool IsPressed(PlayerIndex playerIndex)
        {
            float GetTriggerValue(PlayerIndex playerIndex)
            {
                if (Side == AnalogButtonSide.Left)
                    return InputManager.Players[(int)playerIndex].GamePad.LeftTriggerPosition;
                else
                    return InputManager.Players[(int)playerIndex].GamePad.RightTriggerPosition;
            }

            if (IsAutoRepeatCooldownRunning)
                return false;

            var result = GetTriggerValue(playerIndex) > Tolerance;

            if (CanAutoRepeat)
            {
                if (result && !IsAutoRepeatCooldownRunning)
                {
                    ResetAutoRepeatCooldown();
                }
            }
            else if (result)
            {
                if (!waitForRelease)
                {
                    waitForRelease = true;
                }
                else
                {
                    result = false;
                }
            }
            else if (waitForRelease)
            {
                waitForRelease = false;
            }

            return result;
        }

        // Side
        public AnalogButtonSide Side { get; set; } = side;
    }
}