using Engendro;
using Engendro.Audio;
using Engendro.Input;
using EngendroAdventure.Scripting;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// DialogBlockScene 
    /// </summary>
    public sealed class DialogBlockScene : Scene
    {
        #region Private fields

        private readonly UITextButton buttonQuit;
        private bool completed;
        private readonly DialogBlock dialogBlock;
        private readonly FloatTween fadeTween = new();
        private readonly UIContextMenu<string> menu;
        private Script? runningScript;
        private int runSelectedOptionCooldown;
        private readonly GameSession session;
        private bool terminate;

        #endregion

        #region Constructor

        // Constructor
        public DialogBlockScene(GameSession session, DialogBlock dialogBlock)
            : base(session.Game, SceneSettings.None)
        {
            this.session = session;
            this.dialogBlock = dialogBlock;

            this.menu = new UIContextMenu<string>(Game, Fonts.CommonOutline)
            {
                OptionTextScale = ScaleInfo.ContextMenu.Option,
                SelectInputBinding = InputBindings.SelectDialogOption
            };

            this.buttonQuit = new UITextButton(Game, InputBindings.Exit)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom),
            };

            fadeTween.Start(TweenStyle.Linear, 0, 1, 800);
        }

        #endregion

        #region Private members

        // EndOption
        private void EndOption()
        {
            runningScript = null;

            if (RunningOption != null)
            {
                if (string.IsNullOrWhiteSpace(RunningOption.ReadKey))
                    dialogBlock.Remove(RunningOption);

                InvalidateOptions();
                if (dialogBlock.AvailableOptions.Count == 0)
                    completed = true;

                if (completed)
                    SceneController.Pop();

                RunningOption = null;

                InputManager.Suspend(500);
            }
        }

        // HandleMouseInput
        private bool HandleMouseInput()
        {
            if (menu.SelectedOption != null && InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
            {
                if (menu.GetOptionAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) != null)
                {
                    MouseCursor.Instance.AnimateClick();
                    Sound.Play(SoundNames.UISelectA);
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
                menu.AddOption(option.Id.ToString(), optionText, Atlases.UI.GetImage("DialogOptionBullet"));
            }

            Layout();
        }

        // Layout
        private void Layout()
        {
            menu.X = 5;
            menu.Y = Screen.HUDArea.Bottom - menu.BoundingBox.Height - 2;

            if (dialogBlock.AllowQuit)
            {
                var buttons = new List<UITextButton>();

                if (dialogBlock.AllowQuit)
                    buttons.Add(buttonQuit);

                Utils.LayoutControlsVertically(buttons.ToArray(), 1);
            }
        }

        // RunSelectedOption
        private void RunSelectedOption()
        {
            if (menu.SelectedOption is UIContextMenuOption<string> menuOption)
            {
                RunningOption = dialogBlock.GetOption(int.Parse(menuOption.Key));

                if (RunningOption != null)
                {
                    session.DialogOptionId = RunningOption.Id;
                    runningScript = dialogBlock.Script;
                    session.AwaitScript(runningScript);
                }
            }
        }

        #endregion

        #region Protected members

        // OnActivate
        protected override void OnActivate()
        {
            fadeTween.Start(TweenStyle.Linear, 0, 1, 500);
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (completed || RunningOption != null || runSelectedOptionCooldown > 0)
                return;

            base.OnDraw(gameTime);

            RemizioneGame.Effects.ColorReduction.SetColor(fadeTween.CurrentValue);

            /*
            Game.SpriteBatch.Begin(Game.Camera);
            Game.Shapes.DrawRectangle(new RectangleF(0, menu.Y - 4, 240, 135 - menu.Y + 4), Color.Black * .5f);
            Game.SpriteBatch.End();
            */

            menu.Draw(gameTime);

            if (dialogBlock.AllowQuit)
                buttonQuit.Draw(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (fadeTween.IsRunning || RunningOption != null)
                return HandleInputResult.Unhandled;

            if (menu.HandleInput(gameTime) == HandleInputResult.Handled || runSelectedOptionCooldown > 0)
                return HandleInputResult.Handled;

            if (HandleMouseInput())
                return HandleInputResult.Handled;

            // Quit
            else if (dialogBlock.AllowQuit && buttonQuit.TestPressed(0))
            {
                SceneController.Pop();
            }

            // Select
            else if (menu.SelectInputBinding != null && menu.SelectInputBinding.IsPressed(PlayerIndex.One))
            {
                Sound.Play(SoundNames.UISelectA);
                runSelectedOptionCooldown = 500;
                return HandleInputResult.Handled;
            }

            return base.OnHandleInput(gameTime);
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

            fadeTween.Update(gameTime);

            if (RunningOption == null)
                menu.Update(gameTime);

            if (dialogBlock.AllowQuit)
                buttonQuit.Update(gameTime);

            if (runningScript != null && !session.ScriptProcessor.IsExecutingScript(runningScript) && IsCurrentScene)
            {
                EndOption();

                if (terminate)
                {
                    terminate = false;
                    SceneController.Pop();
                }
            }

            MouseCursor.Instance.State = menu.HoveredOption == null ? MouseCursorState.Cross : MouseCursorState.CrossOn;
        }

        #endregion

        // HasMouseControl
        public override bool HasMouseControl => RunningOption == null;

        // RunningOption
        public DialogOption? RunningOption { get; private set; }

        // Terminate
        public void Terminate() => terminate = true;
    }
}
