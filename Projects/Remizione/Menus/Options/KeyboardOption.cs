namespace Remizione.Menus
{
    /// <summary>
    /// KeyboardOption
    /// </summary>
    public sealed class KeyboardOption : ButtonOption<string>
    {
        // Constructor
        public KeyboardOption(RemizioneGame game)
            : base(game, "@Menu.Options.Keyboard")
        {
        }

        // OnPerformClick
        protected override void OnPerformClick()
        {
            if (Game.SceneManager.CurrentScene is MenuScene currentScene)
            {
                currentScene.GoToScene(new KeyboardControlsScene(Game));
            }
        }
    }
}
