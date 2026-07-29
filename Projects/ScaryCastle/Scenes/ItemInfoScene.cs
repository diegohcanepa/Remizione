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
        private bool allowDiscard;
        private readonly UIButton closeButton;
        private readonly UIButton discardButton;
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
                PivotOrigin = RectanglePoint.Center,
                Position = Screen.Area.GetPoint(RectanglePoint.Center, 0, -5)
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
                Opacity = .7f,
                PivotOrigin = RectanglePoint.Left,
                Position = container.BoundingBox.GetPoint(RectanglePoint.LeftTop, 22, 13),
                Scale = ScaleInfo.Text.VeryLarge,
                ShadowColor = ColorPalette.Shadow,
                ShadowOffset = new(.5f)
            };

            // Description sprite
            this.descriptionText = new(Fonts.Common)
            {
                Color = ColorPalette.Text.Highlight,
                Opacity = .7f,
                MaximumWidth = 120,
                PauseOnPunctuationMarks = false,
                PivotOrigin = RectanglePoint.LeftTop,
                Position = container.BoundingBox.GetPoint(RectanglePoint.LeftTop, 6, 23),
                Scale = ScaleInfo.Text.Large,
                ShadowColor = ColorPalette.Shadow,
                ShadowOffset = new(.5f),
            };

            // Close button
            this.closeButton = new(null)
            {
                ImageName = nameof(Atlases.UI.CloseWindowButton),
                PivotOrigin = RectanglePoint.Center,
                Position = container.BoundingBox.GetPoint(RectanglePoint.RightTop)
            };

            // Discard button
            this.discardButton = new(null, 1.5f)
            {
                ImageName = nameof(Atlases.UI.DiscardItemIcon),
                PivotOrigin = RectanglePoint.RightTop,
                Position = container.BoundingBox.GetPoint(RectanglePoint.RightBottom, 0, -2)
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
            //Game.Shapes.DrawRectangle(Screen.Area, Color.Black);
            container.Draw(gameTime);
            imageShadow.Draw(gameTime);
            image.Draw(gameTime);
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            itemNameText.Draw(gameTime);
            descriptionText.Draw(gameTime);
            Game.SpriteBatch.End();

            closeButton.Draw(gameTime);

            if (allowDiscard)
                discardButton.Draw(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput()
        {
            if (allowDiscard)
            {
                if (discardButton.TestPressed(PlayerIndex.One))
                {
                    item?.Remove();
                    session.InteractionContext.HeldItem = null;
                    session.TextHUD.Message.Show(MessageKind.ItemDiscarded);
                    Game.SceneManager.Pop();
                    return HandleInputResult.Handled;
                }
            }

            if (HandleMouseInput())
                return HandleInputResult.Handled;

            return base.OnHandleInput();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            closeButton.Update(gameTime);

            if (allowDiscard)
                discardButton.Update(gameTime);

            itemNameText.Update(gameTime);
            descriptionText.Update(gameTime);
        }

        #endregion

        // Show
        public void Show(Item item)
        {
            this.item = item;
            this.allowDiscard = item.Definition.Behavior == ItemBehavior.Common;
            this.itemNameText.Text = item.Definition.DisplayName;
            this.descriptionText.Text = item.Definition.Description;
            this.imageShadow.RenderImage = item.Definition.Image;
            this.image.RenderImage = item.Definition.Image;
        }
    }
}
