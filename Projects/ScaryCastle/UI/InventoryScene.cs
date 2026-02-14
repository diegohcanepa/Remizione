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
        private readonly ImageSprite bottomGradient;
        private readonly Vector2 deckIconOriginalScale = Vector2.One;
        private readonly Vector2 deckIconSelectedScale = Vector2.One * 1.1f;
        private readonly ImageSprite[] icons;
        private readonly TextSprite itemName;
        private int lastSeenInventoryVersion = -1;
        private readonly ImageSprite[] shadows;
        private readonly ImageSprite[] slots;
        private readonly ImageSprite switchInventoryButton;

        #endregion

        #region Constructor

        // Constructor
        public InventoryScene(Inventory inventory)
            : base(inventory.Session.Game, SceneSettings.PausePreviousScenes)
        {
            this.amounts = new TextSprite[Inventory.MaximumCapacity];
            this.icons = new ImageSprite[Inventory.MaximumCapacity];
            this.shadows = new ImageSprite[Inventory.MaximumCapacity];
            this.slots = new ImageSprite[Inventory.MaximumCapacity];

            // Bottom gradient
            this.bottomGradient = new ImageSprite(Game, Atlases.UI.GetImage("InventoryContainer"))
            {
                Opacity = .8f,
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.Area.GetPoint(RectanglePoint.Bottom),
            };

            // Switch inventory button
            this.switchInventoryButton = new(Game)
            {
                PivotOrigin = RectanglePoint.Center,
            };

            // Slots
            for (var i = 0; i < slots.Length; i++)
            {
                slots[i] = new(Game, Atlases.UI.InventorySacredSlot)
                {
                    PivotOrigin = RectanglePoint.LeftBottom,
                    Y = Screen.Area.Bottom - 10
                };

                icons[i] = new(Game)
                {
                    PivotOrigin = RectanglePoint.Center,
                    Scale = ScaleInfo.UIElement.Medium,
                    Y = slots[i].BoundingBox.Center.Y
                };

                shadows[i] = new(Game)
                {
                    Color = Color.Black,
                    Opacity = ColorPalette.ShadowOpacity,
                    PivotOrigin = RectanglePoint.Center,
                    Scale = ScaleInfo.UIElement.Medium,
                    Y = slots[i].BoundingBox.Center.Y + 1
                };

                // Amount text
                amounts[i] = new(Game, Fonts.CommonOutline)
                {
                    Color = ColorPalette.Text.Highlight,
                    PivotOrigin = RectanglePoint.Top,
                    Y = slots[i].BoundingBox.Center.Y + 5,
                    Scale = ScaleInfo.Text.ExtraLarge
                };
            }

            // Item name
            this.itemName = new(Game, Fonts.CommonOutline)
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
                        MouseCursor.PerformClick();
                        SceneController.Pop();
                        return true;
                    }
                }
                else if (switchInventoryButton.BoundingBox.Contains(InputManager.DefaultPlayer.Mouse.VirtualPosition))
                {
                    MouseCursor.PerformClick();
                    SwitchInventory();
                }
                else
                {
                    MouseCursor.Shake();
                    Sound.Play(SoundNames.Error);
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
                    Sound.Play(SoundNames.Interact);
                    Inventory.Session.ShowEcho(item.Definition.LocalizedDescription, false, item.Definition.Image);
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
            float spacing = 1;

            float rowWidth = (slotCount * slotWidth) + ((slotCount - 1) * spacing);
            float startingX = (screenWidth - rowWidth) / 2;

            for (int i = 0; i < slotCount; i++)
            {
                slots[i].Image = Inventory.Category == InventoryCategory.Common ? Atlases.UI.InventoryCommonSlot : Atlases.UI.InventorySacredSlot;
                slots[i].X = startingX + (i * (slotWidth + spacing));
                icons[i].Image = null;
                shadows[i].Image = null;
                amounts[i].Text = null;

                if (i < Inventory.Count)
                {
                    icons[i].X = slots[i].BoundingBox.Center.X;
                    icons[i].Image = Inventory[i].Definition.Image;

                    shadows[i].X = icons[i].X - 1;
                    shadows[i].Image = Inventory[i].Definition.Image;

                    amounts[i].X = icons[i].X;
                    amounts[i].Text = Inventory[i].Definition.IsStackable ? Inventory[i].Count.ToString(CultureInfo.InvariantCulture) : null;
                }
            }

            switchInventoryButton.Scale = deckIconOriginalScale;
            switchInventoryButton.Image = Inventory.Category == InventoryCategory.Sacred ? Atlases.UI.CommonSackShortcut : Atlases.UI.SacredSackShortcut;
            switchInventoryButton.Position = slots[0].BoundingBox.GetPoint(RectanglePoint.Center, -switchInventoryButton.BoundingBox.Width - 4, 0);
        }

        // SwitchInventory
        private void SwitchInventory()
        {
            this.Inventory = Inventory.Category == InventoryCategory.Common ? Inventory.Session.SacredInventory : Inventory.Session.CommonInventory;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);

            // Gradient
            bottomGradient.Draw(gameTime);

            switchInventoryButton.Draw(gameTime);

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
                amounts[i].Draw(gameTime);
            }

            itemName.Draw(gameTime);

            Game.SpriteBatch.End();

            Inventory.Session.HUD.Draw(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
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

            MouseCursor.State = MouseCursorState.Hand;
            Inventory.Session.InteractionContext.HeldItem = null;

            if (lastSeenInventoryVersion != Inventory.ContentVersion)
            {
                lastSeenInventoryVersion = Inventory.ContentVersion;
                Refresh();
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            Inventory.Session.HUD.Update(gameTime);

            if (InputManager.DefaultPlayer.Mouse.VirtualPosition.Y < 105)
            {
                SceneController.Pop();
                return;
            }

            switchInventoryButton.Update(gameTime);

            for (var i = 0; i < Inventory.Count; i++)
            {
                icons[i].Scale = ScaleInfo.UIElement.Medium;
            }

            var cursorOverDeckIcon = switchInventoryButton.BoundingBox.Contains(InputManager.DefaultPlayer.Mouse.VirtualPosition);

            if (GetSelectedItem() is Item item)
            {
                itemName.Text = item.Definition.LocalizedDisplayName;
                itemName.X = slots[item.Index].BoundingBox.Center.X;
                icons[item.Index].Scale = ScaleInfo.InventoryHeldItem;
            }
            else
            {
                itemName.Text = null;
            }

            if (cursorOverDeckIcon)
            {
                if (switchInventoryButton.Scale != deckIconSelectedScale)
                {
                    switchInventoryButton.Scale = deckIconSelectedScale;
                }
            }
            else if (switchInventoryButton.Scale == deckIconSelectedScale)
            {
                switchInventoryButton.Scale = deckIconOriginalScale;
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
        public Inventory Inventory
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
