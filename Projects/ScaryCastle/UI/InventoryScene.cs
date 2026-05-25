using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System.Globalization;

namespace ScaryCastle
{
    /// <summary>
    /// InventoryScene
    /// </summary>
    public sealed class InventoryScene : Scene, IInputHandler
    {
        #region Private fields

        private readonly TextSprite[] amounts;
        private readonly Sprite[] gooIcons;
        private readonly Sprite[] icons;
        private readonly TextSprite itemName;
        private int lastSeenInventoryVersion = -1;
        private readonly Sprite[] shadows;
        private readonly Sprite[] slots;

        #endregion

        #region Constructor

        // Constructor
        public InventoryScene(PlayerInventory inventory)
        {
            this.PausePreviousScenes = false;
            this.amounts = new TextSprite[PlayerInventory.MaximumCapacity];
            this.gooIcons = new Sprite[PlayerInventory.MaximumCapacity];
            this.icons = new Sprite[PlayerInventory.MaximumCapacity];
            this.shadows = new Sprite[PlayerInventory.MaximumCapacity];
            this.slots = new Sprite[PlayerInventory.MaximumCapacity];

            // Slots
            for (var i = 0; i < slots.Length; i++)
            {
                slots[i] = new(Atlases.UI.InventorySlot)
                {
                    PivotOrigin = RectanglePoint.LeftBottom,
                    Y = Screen.Area.Bottom - 13
                };

                icons[i] = new()
                {
                    PivotOrigin = RectanglePoint.Center,
                    Y = slots[i].BoundingBox.Center.Y
                };

                gooIcons[i] = new()
                {
                    PivotOrigin = RectanglePoint.Top,
                    Scale = ScaleInfo.UIElement.Medium,
                    Y = slots[i].BoundingBox.Bottom - 2
                };

                shadows[i] = new()
                {
                    Color = Color.Black,
                    Opacity = ColorPalette.ShadowOpacity,
                    PivotOrigin = RectanglePoint.Center,
                    Scale = ScaleInfo.UIElement.Medium,
                    Y = slots[i].BoundingBox.Center.Y + 1
                };

                // Amount text
                amounts[i] = new(Fonts.CommonOutline)
                {
                    Color = ColorPalette.Text.Highlight,
                    PivotOrigin = RectanglePoint.Top,
                    Y = slots[i].BoundingBox.Center.Y + 6,
                    Scale = ScaleInfo.Text.Huge
                };
            }

            // Item name
            this.itemName = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Bottom,
                Y = slots[0].BoundingBox.Top - 3,
                Scale = ScaleInfo.UISentence
            };

            this.Inventory = inventory;
        }

        #endregion

        #region Private members

        // HandleMouseInput
        private bool HandleMouseInput()
        {
            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
            {
                if (Inventory.Session.InteractionContext.HeldItem == null && GetItemAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) is Item grabbedItem)
                {
                    if (grabbedItem.Definition.Image != null)
                    {
                        Inventory.Session.InteractionContext.HeldItem = grabbedItem;
                        MouseCursor.PerformClick(false);
                        Game.SceneManager.Pop();
                        return true;
                    }
                }
                else
                {
                    MouseCursor.Shake();
                }
            }

            /*
            if (MouseCursor.Item == null && InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                if (session.Player != null && GetItemAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) is Item itemToDrop)
                {
                    session.Inventory.DropItem(itemToDrop, session.Player.Position);
                }
            }
            */

            if (InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                if (GetItemAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) is Item item)
                {
                    Inventory.Session.ShowItemInfo(item);
                    Sound.Play(SoundNames.Interact);
                }
            }

            return false;
        }

        // Refresh
        private void Refresh()
        {
            float screenWidth = Screen.NativeWidth;
            int slotCount = Inventory.Capacity;
            float slotWidth = slots[0].BoundingBox.Width;
            float spacing = 2;

            float rowWidth = (slotCount * slotWidth) + ((slotCount - 1) * spacing);
            float startingX = (screenWidth - rowWidth) / 2;

            for (int i = 0; i < slotCount; i++)
            {
                slots[i].RenderImage = Atlases.UI.InventorySlot;
                slots[i].X = startingX + (i * (slotWidth + spacing));
                icons[i].RenderImage = null;
                gooIcons[i].RenderImage = null;
                shadows[i].RenderImage = null;
                amounts[i].Text = null;

                if (i < Inventory.Count)
                {
                    icons[i].X = slots[i].BoundingBox.Center.X;
                    icons[i].RenderImage = Inventory[i].Definition.Image;

                    if (Inventory[i].Definition.GooCost > 0)
                    {
                        gooIcons[i].X = slots[i].BoundingBox.Center.X;
                        gooIcons[i].RenderImage = Atlases.UI.GooIcon;
                    }

                    shadows[i].X = icons[i].X - 1;
                    shadows[i].RenderImage = Inventory[i].Definition.Image;

                    amounts[i].X = icons[i].X;

                    if (Inventory[i].Definition.IsStackable || Inventory[i].Definition.IsDepletable)
                        amounts[i].Text = Inventory[i].Amount.ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!IsCurrentScene)
                return;

            Game.SpriteBatch.Begin(Game.Camera);

            for (var i = 0; i < Inventory.Capacity; i++)
            {
                slots[i].Draw(gameTime);

                if (Inventory.Session.InteractionContext.HeldItem?.Index == i)
                {
                    if (!Inventory[i].Definition.IsStackable)
                        continue;
                }

                shadows[i].Draw(gameTime);
                icons[i].Draw(gameTime);
                gooIcons[i].Draw(gameTime);
                amounts[i].Draw(gameTime);
            }

            itemName.Draw(gameTime);

            Game.SpriteBatch.End();
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput()
        {
            // Mouse input
            if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.Mouse)
            {
                if (HandleMouseInput())
                    return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            base.OnLoadContent();

            Inventory.Session.InteractionContext.HeldItem = null;

            if (lastSeenInventoryVersion != Inventory.ContentVersion)
            {
                lastSeenInventoryVersion = Inventory.ContentVersion;
                Refresh();
            }
        }

        // OnUnloadContent
        protected override void OnUnloadContent()
        {
            base.OnUnloadContent();
            itemName.Clear();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (InputManager.DefaultPlayer.Mouse.VirtualPosition.Y < 105)
            {
                Game.SceneManager.Pop();
                return;
            }

            for (var i = 0; i < Inventory.Count; i++)
            {
                icons[i].Scale = ScaleInfo.UIElement.Medium;
            }

            if (GetSelectedItem() is Item item)
            {
                itemName.Text = item.Definition.DisplayName;
                itemName.X = slots[item.Index].BoundingBox.Center.X;
                icons[item.Index].Scale = ScaleInfo.InventoryHeldItem;
            }
            else
            {
                itemName.Text = null;
            }
        }

        #endregion

        // GetItemAt
        public Item? GetItemAt(Vector2 position)
        {
            for (int i = 0; i < Inventory.Count; i++)
            {
                if (slots[i].BoundingBox.Contains(position))
                    return i < Inventory.Count ? Inventory[i] : null;
            }

            return null;
        }

        // GetSelectedItem
        public Item? GetSelectedItem()
        {
            return GetItemAt(InputManager.DefaultPlayer.Mouse.VirtualPosition);
        }

        // Inventory
        public PlayerInventory Inventory
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Refresh();
                }
            }
        }
    }
}
