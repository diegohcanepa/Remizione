namespace Remizione.Menus
{
    /// <summary>
    /// FullscreenOption
    /// </summary>
    internal sealed class FullscreenOption : BooleanOption
    {
        // Constructor
        public FullscreenOption(RemizioneGame game)
            : base(game, "@Menu.Options.Fullscreen", game.IsFullScreen)
        {
        }

        // OnValueChanged
        protected override void OnValueChanged(bool newValue)
        {
            if (Value)
                Game.SwitchToFullScreen();
            else
                Game.SwitchToWindowedMode();
        }
    }

}
