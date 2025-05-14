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
        private readonly PopupMenu<string> menu;
        private readonly Vector2 position = new(5, 30);
        private readonly GameSession session;

        #region Constructor

        // Constructor
        public InGameMenuScene(GameSession session)
            : base(session.Game, SceneSettings.None)
        {
            this.session = session;

            menu = new(Game, HorizontalAlignment.Left, false)
            {
                Position = new(5, 30),
                TextScale = ScaleInfo.Text.VeryLarge,
            };

            menu.AddOption(Localization.GetLocalizedValue(InGameMenuOptionName.Inventory), session.ShowInventory);
            menu.AddOption(Localization.GetLocalizedValue(InGameMenuOptionName.Prayers));
            menu.AddOption(Localization.GetLocalizedValue(InGameMenuOptionName.Manifestations));
            menu.AddOption(Localization.GetLocalizedValue(InGameMenuOptionName.Attributes));
            menu.AddOption(Localization.GetLocalizedValue(InGameMenuOptionName.Map));
            menu.AddOption(Localization.GetLocalizedValue(InGameMenuOptionName.Creatures));
            menu.AddOption(Localization.GetLocalizedValue(InGameMenuOptionName.Settings));
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
            //menu.Show(position);
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
