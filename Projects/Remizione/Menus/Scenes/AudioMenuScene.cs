using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace Remizione.Menus
{
    /// <summary>
    /// AudioMenuScene
    /// </summary>
    public sealed partial class AudioMenuScene : StandardMenuScene
    {
        private SoundInstance? ambience;
        private SoundInstance? backgroundMusic;
        private readonly OptionMenu menu;

        #region Constructor

        // Constructor
        public AudioMenuScene(RemizioneGame game)
            : base(game, "@Menu.Titles.Audio")
        {
            menu = new OptionMenu(game, 6);

            menu.AddOption(new VolumeOption(game, VolumeCategory.Master));
            menu.AddOption(new VolumeOption(game, VolumeCategory.Music));
            menu.AddOption(new VolumeOption(game, VolumeCategory.FX));
            menu.AddOption(new VolumeOption(game, VolumeCategory.Ambient));

            menu.Position = new Vector2(Screen.Center.X, 70);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);
            menu.Draw(gameTime);
            DrawVersionInformation(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput()
        {
            if (menu.HandleInput() == HandleInputResult.Handled)
            {
                return HandleInputResult.Handled;
            }
            else
            {
                return base.OnHandleInput();
            }
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            base.OnLoadContent();

            ambience = Sound.Find("Hallucinations")?.PopInstance();
            if (ambience != null)
            {
                ambience.Looped = true;
                ambience.Play();
            }

            backgroundMusic = Sound.Find("BackIdleTension3")?.PopInstance();
            if (backgroundMusic != null)
            {
                backgroundMusic.Looped = true;
                backgroundMusic.Play();
            }
        }

        // OnUnloadContent
        protected override void OnUnloadContent()
        {
            UserSettingsData.SaveCurrentSystemSettings(Game);

            ambience?.Stop(300);
            ambience = null;

            backgroundMusic?.Stop(300);
            backgroundMusic = null;

            base.OnUnloadContent();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            menu.Update(gameTime);
        }

        #endregion
    }
}
