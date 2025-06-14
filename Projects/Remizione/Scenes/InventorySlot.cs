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
        private readonly TextSprite nameText;
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

            // Name text
            nameText = new TextSprite(game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Bottom,
                Scale = ScaleInfo.Text.Large
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
                iconImage.Color = IsSelected ? ColorPalette.InventoryItem.Active : ColorPalette.InventoryItem.Inactive;
                iconImage.Scale = IsSelected ? ScaleInfo.InventoryItem.Active : ScaleInfo.InventoryItem.Inactive;
                return;
            }

            if (IsSelected)
            {
                iconImage.Tweens.ColorTween = ColorTween.Create(TweenStyle.CubicInOut, iconImage.Color, Color.White, tweenDuration);
                iconImage.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicInOut, iconImage.Scale, ScaleInfo.InventoryItem.Active, tweenDuration);
            }
            else
            {
                iconImage.Tweens.ColorTween = ColorTween.Create(TweenStyle.CubicInOut, iconImage.Color, ColorPalette.InventoryItem.Inactive, tweenDuration);
                iconImage.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicInOut, iconImage.Scale, ScaleInfo.InventoryItem.Inactive, tweenDuration);
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            slotImage.Draw(gameTime);
            iconImage.Draw(gameTime);
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearWrap);
            if (IsSelected)
                nameText.Draw(gameTime);
            amountText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
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
                        nameText.Text = item.DisplayText;

                        if (item.MetaItem.Maximum > 1)
                            amountText.Text = $"{item.Count}/{item.MetaItem.Maximum}";
                        else
                            amountText.Text = string.Empty;
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
                amountText.Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Bottom, 3, -4);
                nameText.Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Top, 0, -1);
            }
        }

        // Reset
        public void Reset()
        {
            Item = null;
            iconImage.Image = null;
            Position = Vector2.Zero;
            nameText.Clear();
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
