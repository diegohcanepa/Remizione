using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// InventorySlot
    /// </summary>
    public sealed class InventorySlot : GameObject
    {
        #region Private fields

        private readonly TextSprite amount;
        private readonly FloatTween blinkTween = new();
        private readonly Sprite icon;
        private readonly Sprite perishStateIcon;
        private readonly Sprite shadow;
        private readonly Sprite slot;
        private static int slotImageIndex;

        #endregion

        #region Constructor

        // Constructor
        public InventorySlot()
        {
            // Amount
            this.amount = new(Fonts.Common)
            {
                Color = Colors.Palette.Text,
                PivotOrigin = RectanglePoint.Top,
                Scale = ScaleInfo.Text.ExtraLarge
            };

            // Icon
            this.icon = new() { PivotOrigin = RectanglePoint.Center };

            // Perish state
            this.perishStateIcon = new() { PivotOrigin = RectanglePoint.Top, Scale = ScaleInfo.UIElement.Medium };

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
                RenderImage = Atlases.UI.InventoryItemSlots[slotImageIndex]
            };

            slotImageIndex++;
            if (slotImageIndex == Atlases.UI.InventoryItemSlots.Count)
                slotImageIndex = 0;
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
            amount.Y = slot.BoundingBox.Bottom;

            perishStateIcon.Position = amount.Position;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            slot.Draw(gameTime);

            if (Item == null)
                return;

            shadow.Draw(gameTime);
            icon.Draw(gameTime);

            if (Item.Definition.IsStackable)
                amount.Draw(gameTime);

            if (Item.Definition.IsPerishable)
                perishStateIcon.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            perishStateIcon.Update(gameTime);
        }

        #endregion

        // BoundingBox
        public RectangleF BoundingBox => slot.BoundingBox;

        // Contains
        public bool Contains(Vector2 position)
        {
            return slot.BoundingBox.Contains(position);
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

                        if (field.Definition.IsPerishable)
                        {
                            perishStateIcon.RenderImage = Atlases.UI.ItemPerishStates[(int)field.PerishState];

                            if (field.PerishState == ItemPerishState.Rotten)
                            {
                                blinkTween.Start(TweenStyle.Linear, 1, .6f, 600, -1);
                                perishStateIcon.Tweens.OpacityTween = blinkTween;
                            }
                        }
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