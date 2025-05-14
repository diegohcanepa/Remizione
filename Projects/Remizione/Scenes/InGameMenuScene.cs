using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Remizione.UI;

namespace Remizione
{
    /// <summary>
    /// InGameMenuScene
    /// </summary>
    public sealed class InGameMenuScene : Scene
    {
        private readonly InGameMenu<InGameMenuOptionName> menu;
        private readonly Vector2 position = new(5, 30);
        private readonly GameSession session;

        #region Constructor

        // Constructor
        public InGameMenuScene(GameSession session)
            : base(session.Game, SceneSettings.None)
        {
            this.session = session;

            menu = new(Game);
            menu.AddOption(InGameMenuOptionName.Inventory, Localization.GetLocalizedValue(InGameMenuOptionName.Inventory), session.ShowInventory);
            menu.AddOption(InGameMenuOptionName.Prayers, Localization.GetLocalizedValue(InGameMenuOptionName.Prayers), QuitToDesktopAction);
            menu.AddOption(InGameMenuOptionName.Manifestations, Localization.GetLocalizedValue(InGameMenuOptionName.Manifestations), QuitToDesktopAction);
            menu.AddOption(InGameMenuOptionName.Attributes, Localization.GetLocalizedValue(InGameMenuOptionName.Attributes), session.ShowCharacterSheet);
            menu.AddOption(InGameMenuOptionName.Map, Localization.GetLocalizedValue(InGameMenuOptionName.Map), QuitToDesktopAction);
            menu.AddOption(InGameMenuOptionName.Creatures, Localization.GetLocalizedValue(InGameMenuOptionName.Creatures), QuitToDesktopAction);
            menu.AddOption(InGameMenuOptionName.Settings, Localization.GetLocalizedValue(InGameMenuOptionName.Settings), QuitToDesktopAction);
            menu.AddOption(InGameMenuOptionName.QuitToDesktop, Localization.GetLocalizedValue(InGameMenuOptionName.QuitToDesktop), QuitToDesktopAction);
        }

        #endregion

        #region Private members

        // HandleMouseInput
        private bool HandleMouseInput()
        {
            if (InputManager.DefaultPlayer.LastInputMethod != InputMethod.Mouse)
                return false;

            if (InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                SceneController.Pop();
                return true;
            }

            return false;
        }

        // QuitToDesktopAction
        private void QuitToDesktopAction()
        {
            session.Game.Exit();
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            menu.Draw(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (menu.HandleInput(gameTime) == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            if (HandleMouseInput())
                return HandleInputResult.Handled;

            if (InputBindings.InGameMenu.IsPressed(PlayerIndex.One))
            {
                SceneController.Pop();
                return HandleInputResult.Handled;
            }

            return base.OnHandleInput(gameTime);
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            MouseCursor.Instance.State = MouseCursorState.Arrow;
            menu.Show(position);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            menu.Update(gameTime);
        }

        #endregion

        // IsActiveInGameLoop
        public override bool IsActiveInGameLoop => IsCurrentScene;
    }
}
