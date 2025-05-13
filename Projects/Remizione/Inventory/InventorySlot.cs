using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione
{
    /// <summary>
    /// InventorySlot
    /// </summary>
    public sealed class InventorySlot : GameObject
    {
        private readonly TextSprite amountText;
        private Item? item;
        private readonly ImageSprite itemImage;
        private static readonly Vector2 itemScale = new(.5f);
        private readonly ImageSprite slotImage;
        private readonly ImageSprite unreadSignImage;

        // Constructor
        public InventorySlot(EngendroGame game)
            : base(game)
        {
            // Slot image
            this.slotImage = new ImageSprite(Game, Atlases.UI.InventorySlot)
            {
                Scale = new(.75f)
            };

            // Item image
            this.itemImage = new ImageSprite(Game)
            {
                PivotOrigin = RectanglePoint.Top,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Top, 0, 1.5f),
                Scale = itemScale,
                VisualParent = slotImage
            };

            // Amount
            this.amountText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.TextWhite,
                PivotOrigin = RectanglePoint.RightBottom,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.RightBottom, -1.5f, -.5f),
                Scale = ScaleInfo.TextQuickSlot,
                Spacing = -10,
                VisualParent = slotImage
            };

            // New sign image
            this.unreadSignImage = new ImageSprite(Game, Atlases.UI.UnreadSign)
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.RightTop, -1, 1.5f),
                Scale = itemScale,
                VisualParent = slotImage
            };

            this.Item = item;
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            slotImage.Draw(gameTime);
            if (item != null)
            {
                itemImage.Draw(gameTime);
                if (item.Unread)
                    unreadSignImage.Draw(gameTime);
            }
            Game.SpriteBatch.End();

            if (item != null && item.MetaItem.Maximum != 1)
            {
                Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
                amountText.Draw(gameTime);
                Game.SpriteBatch.End();
            }
        }

        #endregion

        // Action
        // TODO: Remove
        public ItemAction Action => ItemAction.None;

        // BoundingBox
        public RectangleF BoundingBox => slotImage.BoundingBox;

        // Item
        public Item? Item
        {
            get => item;
            set
            {
                if (value != item)
                {
                    item = value;
                    if (item != null)
                    {
                        itemImage.Opacity = item.Count == 0 ? .2f : 1;
                        amountText.Text = item.Count.ToString();
                    }
                }

                slotImage.Opacity = item == null ? .3f : 1;
            }
        }

        // Position
        public Vector2 Position
        {
            get => slotImage.Position;
            set => slotImage.Position = value;
        }

        // Size
        public Vector2 Size => new(slotImage.Width, slotImage.Height);
    }
}
