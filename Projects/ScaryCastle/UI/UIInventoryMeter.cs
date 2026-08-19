using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// UIInventoryMeter
    /// </summary>
    public sealed class UIInventoryMeter : GameObject
    {
        #region Private fields

        private readonly TextSprite amountText;
        private readonly Sprite icon;
        private readonly Sprite iconShadow;
        private readonly ItemContainer inventory;
        private int lastKnownCount = -1;
        private readonly FloatTween rotationTween = new();
        private readonly Vector2Tween scaleTween = new();
        private readonly Sprite slot;

        #endregion

        #region Constructor

        // Constructor
        public UIInventoryMeter(ItemContainer inventory)
        {
            this.inventory = inventory;

            // Amount
            this.amountText = new TextSprite(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, -10, 0),
                Scale = ScaleInfo.Text.ExtraLarge,
                Text = "00"
            };

            // Slot
            this.slot = new(Atlases.UI.InventoryMeterSlot)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Position = amountText.BoundingBox.GetPoint(RectanglePoint.Top, 0, 0)
            };

            // Icon
            this.icon = new(Atlases.UI.Sack)
            {
                PivotOrigin = RectanglePoint.Center,
                Position = slot.BoundingBox.Center
            };

            // IconShadow
            this.iconShadow = new(icon.RenderImage)
            {
                Color = Color.Black,
                Opacity = ColorPalette.ShadowOpacity,
                PivotOrigin = RectanglePoint.Center,
                Position = icon.BoundingBox.GetPoint(RectanglePoint.Center, -1.5f, 1)
            };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            slot.Draw(gameTime);
            iconShadow.Draw(gameTime);
            icon.Draw(gameTime);
            amountText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            icon.Update(gameTime);

            if (lastKnownCount != inventory.Count)
            {
                lastKnownCount = inventory.Count;
                amountText.Text = $"{inventory.Count}/{inventory.Capacity}";
                amountText.Color = inventory.IsFull ? ColorPalette.Text.Terra : ColorPalette.Text.Highlight;
            }
        }

        #endregion

        // Animate
        public void Animate()
        {
            rotationTween.Start(TweenStyle.QuadraticInOut, 0, 15, 50, 6);
            scaleTween.Start(TweenStyle.QuadraticInOut, Vector2.One, Vector2.One * 1.3f, 150, 2);

            icon.Tweens.RotationTween = rotationTween;
            icon.Tweens.ScaleTween = scaleTween;
        }
    }
}