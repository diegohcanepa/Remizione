using Engendro.Audio;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace Remizione.Menus
{
    /// <summary>
    /// PreloadScene
    /// </summary>
    public partial class PreloadScene : SplashScene
    {
        private readonly RemizioneGame game;
        private Task? loadTask;
        private int minDuration;

        // Constructor
        public PreloadScene(RemizioneGame game)
            : base(game)
        {
            this.game = game;

            BackgroundColor = Color.Black;
            FadeDuration = 300;

#if WINDOWS
            NextScene = new ControllerAdviceScene(game);
#else
            NextScene = new PressToStartScene(game);
#endif

        }

        #region Private members

        // LoadCore
        private void LoadCore()
        {
            // Sounds
            AudioManager.LoadAll();
        }

        #endregion

        #region Protected members

        // OnLoadContent
        protected override void OnLoadContent()
        {
            base.OnLoadContent();
            minDuration = 3000;
            loadTask = Task.Run(LoadCore);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (minDuration >= 0)
            {
                minDuration -= gameTime.ElapsedGameTime.Milliseconds;
            }

            if (loadTask != null && loadTask.IsCompleted && minDuration <= 0)
            {
                loadTask = null;
                GoToNextScene();
            }
        }

        #endregion
    }
}
