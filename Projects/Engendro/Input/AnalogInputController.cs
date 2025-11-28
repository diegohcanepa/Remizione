using Microsoft.Xna.Framework;

namespace Engendro.Input
{
    /// <summary>
    /// AnalogInputController
    /// </summary>
    public abstract class AnalogInputController
    {
        private int autoRepeatCooldown;

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
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    autoRepeatCooldown = 0;
                }
            }
        } = 100;

        // CanAutoRepeat
        public bool CanAutoRepeat => AutoRepeatRate > 0;

        // IsAutoRepeatCooldownRunning
        public bool IsAutoRepeatCooldownRunning => autoRepeatCooldown > 0 && CanAutoRepeat;

        // Tolerance
        public float Tolerance
        {
            get;
            set
            {
                if (value != field)
                {
                    OnToleranceChanged(value);
                    field = value;
                }
            }
        } = .3f;

        // Update
        public virtual void Update(GameTime gameTime)
        {
            if (CanAutoRepeat && autoRepeatCooldown > 0)
                autoRepeatCooldown -= gameTime.ElapsedGameTime.Milliseconds;
        }
    }
}
