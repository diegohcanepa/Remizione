using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ItemInfoScene
    /// </summary>
    public sealed class ItemInfoScene : Scene
    {
        private readonly UIButton button;
        private readonly Sprite image;
        private readonly Sprite imageSlot;
        private Item? item;
        private readonly TextSprite itemNameText;
        private readonly GameSession session;
        private readonly TextSprite descriptionText;

        #region Constructor

        // Constructor
        public ItemInfoScene(GameSession session)
        {
            this.session = session;
            this.BackgroundColor = Color.Black;

            PausePreviousScenes = true;
            ExclusiveDraw = true;

            // Image slot
            this.imageSlot = new()
            {
                PivotOrigin = RectanglePoint.Top,
                Position = Screen.Area.GetPoint(RectanglePoint.Top, 0, 20),
                RenderImage = Atlases.UI.InventoryItemSlot
            };

            // Image
            this.image = new()
            {
                PivotOrigin = RectanglePoint.Center,
                Position = imageSlot.BoundingBox.GetPoint(RectanglePoint.Center),
            };

            // Item name
            this.itemNameText = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.MouseCursor.Tooltip,
                PivotOrigin = RectanglePoint.Top,
                Position = imageSlot.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, 3),
                Scale = ScaleInfo.Text.Giant,
                Text = "Text"
            };

            // Description
            this.descriptionText = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.MouseCursor.Tooltip * .8f,
                MaximumWidth = 190,
                PivotOrigin = RectanglePoint.Top,
                Position = itemNameText.BoundingBox.GetPoint(RectanglePoint.Bottom),
                Scale = ScaleInfo.Text.VeryLarge
            };

            // Button
            this.button = new(null, 1.5f)
            {
                ImageName = nameof(Atlases.UI.DiscardItemIcon),
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.Area.GetPoint(RectanglePoint.RightBottom, 0, -2)
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
            Game.SpriteBatch.Begin(Game.Camera);
            imageSlot.Draw(gameTime);
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
                session.HUD.Message.Show(MessageKind.ItemDiscarded);
                Game.SceneManager.Pop();
                return HandleInputResult.Handled;
            }

            if (HandleMouseInput())
                return HandleInputResult.Handled;

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
            this.itemNameText.Text = item.Definition.DisplayName;
            this.descriptionText.Text = item.Definition.Description;
            this.image.RenderImage = item.Definition.Image;
        }
    }
}
