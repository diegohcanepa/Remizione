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
        private readonly UIButton button;
        private readonly Sprite container;
        private readonly Sprite image;
        private readonly Sprite imageShadow;
        private Item? item;
        private readonly TextSprite itemNameText;
        private readonly GameSession session;
        private readonly TextSprite descriptionText;

        #region Constructor

        // Constructor
        public ItemInfoScene(GameSession session)
        {
            this.session = session;

            PausePreviousScenes = true;

            // Container
            this.container = new(Atlases.UI.GetImage("ItemInfoContainer"))
            {
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.Area.GetPoint(RectanglePoint.Bottom, 0, -5)
            };

            // Image
            this.image = new()
            {
                PivotOrigin = RectanglePoint.Center,
                Position = container.BoundingBox.GetPoint(RectanglePoint.LeftTop, 9, 9),
                Scale = ScaleInfo.UIElement.Medium
            };

            // ImageShadow
            imageShadow = new()
            {
                Color = Color.Black,
                Opacity = ColorPalette.ShadowOpacity,
                PivotOrigin = RectanglePoint.Center,
                Scale = ScaleInfo.UIElement.Medium,
                X = image.X,
                Y = image.BoundingBox.Center.Y + 1,
            };

            // Item name text
            this.itemNameText = new(Fonts.Common)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Left,
                Position = container.BoundingBox.GetPoint(RectanglePoint.LeftTop, 22, 13),
                Scale = ScaleInfo.Text.ExtraLarge,
                ShadowOffset = new(.5f)
            };

            // Description sprite
            this.descriptionText = new(Fonts.Common)
            {
                Color = ColorPalette.Text.Sentence,
                Opacity = .7f,
                MaximumWidth = 130,
                PauseOnPunctuationMarks = false,
                PivotOrigin = RectanglePoint.LeftTop,
                Position = container.BoundingBox.GetPoint(RectanglePoint.LeftTop, 6, 23),
                Scale = ScaleInfo.Text.ExtraLarge,
                ShadowOffset = new(.5f),
                TypingSpeed = 20
            };

            // Button
            this.button = new(null, 1.5f)
            {
                ImageName = nameof(Atlases.UI.DiscardItemIcon),
                PivotOrigin = RectanglePoint.RightTop,
                Position = container.BoundingBox.GetPoint(RectanglePoint.RightTop, 0, 2)
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

                if (descriptionText.IsTyping)
                    descriptionText.StopTyping();
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
            Game.Shapes.DrawRectangle(Screen.Area, ColorPalette.SceneShade);
            container.Draw(gameTime);
            imageShadow.Draw(gameTime);
            image.Draw(gameTime);
            itemNameText.Draw(gameTime);
            descriptionText.Draw(gameTime);
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
                session.TextHUD.Message.Show(MessageKind.ItemDiscarded);
                Game.SceneManager.Pop();
                return HandleInputResult.Handled;
            }

            if (HandleMouseInput())
                return HandleInputResult.Handled;

            if (InputBindings.Continue.IsPressed(PlayerIndex.One))
            {
                if (descriptionText.IsTyping)
                    descriptionText.StopTyping();
                else
                    Game.SceneManager.Pop();
                return HandleInputResult.Handled;
            }

            return base.OnHandleInput();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            button.Update(gameTime);
            itemNameText.Update(gameTime);
            descriptionText.Update(gameTime);
        }

        #endregion

        // Show
        public void Show(Item item)
        {
            this.item = item;
            itemNameText.Text = item.Definition.DisplayName;
            descriptionText.Text = item.Definition.Description;
            this.imageShadow.RenderImage = item.Definition.Image;
            this.image.RenderImage = item.Definition.Image;
            MouseCursor.State = MouseCursorState.Arrow;
        }
    }
}
