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
        private readonly ImageSprite arrow;
        private readonly ImageSprite gradient;
        private readonly FloatTween opacityTween = new();
        private readonly TextSprite textSprite;

        #region Constructor

        // Constructor
        public EchoScene(RemizioneGame game)
            : base(game, SceneSettings.None)
        {
            // Arrow
            this.arrow = new(game, Atlases.UI.DialogArrowLarge)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, -5, -3),
                Scale = ScaleInfo.UIElement.Medium
            };

            arrow.Tweens.YTween = FloatTween.Create(TweenStyle.CubicInOut, arrow.Y, arrow.Y + 1, 250, -1);

            // Text sprite
            this.textSprite = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                MaximumWidth = (int)(Screen.NativeWidth * .8f),
                PauseOnPunctuationMarks = false,
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.Area.GetPoint(RectanglePoint.Bottom, 0, -15),
                Scale = ScaleInfo.Text.VeryLarge
            };

            // Gradient
            this.gradient = new(game, Atlases.UI.BottomGradient)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.Area.GetPoint(RectanglePoint.Bottom)
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
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);
            gradient.Draw(gameTime);
            textSprite.Draw(gameTime);
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera);
            arrow.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (HandleMouseInput())
                return HandleInputResult.Handled;

            if (InputBindings.Continue.IsPressed(PlayerIndex.One))
            {
                if (textSprite.IsTyping)
                    textSprite.StopTyping();
                else
                    SceneController.Pop();
                return HandleInputResult.Handled;
            }

            return base.OnHandleInput(gameTime);
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
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
            arrow.Update(gameTime);
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
