using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScaryCastle
{
    /// <summary>
    /// ItemInfoScene
    /// </summary>
    public sealed class ItemInfoScene : Scene
    {
        private readonly Sprite arrow;
        private readonly UIButton button;
        private readonly Sprite gradient;
        private readonly Sprite image;
        private Item? item;
        private readonly FloatTween opacityTween = new();
        private readonly GameSession session;
        private readonly TextSprite textSprite;

        #region Constructor

        // Constructor
        public ItemInfoScene(GameSession session)
        {
            this.session = session;

            const int topMargin = 35;

            PausePreviousScenes = true;

            // Gradient
            this.gradient = new(Atlases.UI.BottomGradient)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.Area.GetPoint(RectanglePoint.Bottom)
            };

            // Arrow
            this.arrow = new(Atlases.UI.DialogArrowLarge)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Bottom,
                Position = gradient.BoundingBox.GetPoint(RectanglePoint.RightBottom, -14, -8),
                Scale = ScaleInfo.UIElement.Large
            };

            arrow.Tweens.YTween = FloatTween.Create(TweenStyle.CubicInOut, arrow.Y, arrow.Y + 1, 250, -1);

            // Text sprite
            this.textSprite = new(Fonts.CommonOutline)
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
            this.image = new()
            {
                PivotOrigin = RectanglePoint.Left,
                Position = gradient.BoundingBox.GetPoint(RectanglePoint.LeftTop, 8, topMargin)
            };

            // Button
            this.button = new()
            {
                ImageName = nameof(Atlases.UI.DiscardItemIcon),
                PivotOrigin = RectanglePoint.Right,
                Text = "Descartar",
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, 0, -40)
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

            button.Draw(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput()
        {
            if (button.TestPressed(PlayerIndex.One))
            {
                item?.Remove();
                session.InteractionContext.HeldItem = null;
                session.HUD.Message.Show(MessageKind.ItemDiscarded);
                Game.SceneManager.Pop();
                return HandleInputResult.Handled;
            }

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

            return base.OnHandleInput();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            arrow.Update(gameTime);
            button.Update(gameTime);
            textSprite.Update(gameTime);
            image.Opacity = textSprite.Opacity;
        }

        #endregion

        // Show
        public void Show(Item item)
        {
            this.item = item;
            textSprite.Text = item.Definition.Description;
            opacityTween.Start(TweenStyle.CubicIn, 0, 1, 500);
            textSprite.Tweens.OpacityTween = opacityTween;
            this.image.RenderImage = item.Definition.Image;
        }
    }
}
