using Adberration.Scripting;
using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Adberration
{
    /// <summary>
    /// ScriptConsole
    /// </summary>
    public sealed class ScriptConsole : GameObject
    {
        #region Private fields

        private int commandIndex = -1;
        private readonly TextSprite cursorSprite;
        private string inputText = string.Empty;
        private readonly Session session;
        private readonly TextSprite textSprite;

        #endregion

        #region Constructor

        // Constructor
        public ScriptConsole(Session session, InputBinding inputBinding, TextSprite textSprite, RectangleF backgroundArea)
            : base(session.Game)
        {
            this.session = session;
            this.textSprite = textSprite;
            this.BackgroundArea = backgroundArea;
            this.InputBinding = inputBinding;
            this.TextDefaultColor = textSprite.Color;
            this.TextErrorColor = textSprite.Color;

            Game.Window.KeyDown += Window_KeyDown;
            Game.Window.TextInput += HandleTextInput;

            this.cursorSprite = new TextSprite(textSprite.Game, textSprite.Font)
            {
                Color = textSprite.Color,
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = textSprite.Position,
                Scale = textSprite.Scale,
                Text = "_"
            };

            this.cursorSprite.Tweens.OpacityTween = FloatTween.Create(TweenStyle.Linear, 0, 1, 400, -1);
        }

        private void Window_KeyDown(object? sender, InputKeyEventArgs e)
        {
            if (CommandList.Count == 0)
                return;

            if (e.Key is Keys.Down or Keys.Up)
            {
                if (commandIndex == -1)
                {
                    commandIndex = 0;
                }
            }
            else
            {
                return;
            }

            if (e.Key == Keys.Down)
            {
                commandIndex++;
                if (commandIndex == CommandList.Count)
                {
                    commandIndex = 0;
                }
            }
            else if (e.Key == Keys.Up)
            {
                commandIndex--;
                if (commandIndex < 0)
                {
                    commandIndex = 0;
                }
            }

            inputText = CommandList[commandIndex];
            textSprite.Text = inputText;
        }

        #endregion

        #region Private members

        // ProcessCommand
        private void ProcessCommand(string command)
        {
            try
            {
                session.ScriptProcessor.ExecuteCommand(command);
                CommandList.Remove(command);
                CommandList.Insert(0, command);
            }
            catch (Exception e)
            {
                if (e is ScriptException)
                {
                    textSprite.Text = e.Message;
                }
                else if (e.InnerException is ScriptException)
                {
                    textSprite.Text = e.InnerException.Message;
                }
                else
                {
                    textSprite.Text = e.Message;
                }

                HasError = true;
            }
        }

        // HandleTextInput
        private void HandleTextInput(object? sender, TextInputEventArgs e)
        {
            if (HasError)
            {
                inputText = string.Empty;
                HasError = false;
            }

            if (e.Key == Keys.Up)
            {
                if (commandIndex > 0)
                    commandIndex--;

                inputText = CommandList[commandIndex];
            }

            else if (e.Key == Keys.Down)
            {
                if (commandIndex < CommandList.Count - 1)
                    commandIndex++;

                inputText = CommandList[commandIndex];
            }

            else if (e.Key == Keys.Tab)
            {
                return;
            }
            else if (e.Key == Keys.Back)
            {
                if (inputText.Length > 0)
                    inputText = inputText.Remove(inputText.Length - 1);
            }
            else if (e.Key == Keys.Enter)
            {
                if (!HasError && inputText.Length > 0)
                {
                    ProcessCommand(inputText);
                    inputText = "";
                }
            }
            else
            {
                inputText += e.Character.ToString();
            }

            if (!HasError)
                textSprite.Text = inputText;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!IsActive)
                return;

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);

            Game.Shapes.DrawRectangle(BackgroundArea, Color.Black);
            textSprite.Draw(gameTime);

            if (!HasError && !string.IsNullOrWhiteSpace(inputText))
                cursorSprite.Draw(gameTime);

            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (InputBinding.IsKeyPressed() && InputManager.DefaultPlayer.Keyboard.IsKeyDown(Keys.LeftShift))
            {
                IsActive = !IsActive;
                return;
            }

            if (string.IsNullOrWhiteSpace(inputText) && !HasError)
            {
                textSprite.Text = "Type script command...";
                textSprite.Opacity = .8f;
            }
            else
            {
                textSprite.Opacity = 1;
            }

            cursorSprite.Update(gameTime);
            cursorSprite.Position = textSprite.IsEmpty ? textSprite.Position : textSprite.BoundingBox.GetPoint(RectanglePoint.RightBottom);
        }

        #endregion

        // BackgroundArea
        public RectangleF BackgroundArea { get; set; }

        // BackgroundColor
        public Color BackgroundColor { get; set; } = Color.Black;

        // CommandList
        public List<string> CommandList { get; } = [];

        // HasError
        public bool HasError
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    textSprite.Color = field ? TextErrorColor : TextDefaultColor;
                }
            }
        }

        // InputBinding
        public InputBinding InputBinding { get; }

        // IsActive
        public bool IsActive
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    HasError = false;
                    textSprite.Clear();
                    commandIndex = -1;
                    inputText = string.Empty;
                }
            }
        }

        // TextDefaultColor
        public Color TextDefaultColor { get; set; }

        // TextErrorColor
        public Color TextErrorColor { get; set; }
    }
}
