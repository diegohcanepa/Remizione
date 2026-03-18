using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace ScaryCastle.Menus
{
    /// <summary>
    /// PauseMenuScene
    /// </summary>
    public sealed partial class PauseMenuScene : MenuScene
    {
        #region Private fields

        private readonly Sprite bottomOrnament;
        private bool disposeSession;
        private const float logoScale = .12f;
        private readonly Menu menu;
        private readonly GameSession session;

        #endregion

        #region Constructor

        // Constructor
        public PauseMenuScene(GameSession session)
            : base(session.Game, Atlases.Menu.PauseMenuBackground)
        {
            this.session = session;

            menu = new Menu(Game)
            {
            };

            menu.AddItem(MenuItemName.Resume, () => Game.SceneManager.Pop(), null);
            menu.AddItem(MenuItemName.Options, ShowOptions, Atlases.Menu.OptionsIcon);
            menu.AddItem(MenuItemName.ExitToMainMenu, ExitToMainMenu, null);

            this.bottomOrnament = new(Atlases.Menu.BottomOrnament)
            {
                PivotOrigin = RectanglePoint.Top,
                Position = menu.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, 7)
            };
        }

        #endregion

        #region Private members

        // ExitToMainMenu
        private void ExitToMainMenu()
        {
            if (Game.CurrentSession == null)
            {
                return;
            }

            MessageBoxScene messageBox = new(Game, MessageBoxOptions.Yes | MessageBoxOptions.No, ExitToMainMenuMessageAction, MessageBoxOptions.No)
            {
                Message = "@MessageBox.QuitToMenu",
                SubMessage = "@MessageBox." + (Game.CurrentSession.CanSave ? "ProgressWillBeSaved" : "ProgressWarning")
            };

            if (!Game.CurrentSession.CanSave)
            {
                messageBox.SubMessageColor = ColorPalette.MessageBoxRedText;
            }

            Game.SceneManager.Push(messageBox);
        }

        // ExitToMainMenuMessageAction
        private void ExitToMainMenuMessageAction(MessageBoxOptions option)
        {
            if (option == MessageBoxOptions.Yes)
            {
                TransitionManager.CurrentTransition.In(0);
                Game.CurrentSession?.Save();
                disposeSession = true;
            }
        }

        // ShowOptions
        private void ShowOptions()
        {
            OptionsMenuScene scene = new(Game);
            GoToScene(scene);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            menu.Draw(gameTime);

            Game.SpriteBatch.Begin(Game.Camera);
            bottomOrnament.Draw(gameTime);
            Game.SpriteBatch.End();

            DrawVersionInformation(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput()
        {
            if (disposeSession)
            {
                return HandleInputResult.Handled;
            }

            if (menu.HandleInput() == HandleInputResult.Handled)
            {
                return HandleInputResult.Handled;
            }

            if (InputBindings.InGameMenu.IsPressed(0) || InputBindings.Back.IsPressed(0))
            {
                InputManager.Suspend(300);
                Game.SceneManager.Pop();
                return HandleInputResult.Handled;
            }

            return base.OnHandleInput();
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            TransitionManager.PushTransition(new Transition() { TweenStyle = TweenStyle.CubicIn });

            base.OnLoadContent();

            disposeSession = false;
        }

        // OnUnloadContent
        protected override void OnUnloadContent()
        {
            base.OnUnloadContent();
            TransitionManager.PopTransition();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (disposeSession && Game.CurrentSession != null && !Game.CurrentSession.IsSaving)
            {
                Game.DisposeSession(new TitleMenuScene(Game));
            }

            menu.Update(gameTime);
        }

        #endregion
    }
}
