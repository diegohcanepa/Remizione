using Engendro.Input;

namespace Remizione.Menus
{
    /// <summary>
    /// VibrationOption
    /// </summary>
    internal sealed class VibrationOption : BooleanOption
    {
        // Constructor
        public VibrationOption(RemizioneGame game)
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

