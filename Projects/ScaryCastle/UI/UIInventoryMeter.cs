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
        private readonly Inventory inventory;
        private int lastKnownCount = -1;
        private readonly FloatTween rotationTween = new();
        private readonly Vector2Tween scaleTween = new();

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
                PivotOrigin = RectanglePoint.Center,
                Position = Screen.Area.GetPoint(RectanglePoint.RightBottom, -12, -12),
            };

            // Amount
            this.amountText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Right,
                Position = icon.BoundingBox.GetPoint(RectanglePoint.Left, -1, 1),
                Scale = ScaleInfo.Text.Huge,
                Spacing = -6
            };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            icon.Draw(gameTime);
            amountText.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            icon.Update(gameTime);

            if (lastKnownCount != inventory.Count)
            {
                lastKnownCount = inventory.Count;
                amountText.Text = $"{inventory.Count}/{inventory.Capacity}";
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
