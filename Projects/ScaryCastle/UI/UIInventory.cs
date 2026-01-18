using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System.Globalization;

namespace ScaryCastle
{
    /// <summary>
    /// UIInventory
    /// </summary>
    public sealed class UIInventory : GameObject, IInputHandler
    {
        #region Private fields

        private readonly TextSprite[] amounts;
        private readonly ImageSprite[] icons;
        private readonly Inventory inventory;
        private readonly TextSprite itemName;
        private int lastSeenInventoryVersion;
        private readonly ImageSprite[] slots;

        #endregion

        #region Constructor

        // Constructor
        public UIInventory(Inventory inventory)
            : base(inventory.Session.Game)
        {
            this.inventory = inventory;
            this.amounts = new TextSprite[GameSettings.MaxInventoryCapacity];
            this.icons = new ImageSprite[GameSettings.MaxInventoryCapacity];
            this.slots = new ImageSprite[GameSettings.MaxInventoryCapacity];

            for (var i = 0; i < slots.Length; i++)
            {
                slots[i] = new(Game, Atlases.UI.InventorySlot)
                {
                    PivotOrigin = RectanglePoint.Bottom,
                    Y = Screen.Area.Bottom - 7
                };

                icons[i] = new(Game)
                {
                    PivotOrigin = RectanglePoint.Center,
                    Scale = ScaleInfo.UIElement.Medium,
                    Y = slots[i].BoundingBox.Center.Y
                };

                // Amount text
                amounts[i] = new(Game, Fonts.CommonOutline)
                {
                    Color = ColorPalette.Text.Default,
                    PivotOrigin = RectanglePoint.Top,
                    Y = slots[i].BoundingBox.Center.Y + 4,
                    Scale = ScaleInfo.Text.ExtraLarge
                };
            }

            // Item name
            this.itemName = new(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.OrangeLight,
                PivotOrigin = RectanglePoint.Bottom,
                Y = slots[0].BoundingBox.Top - 2,
                Scale = ScaleInfo.UISentence
            };

            RefreshUI();
        }

        #endregion

        #region Private members

        // HandleMouseInput
        private bool HandleMouseInput()
        {
            if (!IsVisible)
                return false;

            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
            {
                if (GetItemAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) is Item grabbedItem)
                {
                    if (grabbedItem.Definition.Image != null)
                    {
                        MouseCursor.AnimateClick();
                        Sound.Play(SoundNames.UISelectC);
                        MouseCursor.Item = grabbedItem;
                        return true;
                    }
                }
            }

            if (InputManager.DefaultPlayer.Mouse.IsRightButtonPressed() && IsVisible)
            {
                if (MouseCursor.Item != null)
                {
                    MouseCursor.Item = null;
                    Sound.Play(SoundNames.UISelectD);
                }
                else
                {
                    IsVisible = false;
                }

                return true;
            }

            return false;
        }

        // RefreshUI
        private void RefreshUI()
        {
            float screenWidth = Screen.NativeWidth;
            int slotCount = inventory.Capacity;
            float slotWidth = slots[0].BoundingBox.Width;
            float spacing = 1;

            float rowWidth = (slotCount * slotWidth) + ((slotCount - 1) * spacing);
            float startingX = (screenWidth - rowWidth) / 2;

            for (int i = 0; i < slotCount; i++)
            {
                slots[i].X = startingX + (i * (slotWidth + spacing));
                icons[i].Image = null;  
                amounts[i].Text = null;

                if (i < inventory.Count)
                {
                    icons[i].X = slots[i].BoundingBox.Center.X;
                    icons[i].Image = inventory[i].Definition.Image;

                    amounts[i].X = icons[i].X;
                    amounts[i].Text = inventory[i].Definition.IsStackable ? inventory[i].Count.ToString(CultureInfo.InvariantCulture) : null;
                }
            }

            var lt = slots[0].BoundingBox.GetPoint(RectanglePoint.LeftTop);
            var rb = slots[inventory.Capacity - 1].BoundingBox.GetPoint(RectanglePoint.RightBottom);

            BoundingBox = new RectangleF(lt.X, lt.Y, rb.X - lt.X, rb.Y - lt.Y);

            MouseCursor.Item = MouseCursor.Item;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!IsVisible)
                return;

            if (MouseCursor.Target != null)
            {
                if (BoundingBox.Contains(InputManager.DefaultPlayer.Mouse.VirtualPosition))
                    return;
            }

            Game.SpriteBatch.Begin(Game.Camera);
            for (var i = 0; i < inventory.Capacity; i++)
            {
                slots[i].Draw(gameTime);

                if (MouseCursor.Item?.Index == i)
                {
                    if (!inventory[i].Definition.IsStackable)
                       continue;
                }

                icons[i].Draw(gameTime);
                amounts[i].Draw(gameTime);
            }

            itemName.Draw(gameTime);

            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (IsVisible)
            {
                if (lastSeenInventoryVersion != inventory.ContentVersion)
                {
                    lastSeenInventoryVersion = inventory.ContentVersion;
                    RefreshUI();
                }

                for (var i = 0; i < inventory.Count; i++)
                {
                    icons[i].Scale = ScaleInfo.UIElement.Medium;
                }

                if (MouseCursor.Item == null && GetSelectedItem() is Item item)
                {
                    itemName.Text = item.Definition.LocalizedDisplayName;
                    itemName.X = slots[item.Index].X;
                    icons[item.Index].Scale = ScaleInfo.UIElement.Medium * 1.2f;
                }
                else
                {
                    itemName.Text = null;
                }
            }
            else
            {
                itemName.Text = null;
            }
        }

        #endregion

        // BoundingBox
        public RectangleF BoundingBox { get; private set; }

        // GetItemAt
        public Item? GetItemAt(Vector2 position)
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (slots[i].BoundingBox.Contains(position))
                    return i < inventory.Count ? inventory[i] : null;
            }

            return null;
        }

        // GetSelectedItem
        public Item? GetSelectedItem()
        {
            return GetItemAt(InputManager.DefaultPlayer.Mouse.VirtualPosition);
        }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            // Mouse input
            if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.Mouse)
            {
                if (HandleMouseInput())
                    return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // IsVisible
        public bool IsVisible { get; set; }
    }
}
