using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Globalization;

namespace ScaryCastle
{
    /// <summary>
    /// DialogBlockScene 
    /// </summary>
    public sealed class DialogBlockScene : Scene
    {
        #region Private fields

        private bool completed;
        private readonly DialogBlock dialogBlock;
        private readonly UIContextMenu<string> menu;
        private Script? runningScript;
        private int runSelectedOptionCooldown;
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
                OptionColor = ColorPalette.Text.Highlight,
                OptionSelectedColor = ColorPalette.Text.Yellow,
                OptionTextScale = ScaleInfo.ContextMenu.Option,
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
            if (InputManager.DefaultPlayer.Mouse.IsRightButtonPressed() && dialogBlock.AllowQuit)
            {
                Game.SceneManager.Pop();
            }
            else if (menu.SelectedOption != null && InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
            {
                if (menu.GetOptionAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) != null)
                {
                    MouseCursor.PerformClick();
                    runSelectedOptionCooldown = 500;
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
                menu.AddOption(option.Id.ToString(), optionText, Atlases.UI.FindImage("DialogOptionBullet"));
            }

            Layout();
        }

        // Layout
        private void Layout()
        {
            menu.X = 8;
            menu.Y = Screen.HUDArea.Bottom - menu.BoundingBox.Height - 5;

            if (dialogBlock.AllowQuit)
            {
                var buttons = new List<UIButton>();

                //if (dialogBlock.AllowQuit)
                  //  buttons.Add(buttonQuit);

                Utils.LayoutControlsVertically([.. buttons], 1);
            }
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
            if (completed || RunningOption != null || runSelectedOptionCooldown > 0)
                return;

            base.OnDraw(gameTime);

            /*
            Game.SpriteBatch.Begin(Game.Camera);
            Game.Shapes.DrawRectangle(new RectangleF(0, menu.Y - 4, 240, 135 - menu.Y + 4), Color.Black);
            Game.SpriteBatch.End();
            */

            menu.Draw(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput()
        {
            if (RunningOption != null)
                return HandleInputResult.Unhandled;

            if (menu.HandleInput() == HandleInputResult.Handled || runSelectedOptionCooldown > 0)
                return HandleInputResult.Handled;

            if (HandleMouseInput())
                return HandleInputResult.Handled;

            // Select
            else if (menu.SelectInputBinding != null && menu.SelectInputBinding.IsPressed(PlayerIndex.One))
            {
                Sound.Play(SoundNames.UISelectA);
                runSelectedOptionCooldown = 500;
                return HandleInputResult.Handled;
            }

            return base.OnHandleInput();
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            base.OnLoadContent();
            InvalidateOptions();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (runSelectedOptionCooldown > 0)
            {
                runSelectedOptionCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                if (runSelectedOptionCooldown <= 0)
                    RunSelectedOption();
            }

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

            //MouseCursor.Instance.State = menu.HoveredOption == null ? MouseCursorState.Arrow : MouseCursorState.ArrowOn;
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
