using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione
{
    /// <summary>
    /// EchoScene
    /// </summary>
    public sealed class EchoScene : Scene
    {
        private readonly FloatTween opacityTween = new();
        private readonly Sprite background = new(Atlases.UI.EchoBackground) { PivotOrigin = RectanglePoint.LeftBottom, Position = Screen.Area.GetPoint(RectanglePoint.LeftBottom) };
        private readonly TextSprite textSprite;

        #region Constructor

        // Constructor
        public EchoScene()
            : base()
        {
            this.PausePreviousScenes = false;

            this.textSprite = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.TerraLight,
                MaximumWidth = (int)(Screen.NativeWidth * .7f),
                PauseOnPunctuationMarks = false,
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.Area.GetPoint(RectanglePoint.Bottom, 0, -15),
                Scale = ScaleInfo.Text.VeryLarge
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
                    textSprite.StopTyping();
                else
                    Game.SceneManager.Pop();
                return true;
            }

            return false;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            background.Draw(gameTime);
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

        // OnLoadContent
        protected override void OnLoadContent()
        {
            MouseCursor.Icon = MouseCursorIcon.Talk;

            if (!textSprite.IsEmpty)
            {
                opacityTween.Start(TweenStyle.CubicIn, 0, 1, 500);
                textSprite.Tweens.OpacityTween = opacityTween;
                textSprite.StartTyping();
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            textSprite.Update(gameTime);
        }

        #endregion

        // Show
        public void Show(string text)
        {
            textSprite.Text = text;
            opacityTween.Start(TweenStyle.CubicIn, 0, 1, 500);
            textSprite.Tweens.OpacityTween = opacityTween;
            textSprite.StartTyping();
        }

        // Text
        public string? Text
        {
            get => textSprite.Text;
            set => textSprite.Text = value;
        }
    }
}
