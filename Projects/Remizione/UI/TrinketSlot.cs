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
        private readonly Vector2Tween itemImageScaleTween = new();
        private Item? lastKnownItem;
        private readonly PilgrimSack pilgrimSack;
        private readonly ImageSprite slotImage;

        #endregion

        #region Constructor

        // Constructor
        public TrinketSlot(PilgrimSack pilgrimSack)
            : base(pilgrimSack.Session.Game)
        {
            this.pilgrimSack = pilgrimSack;

            // Slot image
            this.slotImage = new ImageSprite(Game, Atlases.UI.TrinketSlot)
            {
                PivotOrigin = RectanglePoint.LeftTop,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftTop, 2, -2),
            };

            // Item image
            this.itemImage = new ImageSprite(Game)
            {
                PivotOrigin = RectanglePoint.Center,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Center, -.5f, -.5f),
                Scale = ScaleInfo.UIElement.Medium
            };
        }

        #endregion

        #region Private members

        // InvalidateItem
        private void InvalidateItem()
        {
            lastKnownItem = pilgrimSack.EquippedTrinket;
            if (lastKnownItem != null)
            {
                itemImage.Image = lastKnownItem.MetaItem.Image;
                itemImageScaleTween.Start(TweenStyle.Linear, new Vector2(.3f), ScaleInfo.UIElement.Small, 70);
                itemImage.Tweens.ScaleTween = itemImageScaleTween;
            }
            else
            {
                itemImage.Image = Atlases.UI.EquipmentSlotTrinketsIcon;
                itemImage.Scale = ScaleInfo.UIElement.Medium;
                itemImageScaleTween.Stop();
            }
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
            if (lastKnownItem != pilgrimSack.EquippedTrinket)
                InvalidateItem();

            itemImage.Update(gameTime);
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
