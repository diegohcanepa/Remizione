using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System.Globalization;

namespace Remizione
{
    /// <summary>
    /// DialogBlockScene 
    /// </summary>
    public sealed class DialogBlockScene : Scene
    {
        #region Private fields

        private readonly Sprite background = new(Atlases.UI.EchoBackground) { PivotOrigin = RectanglePoint.LeftBottom, Position = Screen.Area.GetPoint(RectanglePoint.LeftBottom) };
        private bool completed;
        private readonly DialogBlock dialogBlock;
        private readonly UIContextMenu<string> menu;
        private Script? runningScript;
        private readonly GameSession session;
        private bool terminate;

        #endregion

        #region Constructor

        // Constructor
        public DialogBlockScene(GameSession session, DialogBlock dialogBlock)
        {
            this.session = session;
            this.dialogBlock = dialogBlock;

            this.menu = new UIContextMenu<string>(Fonts.CommonOutline)
            {
                Alignment = dialogBlock.Alignment,
                OptionSpacing = 2,
                OptionColor = Colors.Palette.Terra,
                OptionSelectedColor = Colors.Palette.Yellow,
                OptionTextScale = ScaleInfo.Text.VeryLarge,
                SelectInputBinding = InputBindings.SelectDialogOption
            };
        }

        #endregion

        #region Private members

        // EndOption
        private void EndOption()
        {
            runningScript = null;

            if (RunningOption != null)
            {
                InvalidateOptions();
                if (dialogBlock.AvailableOptions.Count == 0)
                    completed = true;

                if (completed)
                    Game.SceneManager.Pop();

                RunningOption = null;

                InputManager.Suspend(500);
            }
        }

        // HandleMouseInput
        private bool HandleMouseInput()
        {
            if (dialogBlock.AllowQuit && InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                Game.SceneManager.Pop();
            }
            else if (menu.SelectedOption != null && InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
            {
                if (menu.GetOptionAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) != null)
                {
                    RunSelectedOption();
                    return true;
                }
            }

            return false;
        }

        // InvalidateOptions
        private void InvalidateOptions()
        {
            menu.Clear();

            dialogBlock.Invalidate();

            foreach (var option in dialogBlock.AvailableOptions)
            {
                var optionText = option.Text;
                var menuOption = menu.AddOption(option.Id.ToString(CultureInfo.InvariantCulture), optionText);
                menuOption.IconOffset = new(0, -1);
            }

            Layout();
        }

        // Layout
        private void Layout()
        {
            menu.X = dialogBlock.Alignment == HorizontalAlignment.Left ? 8 : Screen.Area.Center.X;
            menu.Y = Screen.HUDArea.Bottom - menu.BoundingBox.Height - 8;
        }

        // RunSelectedOption
        private void RunSelectedOption()
        {
            if (menu.SelectedOption is UIContextMenuOption<string> menuOption)
            {
                RunningOption = dialogBlock.FindOption(int.Parse(menuOption.Key, CultureInfo.InvariantCulture));

                if (RunningOption != null)
                {
                    session.DialogOptionId = RunningOption.Id;
                    if (dialogBlock.Script != null)
                    {
                        runningScript = dialogBlock.Script;
                        session.AwaitScript(runningScript);
                    }
                    else
                    {
                        Game.SceneManager.Pop();
                    }
                }
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (completed || RunningOption != null)
                return;

            Game.SpriteBatch.Begin(Game.Camera);
            background.Draw(gameTime);
            session.HUD.AshMeter.Draw(gameTime);
            Game.SpriteBatch.End();

            session.HUD.InventoryMeter.Draw(gameTime);

            menu.Draw(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (RunningOption != null)
                return HandleInputResult.Unhandled;

            if (dialogBlock.AllowQuit && InputBindings.Back.IsPressed(0))
            {
                Game.SceneManager.Pop();
                return HandleInputResult.Handled;
            }

            if (menu.HandleInput(gameTime) == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            if (HandleMouseInput())
                return HandleInputResult.Handled;

            // Select
            else if (menu.SelectInputBinding != null && menu.SelectInputBinding.IsPressed(PlayerIndex.One))
            {
                Sound.Play(SoundNames.UISelect);
                return HandleInputResult.Handled;
            }

            return base.OnHandleInput(gameTime);
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            base.OnLoadContent();
            MouseCursor.Icon = MouseCursorIcon.Cross;
            InvalidateOptions();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (RunningOption == null)
                menu.Update(gameTime);

            if (runningScript != null && !session.ScriptProcessor.IsExecutingScript(runningScript) && IsCurrentScene)
            {
                EndOption();

                if (terminate)
                {
                    terminate = false;
                    Game.SceneManager.Pop();
                }
            }
        }

        #endregion

        // RunningOption
        public DialogOption? RunningOption { get; private set; }

        // Terminate
        public void Terminate()
        {
            terminate = true;
        }
    }
}
