using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle.Menus
{
    /// <summary>
    /// ChooseLanguageMenuScene
    /// </summary>
    public sealed partial class ChooseLanguageMenuScene : StandardMenuScene
    {
        private readonly OptionMenu menu;

        #region Constructor

        // Constructor
        public ChooseLanguageMenuScene(ScaryCastleGame game)
            : base(game, "")
        {
            menu = new OptionMenu(game, 6);
            menu.AddOption(new LanguageOption(game));
            menu.Position = new Vector2(Screen.Center.X, 30);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);
            menu.Draw(gameTime);
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

        // OnUnloadContent
        protected override void OnUnloadContent()
        {
            UserSettingsData.SaveCurrentSystemSettings(Game);
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
