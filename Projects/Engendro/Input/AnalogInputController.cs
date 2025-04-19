using Microsoft.Xna.Framework;

namespace Engendro.Input
{
    /// <summary>
    /// AnalogInputController
    /// </summary>
    public abstract class AnalogInputController
    {
        private int autoRepeatRate = 100;
        private int autoRepeatCooldown;
        private float tolerance = .3f;

        #region Protected members

        // OnToleranceChanged
        protected virtual void OnToleranceChanged(float value)
        {
        }

        // ResetAutoRepeatCooldown
        protected void ResetAutoRepeatCooldown()
        {
            if (CanAutoRepeat)
                autoRepeatCooldown = AutoRepeatRate;
        }

        #endregion

        // AutoRepeatRate
        public int AutoRepeatRate
        {
            get => autoRepeatRate;
            set
            {
                if (value != autoRepeatRate)
                {
                    autoRepeatRate = value;
                    autoRepeatCooldown = 0;
                }
            }
        }

        // CanAutoRepeat
        public bool CanAutoRepeat => AutoRepeatRate > 0;

        // IsAutoRepeatCooldownRunning
        public bool IsAutoRepeatCooldownRunning => autoRepeatCooldown > 0 && CanAutoRepeat;

        // Tolerance
        public float Tolerance
        {
            get => tolerance;
            set
            {
                if (value != tolerance)
                {
                    OnToleranceChanged(value);
                    tolerance = value;
                }
            }
        }

        // Update
        public virtual void Update(GameTime gameTime)
        {
            if (CanAutoRepeat && autoRepeatCooldown > 0)
                autoRepeatCooldown -= gameTime.ElapsedGameTime.Milliseconds;
        }
    }
}
