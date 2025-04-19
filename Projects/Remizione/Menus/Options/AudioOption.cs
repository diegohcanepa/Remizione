namespace Remizione.Menus
{
    /// <summary>
    /// AudioOption
    /// </summary>
    public sealed class AudioOption : ButtonOption<string>
    {
        // Constructor
        public AudioOption(RemizioneGame game)
            : base(game, "@Menu.Options.Audio")
        {
        }

        // OnPerformClick
        protected override void OnPerformClick()
        {
            if (Game.SceneManager.CurrentScene is MenuScene currentScene)
            {
                currentScene.GoToScene(new AudioMenuScene(Game));
            }
        }
    }
}
