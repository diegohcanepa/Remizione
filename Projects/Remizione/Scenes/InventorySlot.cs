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
        private readonly ImageSprite iconImage;
        private Item? item;
        private readonly ImageSprite slotImage;

        // Constructor
        public InventorySlot(EngendroGame game)
            : base(game)
        {
            // Icon image
            this.iconImage = new(game)
            {
                PivotOrigin = RectanglePoint.Middle,
                Scale = ScaleInfo.UIElement.Medium
            };

            // Slot image
            this.slotImage = new(game, Atlases.UI.InventorySlot)
            {
                //Scale = ScaleInfo.UIElement.Medium
            };

            // Amount text
            amountText = new TextSprite(game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Bottom,
                Scale = ScaleInfo.Text.Medium
            };

            Reset();
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            slotImage.Draw(gameTime);
            iconImage.Draw(gameTime);
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearWrap);
            amountText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (item != null && item.MetaItem.IsStackable && item.Count == 0)
                iconImage.Opacity = .5f;
            else
                iconImage.Opacity = 1;

            iconImage.Update(gameTime);
        }

        #endregion

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
                        if (item.MetaItem.Maximum > 1)
                            amountText.Text = $"{item.Count}/{item.MetaItem.Maximum}";
                        else
                            amountText.Text = string.Empty;
                    }
                    else
                    {
                        amountText.Clear();
                    }

                    iconImage.Image = item?.MetaItem.Image;
                }
            }
        }

        // Position
        public Vector2 Position
        {
            get => iconImage.Position;
            set
            {
                slotImage.Position = value;
                iconImage.Position = slotImage.BoundingBox.Center;
                amountText.Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, 2);
            }
        }

        // Reset
        public void Reset()
        {
            Item = null;
            iconImage.Image = null;
            Position = Vector2.Zero;
        }
    }
}
