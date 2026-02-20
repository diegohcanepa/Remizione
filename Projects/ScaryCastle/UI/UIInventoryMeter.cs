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
        private readonly ImageSprite flyingIcon;
        private readonly ImageSprite icon;
        private readonly Inventory inventory;
        private int lastKnownCount = -1;

        #endregion

        #region Constructor

        // Constructor
        public UIInventoryMeter(Inventory inventory)
            : base(inventory.Session.Game)
        {
            this.inventory = inventory;

            // Icon
            this.icon = new(Game, Atlases.UI.Sack)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.Area.GetPoint(RectanglePoint.RightBottom, -7, -7),
            };

            // Amount
            this.amountText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Right,
                Position = icon.BoundingBox.GetPoint(RectanglePoint.Left, -1, 1),
                Scale = ScaleInfo.Text.ExtraLarge,
                Spacing = -6
            };

            // Flying icon
            this.flyingIcon = new(Game)
            {
                PivotOrigin = RectanglePoint.Center,
            };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            flyingIcon.Draw(gameTime);
            icon.Draw(gameTime);
            amountText.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            flyingIcon.Update(gameTime);
            icon.Update(gameTime);

            if (lastKnownCount != inventory.Count)
            {
                lastKnownCount = inventory.Count;
                amountText.Text = $"{inventory.Count}/{inventory.Capacity}";
            }
        }

        #endregion
    }
}
