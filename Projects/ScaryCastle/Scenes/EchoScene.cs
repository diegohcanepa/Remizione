using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScaryCastle
{
    /// <summary>
    /// EchoScene
    /// </summary>
    public sealed class EchoScene : Scene
    {
        private readonly Sprite arrow;
        private readonly Sprite gradient;
        private readonly Sprite image;
        private readonly FloatTween opacityTween = new();
        private readonly TextSprite textSprite;

        #region Constructor

        // Constructor
        public EchoScene(ScaryCastleGame game)
            : base(game)
        {
            const int topMargin = 35;

            // Gradient
            this.gradient = new(game, Atlases.UI.BottomGradient)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.Area.GetPoint(RectanglePoint.Bottom)
            };

            // Arrow
            this.arrow = new(game, Atlases.UI.DialogArrowLarge)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Bottom,
                Position = gradient.BoundingBox.GetPoint(RectanglePoint.RightBottom, -14, -8),
                Scale = ScaleInfo.UIElement.Large
            };

            arrow.Tweens.YTween = FloatTween.Create(TweenStyle.CubicInOut, arrow.Y, arrow.Y + 1, 250, -1);

            // Text sprite
            this.textSprite = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Sentence,
                MaximumWidth = (int)(Screen.NativeWidth * .8f),
                PauseOnPunctuationMarks = false,
                PivotOrigin = RectanglePoint.Top,
                Position = gradient.BoundingBox.GetPoint(RectanglePoint.Top, 0, topMargin),
                Scale = ScaleInfo.Text.VeryLarge,
                TypingSpeed = 20
            };

            // Image
            this.image = new(game)
            {
                PivotOrigin = RectanglePoint.Left,
                Position = gradient.BoundingBox.GetPoint(RectanglePoint.LeftTop, 8, topMargin)
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
                MouseCursor.PerformClick();

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
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);
            gradient.Draw(gameTime);
            image.Draw(gameTime);
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
                    Game.SceneManager.Pop();
                return HandleInputResult.Handled;
            }

            return base.OnHandleInput(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            arrow.Update(gameTime);
            textSprite.Update(gameTime);
            image.Opacity = textSprite.Opacity;
        }

        #endregion

        // Show
        public void Show(string text, bool allowTyping, AtlasImage? image = null)
        {
            textSprite.Text = text;
            opacityTween.Start(TweenStyle.CubicIn, 0, 1, 500);
            textSprite.Tweens.OpacityTween = opacityTween;

            if (allowTyping)
                textSprite.StartTyping();

            this.image.RenderImage = image;
        }
    }
}
