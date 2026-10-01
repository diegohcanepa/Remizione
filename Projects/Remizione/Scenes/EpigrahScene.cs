using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione
{
    /// <summary>
    /// EpigrahScene
    /// </summary>
    public sealed class EpigrahScene : Scene
    {
        private readonly TextSprite textSprite;

        #region Constructor

        // Constructor
        public EpigrahScene()
            : base()
        {
            this.BackgroundColor = Color.Black;
            this.PausePreviousScenes = true;

            // Text sprite
            this.textSprite = new(Fonts.Common)
            {
                Color = ColorPalette.Text.Highlight * .8f,
                MaximumWidth = (int)(Screen.NativeWidth * .6f),
                PivotOrigin = RectanglePoint.Center,
                Position = Screen.Center,
                Scale = ScaleInfo.Text.ExtraLarge
            };
        }

        #endregion

        #region Private members

        // HandleMouseInput
        private bool HandleMouseInput()
        {
            if (InputManager.DefaultPlayer.LastInputMethod != InputMethod.Mouse)
                return false;

            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed() ||
                InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                if (textSprite.IsTyping)
                {
                    textSprite.StopTyping();
                }
                else
                {
                    CanClose = true;
                    Game.SceneManager.Pop();
                }

                return true;
            }

            return false;
        }

        #endregion

        #region Protected members

        // OnActivate
        protected override void OnActivate()
        {
            base.OnActivate();
            MouseCursor.Icon = MouseCursorIcon.Cross;
            MouseCursor.Tooltip = null;
            MouseCursor.CustomImage = null;
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            textSprite.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput()
        {
            if (HandleMouseInput())
                return HandleInputResult.Handled;

            return base.OnHandleInput();
        }

        #endregion

        // CanClose
        public bool CanClose { get; private set; }

        // Show
        public void Show(string text)
        {
            this.textSprite.Text = text;
            CanClose = false;
        }
    }
}
