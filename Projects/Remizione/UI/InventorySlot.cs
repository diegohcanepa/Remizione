using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// InventorySlot
    /// </summary>
    public sealed class InventorySlot
    {
        #region Private fields

        private readonly TextSprite amount;
        private readonly Sprite icon;
        private readonly Sprite shadow;
        private readonly Sprite slot;

        #endregion

        #region Constructor

        // Constructor
        public InventorySlot()
        {
            // Amount
            this.amount = new(Fonts.Common)
            {
                Color = ColorPalette.Text.Terra,
                PivotOrigin = RectanglePoint.Top,
                Scale = ScaleInfo.Text.ExtraLarge
            };

            // Icon
            this.icon = new() { PivotOrigin = RectanglePoint.Center };

            // Shadow
            this.shadow = new()
            {
                Color = Color.Black,
                Opacity = 0.3f,
                PivotOrigin = RectanglePoint.Center,
                Scale = ScaleInfo.UIElement.Large
            };

            // Slot
            this.slot = new()
            {
                PivotOrigin = RectanglePoint.Center,
                RenderImage = Atlases.UI.InventoryItemSlots.GetRandomItem()
            };
        }

        #endregion

        #region Private members

        // UpdateSubElementPositions
        private void UpdateSubElementPositions()
        {
            var center = slot.BoundingBox.Center;

            icon.X = center.X;
            icon.Y = center.Y - 1;

            shadow.X = icon.X - 0.5f;
            shadow.Y = center.Y + 0.5f;

            amount.X = center.X;
            amount.Y = slot.BoundingBox.Bottom - 1;
        }

        #endregion

        // BoundingBox
        public RectangleF BoundingBox => slot.BoundingBox;

        // Contains
        public bool Contains(Vector2 position)
        {
            return slot.BoundingBox.Contains(position);
        }

        // Draw
        public void Draw(GameTime gameTime, bool isItemHeld)
        {
            slot.Draw(gameTime);

            if (Item == null)
                return;

            // Si el item no es apilable y lo tenemos agarrado, no dibujamos la sombra/icono en el slot
            if (isItemHeld && !Item!.Definition.IsStackable)
                return;

            shadow.Draw(gameTime);
            icon.Draw(gameTime);
            amount.Draw(gameTime);
        }

        // Item
        public Item? Item
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;

                    if (field != null)
                    {
                        icon.RenderImage = field.Definition.Image;
                        shadow.RenderImage = field.Definition.Image;

                        if (field.Definition.IsStackable || field.Definition.IsDepletable)
                            amount.Text = $"x{field.Amount}";
                        else
                            amount.Clear();
                    }
                    else
                    {
                        icon.RenderImage = null;
                        shadow.RenderImage = null;
                        amount.Clear();
                    }

                    ResetVisualState();
                    UpdateSubElementPositions();
                }
            }
        }

        // Position
        public Vector2 Position
        {
            get => slot.Position;
            set
            {
                if (slot.Position != value)
                {
                    slot.Position = value;
                    UpdateSubElementPositions();
                }
            }
        }

        // SetSelected
        public void SetSelected(bool isSelected)
        {
            var targetScale = isSelected ? ScaleInfo.UIElement.ExtraLarge : ScaleInfo.UIElement.Large;

            if (icon.Scale != targetScale)
            {
                icon.Scale = targetScale;
                shadow.Scale = targetScale;
            }
        }

        // ResetVisualState
        public void ResetVisualState()
        {
            icon.Scale = ScaleInfo.UIElement.Large;
            shadow.Scale = ScaleInfo.UIElement.Large;
        }
    }
}