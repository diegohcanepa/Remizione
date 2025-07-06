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

        private readonly ImageSprite bottomGradient;
        private readonly UITextButton buttonClose;
        private readonly UITextButton buttonInfo;
        private readonly UITextButton buttonSacrifice;
        private readonly UITextButton buttonSelect;
        private readonly TextSprite itemNameText;
        private Item? originalSelectedItem;
        private InventorySlot? selectedSlot;
        private readonly InventorySlot[] slots = new InventorySlot[12];
        private readonly UIDerivedStatModifier statModifier;
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
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, 0, -4),
                Small = true,
            };

            // Info button
            buttonInfo = new UITextButton(Game, InputBindings.Info)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = buttonClose.BoundingBox.GetPoint(RectanglePoint.RightTop, 0, -2),
                Small = true,
            };

            // Item name
            itemNameText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Bottom,
                Position = new(Screen.NativeWidth / 2, 110),
                Scale = ScaleInfo.Text.VeryLarge
            };

            // Sacrifice button
            buttonSacrifice = new UITextButton(owner.Game, InputBindings.Sacrifice)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = buttonInfo.BoundingBox.GetPoint(RectanglePoint.RightTop, 0, -2),
                Small = true,
            };

            // Select button
            buttonSelect = new UITextButton(owner.Game, InputBindings.Select)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = buttonSacrifice.BoundingBox.GetPoint(RectanglePoint.RightTop, 0, -2),
                Small = true,
            };

            // Bottom gradient
            bottomGradient = new ImageSprite(owner.Game, Atlases.UI.BottomGradient)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.Area.GetPoint(RectanglePoint.Bottom),
                Scale = new Vector2(1, 1.2f)
            };

            // Stat icon
            this.statModifier = new(owner.Game, DerivedStat.HP)
            {
                IsBonus = true,
                PivotOrigin = RectanglePoint.Right,
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
                slots[i].Position = new(x, 120);
            }
        }

        // Sacrifice
        private void Sacrifice(Item item)
        {
            if (item.MetaItem.SacrificeReward == DerivedStat.HP)
                Owner.HP += item.MetaItem.SacrificeRewardAmount;

            else if (item.MetaItem.SacrificeReward == DerivedStat.Tickets)
                Owner.Tickets += item.MetaItem.SacrificeRewardAmount;

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
                statModifier.Stat = slot.Item.MetaItem.SacrificeReward;
                statModifier.Amount = slot.Item.MetaItem.SacrificeRewardAmount;
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
            itemNameText.Draw(gameTime);
            Game.SpriteBatch.End();

            for (int i = 0; i < Owner.InventorySize; i++)
            {
                slots[i].Draw(gameTime);
            }

            if (selectedSlot?.Item != null)
            {
                buttonInfo.Draw(gameTime);
                buttonSacrifice.Draw(gameTime);
                buttonSelect.Draw(gameTime);
                statModifier.Position = buttonSacrifice.BoundingBox.GetPoint(RectanglePoint.Left);
                statModifier.Draw(gameTime);
            }

            buttonClose.Draw(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
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
            if (buttonSelect.TestPressed(PlayerIndex.One))
            {
                originalSelectedItem = selectedSlot.Item;
                SceneController.Pop();
            }

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
            buttonInfo.Update(gameTime);
            buttonSacrifice.Update(gameTime);
            buttonSelect.Update(gameTime);

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
