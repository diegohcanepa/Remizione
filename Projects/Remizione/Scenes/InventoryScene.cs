using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione
{
    /// <summary>
    /// InventoryScene
    /// </summary>
    public sealed class InventoryScene : Scene
    {
        #region Private fields

        private MetaItemCategory activeCategory = MetaItemCategory.Equipment;
        private readonly ImageSprite bottomGradient;
        private readonly UITextButton buttonClose;
        private readonly TextSprite categoryText;
        private readonly ImageSprite inventoryCategoryContainer;
        private readonly TextSprite itemNameText;
        private readonly UIContextMenu menu;
        private readonly UITextButton nextCategory;
        private Item? originalSelectedItem;
        private readonly UITextButton previousCategory;
        private InventorySlot? selectedSlot;
        private readonly InventorySlot[] slots = new InventorySlot[12];
        private readonly StickInputController stick = new(GamePadThumbStick.Left) { AutoRepeatRate = 200 };

        #endregion

        #region Constructor

        // Constructor
        public InventoryScene(Actor owner)
            : base(owner.Game)
        {
            this.Owner = owner;

            // Create slots
            for (var i = 0; i < slots.Length; i++)
            {
                slots[i] = new(owner.Game);
            }

            // Close button
            buttonClose = new UITextButton(owner.Game, InputBindings.Close)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, 0, -2),
                Small = true
            };

            // Item name
            itemNameText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = new(Screen.NativeWidth / 2, 100),
                Scale = ScaleInfo.Text.VeryLarge
            };

            // Bottom gradient
            bottomGradient = new ImageSprite(owner.Game, Atlases.UI.BottomGradient)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.Area.GetPoint(RectanglePoint.Bottom),
                Scale = new Vector2(1, 1.2f)
            };

            // Menu
            menu = new UIContextMenu(Game)
            {
                OptionColor = ColorPalette.Text.Terra,
                OptionSelectedColor = ColorPalette.Text.Default,
                OptionTextScale = ScaleInfo.Text.VeryLarge,
            };

            menu.AddOption("Select", "Select");
            menu.AddOption("Info", "Info");
            menu.AddOption("Discard", "Discard");

            // Inventory category container
            inventoryCategoryContainer = new ImageSprite(owner.Game, Atlases.UI.GetImage("InventoryCategoryContainer"))
            {
                Opacity = .7f,
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.Bottom, 0, -2),
            };

            // Category text
            categoryText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Middle,
                Position = inventoryCategoryContainer.BoundingBox.Center,
                Scale = ScaleInfo.Text.Large
            };

            // Previous category
            previousCategory = new UITextButton(owner.Game, InputBindings.PreviousTab)
            {
                ImageName = nameof(InputBindings.PreviousTab),
                PivotOrigin = RectanglePoint.Right,
                Position = inventoryCategoryContainer.BoundingBox.GetPoint(RectanglePoint.Left),
            };

            // Next category
            nextCategory = new UITextButton(owner.Game, InputBindings.NextTab)
            {
                ImageName = nameof(InputBindings.NextTab),
                PivotOrigin = RectanglePoint.Left,
                Position = inventoryCategoryContainer.BoundingBox.GetPoint(RectanglePoint.Right),
            };
        }

        #endregion

        #region Private members

        // GetSlotAt
        private InventorySlot? GetSlotAt(Vector2 position)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].BoundingBox.Contains(position))
                    return slots[i];
            }

            return null;
        }

        // HandleMouseInput
        private bool HandleMouseInput()
        {
            if (InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                SceneController.Pop();
                return true;
            }

            if (!InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
                return false;

            if (GetSlotAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) is InventorySlot slot)
            {
                if (slot.Item != null)
                {
                    SelectSlot(slot);
                    MouseCursor.Instance.AnimateClick();
                    Sound.Play(SoundNames.UINavigation);
                }
            }

            return false;
        }

        // LayoutSlots
        private void LayoutSlots()
        {
            const float spacing = 1;
            var maximum = Owner.InventorySize;
            var slotSize = slots[0].BoundingBox.Width;
            var totalWidth = (maximum * slotSize) + ((maximum - 1) * spacing);
            var start = (Screen.NativeWidth - totalWidth) / 2;
            start += slots[0].BoundingBox.Width / 2;

            for (int i = 0; i < Owner.InventorySize; i++)
            {
                var x = start + (i * (slotSize + spacing));
                slots[i].Position = new(x, 110);
            }
        }

        // Sacrifice
        private void Sacrifice(Item item)
        {
            item.Remove();
            if (selectedSlot?.Item == item)
                selectedSlot.Item = null;

            Populate();
        }

        // SelectNextItem
        private bool SelectNextItem()
        {
            var result = Owner.Inventory.SelectNext();

            if (Owner.Inventory.SelectedItem != null)
                SelectSlot(Owner.Inventory.SelectedItem);

            return result;
        }

        // SelectPreviousItem
        private bool SelectPreviousItem()
        {
            var result = Owner.Inventory.SelectPrevious();

            if (Owner.Inventory.SelectedItem != null)
                SelectSlot(Owner.Inventory.SelectedItem);

            return result;
        }

        // SelectSlot
        private void SelectSlot(Item item)
        {
            for (var i = 0; i < slots.Length; i++)
            {
                if (slots[i].Item == item)
                {
                    SelectSlot(slots[i]);
                    break;
                }
            }
        }

        // SelectSlot
        private void SelectSlot(InventorySlot slot)
        {
            if (slot == selectedSlot)
                return;

            selectedSlot?.Unselect(IsContentLoaded);
            selectedSlot = slot;
            slot.Select(IsContentLoaded);

            if (slot?.Item != null)
            {
                itemNameText.Text = slot.Item.DisplayText;
                menu.Position = slot.BoundingBox.GetPoint(RectanglePoint.Top, -menu.BoundingBox.Width / 2, -(menu.BoundingBox.Height+2));
                itemNameText.Position = menu.BoundingBox.GetPoint(RectanglePoint.LeftTop, -4, 1);
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!IsCurrentScene || Owner.Session.IsOutcomeInProgress)
                return;

            // Gradient
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            bottomGradient.Draw(gameTime);
            inventoryCategoryContainer.Draw(gameTime);
            categoryText.Draw(gameTime);
            itemNameText.Draw(gameTime);
            Game.SpriteBatch.End();

            for (int i = 0; i < Owner.InventorySize; i++)
            {
                slots[i].Draw(gameTime);
            }

            if (selectedSlot?.Item != null)
                menu.Draw(gameTime);

            //buttonClose.Draw(gameTime);
            previousCategory.Draw(gameTime);
            nextCategory.Draw(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (menu.HandleInput(gameTime) == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.Mouse)
            {
                if (HandleMouseInput())
                    return HandleInputResult.Handled;
            }

            // Close
            if (buttonClose.TestPressed(PlayerIndex.One))
            {
                SceneController.Pop();
            }

            if (selectedSlot?.Item == null)
                return HandleInputResult.Unhandled;

            // Select
            /*
            if (buttonSelect.TestPressed(PlayerIndex.One))
            {
                originalSelectedItem = selectedSlot.Item;
                SceneController.Pop();
            }
            */

            // Previous item
            else if (InputBindings.SelectLeft.IsPressed(PlayerIndex.One) || stick.IsLeft(PlayerIndex.One))
            {
                if (SelectPreviousItem())
                    Sound.Play(SoundNames.UINavigation);
            }

            // Next item
            else if (InputBindings.SelectRight.IsPressed(PlayerIndex.One) || stick.IsRight(PlayerIndex.One))
            {
                if (SelectNextItem())
                    Sound.Play(SoundNames.UINavigation);
            }

            /*
            // Sacrifice
            else if (selectedSlot?.Item is Item item)
            {
                if (buttonSacrifice.TestPressed(PlayerIndex.One))
                {
                    Sacrifice(item);
                }

                else if (buttonInfo.TestPressed(PlayerIndex.One))
                {

                }
            }
            */

            return HandleInputResult.Handled;
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            Owner.Stand();

            originalSelectedItem = Owner.Inventory.SelectedItem;

            base.OnLoadContent();

            LayoutSlots();

            Populate();
        }

        // OnUnloadContent
        protected override void OnUnloadContent()
        {
            base.OnUnloadContent();

            for (var i = 0; i < slots.Length; i++)
            {
                slots[i].Reset();
            }

            if (originalSelectedItem != null)
                Owner.Inventory.Select(originalSelectedItem);

            selectedSlot = null;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            bottomGradient.Update(gameTime);
            stick.Stick = GamePadThumbStick.Left;
            stick.Update(gameTime);
            buttonClose.Update(gameTime);
            menu.Update(gameTime);
            nextCategory.Update(gameTime);  
            previousCategory.Update(gameTime);

            // Update slots
            for (int i = 0; i < Owner.Inventory.Items.Count; i++)
            {
                slots[i].Update(gameTime);
            }

            if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.Mouse)
                MouseCursor.Instance.State = GetSlotAt(InputManager.DefaultPlayer.Mouse.VirtualPosition)?.Item != null ? MouseCursorState.CrossOn : MouseCursorState.Cross;

            base.OnUpdate(gameTime);
        }

        // Populate
        private void Populate()
        {
            categoryText.Text = Localization.GetValue(activeCategory);

            itemNameText.Clear();

            for (var i = 0; i < slots.Length; i++)
            {
                slots[i].Item = null;
            }

            selectedSlot = null;

            var selectedIndex = -1;
            for (int i = 0; i < Owner.Inventory.Items.Count; i++)
            {
                slots[i].Item = Owner.Inventory.Items[i];
                if (Owner.Inventory.Items[i].IsSelected)
                    selectedIndex = Owner.Inventory.Items[i].Index;
            }

            if (selectedIndex != -1)
                SelectSlot(slots[selectedIndex]);
        }

        #endregion

        // Owner
        public Actor Owner { get; set; }
    }
}
