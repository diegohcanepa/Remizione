using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione
{
    /// <summary>
    /// InventorySlot
    /// </summary>
    public sealed class InventorySlot : GameObject
    {
        #region Private fields

        private readonly TextSprite amountText;
        private readonly InventoryGrid grid;
        private readonly ImageSprite icon;
        private Item? item;
        private readonly ImageSprite selectedSlotImage;
        private readonly ImageSprite slotImage;
        private readonly ImageSprite stateIcon;

        #endregion

        #region Constructor

        // Constructor
        public InventorySlot(InventoryGrid grid)
            : base(grid.Game)
        {
            this.grid = grid;

            // Icon image
            this.icon = new(Game)
            {
                PivotOrigin = RectanglePoint.Center,
                Scale = ScaleInfo.UIElement.Medium
            };

            // Slot image
            this.slotImage = new(Game, Atlases.UI.InventorySlot);

            // Selected slot image
            this.selectedSlotImage = new(Game, Atlases.UI.InventorySlotSelected);

            // State icon image
            this.stateIcon = new(Game)
            {
                PivotOrigin = RectanglePoint.Center,
                Opacity = .4f,
            };

            if (grid.ItemContainer.Category == InventoryCategory.Traits)
                stateIcon.Image = Atlases.UI.InventorySlotQuestionIcon;
            else
                stateIcon.Image = Atlases.UI.InventorySlotLockIcon;

            // Amount text
            amountText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Bottom,
                Scale = ScaleInfo.Text.Large
            };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            if (IsSelected)
                selectedSlotImage.Draw(gameTime);
            else
                slotImage.Draw(gameTime);

            if (Item == null)
            {
                if (Index > grid.ItemContainer.Size - 1 || grid.ItemContainer.Category == InventoryCategory.Traits)
                    stateIcon.Draw(gameTime);
            }
            else
                icon.Draw(gameTime);

            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearWrap);
            amountText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (item != null && item.MetaItem.IsStackable && item.Count == 0)
                icon.Opacity = .5f;
            else
                icon.Opacity = 1;

            icon.Update(gameTime);
        }

        #endregion

        // Activate
        public void Activate()
        {
            if (item != null)
            {
                icon.Scale = ScaleInfo.UIElement.Medium;
            }
        }

        // BoundingBox
        public RectangleF BoundingBox => slotImage.BoundingBox;

        // Deactivate
        public void Deactivate()
        {
            icon.Tweens.Reset();
            icon.Rotation = 0;
            icon.Scale = ScaleInfo.UIElement.Small;
        }

        // Index
        public int Index => grid.IndexOf(this);

        // IsMouseOver
        public bool IsMouseOver => BoundingBox.Contains(InputManager.DefaultPlayer.Mouse.VirtualPosition);

        // IsSelected
        public bool IsSelected => Item != null && (grid.SelectedSlot == this || IsMouseOver);

        // Item
        public Item? Item
        {
            get => item;
            set
            {
                if (value != item)
                {
                    item = value;
                    icon.Image = item?.MetaItem.Image;
                    Refresh();
                }
            }
        }

        // PerformDefaultAction
        public void PerformDefaultAction()
        {
            if (Item == null)
                return;

            if (Item.MetaItem.Category == InventoryCategory.Consumables)
            {
                Item.MetaItem.Sound?.Play();
                Item.Use();
                if (Item.Index < 0)
                    Item = null;
                Refresh();
                icon.Scale = ScaleInfo.UIElement.Medium;
                icon.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.Linear, icon.Scale, icon.Scale * 1.2f, 100, 2);
            }
        }

        // Position
        public Vector2 Position
        {
            get => slotImage.Position;
            set
            {
                slotImage.Position = value;
                selectedSlotImage.Position = value;
                icon.Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Center, 0, -1);
                stateIcon.Position = icon.Position;
                amountText.Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, 1.5f);
            }
        }

        // Refresh
        public void Refresh()
        {
            if (item != null)
            {
                amountText.Color = ColorPalette.Text.Default;

                if (item.MetaItem.Maximum > 1)
                {
                    if (item.MetaItem.Maximum == 999)
                        amountText.Text = $"{item.Count}";
                    else
                        amountText.Text = $"{item.Count}/{item.MetaItem.Maximum}";

                    if (item.IsStackFull)
                        amountText.Color = ColorPalette.Text.Terra;
                }
                else
                {
                    amountText.Text = string.Empty;
                }

                icon.Opacity = item.Count == 0 ? .3f : 1;
            }
            else
            {
                amountText.Clear();
            }
        }
    }
}
