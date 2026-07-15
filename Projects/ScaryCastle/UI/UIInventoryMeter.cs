using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        private readonly ItemContainer inventory;
        private int lastKnownCount = -1;
        private readonly FloatTween rotationTween = new();
        private readonly Vector2Tween scaleTween = new();

        #endregion

        #region Constructor

        // Constructor
        public UIInventoryMeter(ItemContainer inventory)
        {
            this.inventory = inventory;

            // Icon
            this.icon = new(Atlases.UI.Sack)
            {
                PivotOrigin = RectanglePoint.Center,
                Position = Screen.Area.GetPoint(RectanglePoint.RightBottom, -10, -10),
            };

            // Amount
            this.amountText = new TextSprite(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Right,
                Position = icon.BoundingBox.GetPoint(RectanglePoint.Left, -1, 1),
                Scale = ScaleInfo.Text.Giant,
                Spacing = -6
            };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            icon.Draw(gameTime);
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
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