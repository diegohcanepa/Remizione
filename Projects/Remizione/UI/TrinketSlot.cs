using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// TrinketSlot
    /// </summary>
    public sealed class TrinketSlot : GameObject
    {
        #region Private fields

        private Actor? actor;
        private readonly ImageSprite itemImage;
        private Item? lastKnownItem;
        private readonly ImageSprite slotImage;

        #endregion

        #region Constructor

        // Constructor
        public TrinketSlot(EngendroGame game)
            : base(game)
        {
            // Slot image
            this.slotImage = new ImageSprite(Game, Atlases.UI.TrinketSlot)
            {
                PivotOrigin = RectanglePoint.LeftTop,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftTop, 2, -2),
            };

            // Item image
            this.itemImage = new ImageSprite(Game)
            {
                PivotOrigin = RectanglePoint.Middle,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Middle),
                Scale = ScaleInfo.UIElement.Medium
            };
        }

        #endregion

        #region Private members

        // InvalidateItem
        private void InvalidateItem()
        {
            lastKnownItem = actor?.Inventory.Trinkets.SelectedItem;
            if (lastKnownItem != null)
                itemImage.Image = lastKnownItem.MetaItem.Image;
            else
                itemImage.Image = Atlases.UI.InventorySlotSadIcon;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            slotImage.Draw(gameTime);
            itemImage.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (lastKnownItem != actor?.Inventory.Trinkets.SelectedItem)
                InvalidateItem();
        }

        #endregion

        // Actor
        public Actor? Actor
        {
            get => actor;
            set
            {
                if (value != actor)
                {
                    actor = value;
                    lastKnownItem = null;
                    InvalidateItem();
                }
            }
        }
    }
}
