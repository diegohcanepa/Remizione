using Engendro;

namespace ScaryCastle.Menus
{
    /// <summary>
    /// GamePadOption
    /// </summary>
    public sealed class GamePadOption : ButtonOption<string>
    {
        private readonly ControlsCoreScene scene;

        // Constructor
        public GamePadOption(ScaryCastleGame game)
            : base(game, "@Menu.Options.Gamepad")
        {
            var runningPlatform = EngendroGame.RunningPlatform;

            scene = runningPlatform switch
            {
                // NintendoSwitch
                RunningPlatform.NintendoSwitch => new NintendoSwitchControlsScene(game),

                // PlayStation4
                RunningPlatform.PlayStation4 => new PlayStation4ControlsScene(game),

                // PlayStation5
                RunningPlatform.PlayStation5 => new PlayStation5ControlsScene(game),

                // XboxOne
                RunningPlatform.XboxOne or RunningPlatform.XboxSeries => new XboxOneControlsScene(game),

                _ => new WindowsControlsScene(game),
            };
        }

        // OnPerformClick
        protected override void OnPerformClick()
        {
            if (Game.SceneManager.CurrentScene is MenuScene currentScene)
            {
                currentScene.GoToScene(scene);
            }
        }
    }
}
