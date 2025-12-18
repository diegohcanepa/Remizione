using Engendro.Input;

namespace ScaryCastle.Menus
{
    /// <summary>
    /// VibrationOption
    /// </summary>
    internal sealed class VibrationOption : BooleanOption
    {
        // Constructor
        public VibrationOption(ScaryCastleGame game)
            : base(game, "@Menu.Options.Vibration", GamePadDevice.AllowVibration)
        {
        }

        #region Protected members

        // OnValueChanged
        protected override void OnValueChanged(bool newValue)
        {
            GamePadDevice.AllowVibration = newValue;
            if (newValue)
                InputManager.DefaultPlayer.GamePad.Vibrate(300, .5f, .5f);
        }

        #endregion
    }
}

