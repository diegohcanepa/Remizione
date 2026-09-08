using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// UIInventoryMeter
    /// </summary>
    public sealed class UIInventoryMeter : GameObject
    {
        #region Private fields

        private readonly TextSprite amountText;
        private readonly Sprite flyingIcon = new() { PivotOrigin = RectanglePoint.Center };
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

            // Amount
            this.amountText = new TextSprite(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.TerraDark,
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, 6, 0),
                Scale = ScaleInfo.Text.Large,
                Text = "00"
            };

            // Icon
            this.icon = new(Atlases.UI.Sack)
            {
                PivotOrigin = RectanglePoint.Center,
                Position = amountText.BoundingBox.GetPoint(RectanglePoint.Top, 0, -5)
            };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            flyingIcon.Draw(gameTime);
            icon.Draw(gameTime);
            amountText.Draw(gameTime);
            Game.SpriteBatch.End();
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
                amountText.Color = inventory.IsFull ? ColorPalette.Text.Red : ColorPalette.Text.Terra;
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

            Sound.Play(SoundNames.ItemAdded);
        }

        // AnimateAddItem
        public void AnimateAddItem(Item item, Vector2 hudPos)
        {
            if (item.Definition.Image is not AtlasImage image)
                return;

            Sound.Play(SoundNames.MoveToSack);

            flyingIcon.RenderImage = image;
            flyingIcon.Position = hudPos;
            flyingIcon.Scale = ScaleInfo.UIElement.Tiny;
            flyingIcon.Tweens.PositionTween = Vector2Tween.Create(TweenStyle.CubicInOut, flyingIcon.Position, icon.BoundingBox.Center, 1400);
            flyingIcon.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicInOut, flyingIcon.Scale, ScaleInfo.UIElement.Large, 600, 2, Animate);
        }
    }
}