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
        private readonly ImageSprite slotImageSelected;

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
                PivotOrigin = RectanglePoint.Middle,
                Scale = ScaleInfo.UIElement.Medium
            };

            // Slot image selected
            this.slotImageSelected = new(game, Atlases.UI.InventorySlotSelected)
            {
                PivotOrigin = RectanglePoint.Middle,
                Scale = ScaleInfo.UIElement.Medium
            };

            // Amount text
            amountText = new TextSprite(game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Top,
                Scale = ScaleInfo.Text.Small
            };

            Reset();
        }

        #region Private fields

        // ChangeVisualState
        private void ChangeVisualState(bool animate)
        {
            const int tweenDuration = 300;

            if (!animate)
            {
                iconImage.Tweens.Reset();
                iconImage.Scale = IsSelected ? ScaleInfo.InventoryItem.Active : ScaleInfo.InventoryItem.Inactive;
                return;
            }

            if (IsSelected)
                iconImage.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicInOut, iconImage.Scale, ScaleInfo.InventoryItem.Active, tweenDuration);
            else
                iconImage.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicInOut, iconImage.Scale, ScaleInfo.InventoryItem.Inactive, tweenDuration);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);

            if (IsSelected)
                slotImageSelected.Draw(gameTime);
            else
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
            if (item != null && item.MetaItem.IsStackable && item.MetaItem.AllowEmpty && item.Count == 0)
                iconImage.Opacity = .5f;
            else
                iconImage.Opacity = 1;

            iconImage.Update(gameTime);
        }

        #endregion

        // BoundingBox
        public RectangleF BoundingBox => slotImage.BoundingBox;

        // IsSelected
        public bool IsSelected { get; private set; }

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
                        Unselect(false);
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
                iconImage.Position = value;
                iconImage.X += .5f;

                slotImage.Position = value;
                slotImageSelected.Position = value;
                amountText.Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Bottom);
            }
        }

        // Reset
        public void Reset()
        {
            Item = null;
            iconImage.Image = null;
            Position = Vector2.Zero;
            Unselect(false);
        }

        // Select
        public void Select(bool animate)
        {
            IsSelected = true;
            ChangeVisualState(animate);
        }

        // Unselect
        public void Unselect(bool animate)
        {
            IsSelected = false;
            ChangeVisualState(animate);
        }
    }
}
