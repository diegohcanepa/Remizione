using Engendro;
using Engendro.Input;
using EngendroAdventure;
using EngendroAdventure.Scripting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// InventoryScene
    /// </summary>
    public sealed class InventoryScene : Scene
    {
        #region Constants

        private const int tweenDuration = 300;

        #endregion

        #region Private fields

        private enum OptionKind { None, Verb, UseWith }
        private readonly ImageSprite bottomGradient;
        private readonly UIControl buttonClose;
        private readonly UIControl buttonInfo;
        private float lastKnownAmount;
        private bool moved;
        private readonly OptionKind[] options = new OptionKind[4];
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
            buttonClose = new UIControl(owner.Game, InputBindings.Close)
            {
                AllowContainer = true,
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom),
            };

            // Info button
            buttonInfo = new UIControl(Game, InputBindings.Info)
            {
                AllowContainer = true,
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, 0, -10),
                Text = "Info"
            };

            // Bottom gradient
            bottomGradient = new ImageSprite(owner.Game, Atlases.UI.BottomGradient)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.Area.GetPoint(RectanglePoint.Bottom),
                Scale = new Vector2(1, 1.2f)
            };
        }

        #endregion

        #region Private members

        // LayoutSlots
        private void LayoutSlots()
        {
            const float spacing = 1;
            var maximum = Owner.Inventory.Size;
            var slotSize = slots[0].BoundingBox.Width;
            var totalWidth = (maximum * slotSize) + ((maximum - 1) * spacing);
            var start = (Screen.NativeWidth - totalWidth) / 2;
            start += slots[0].BoundingBox.Width / 2;

            for (int i = 0; i < Owner.Inventory.Size; i++)
            {
                var x = start + (i * (slotSize + spacing));
                slots[i].Position = new(x, 120);
            }
        }

        // SelectNextItem
        private void SelectNextItem()
        {
            Owner.Inventory.SelectNext();
            if (Owner.Inventory.SelectedItem != null)
                SelectSlot(Owner.Inventory.SelectedItem);
        }

        // SelectPreviousItem
        private void SelectPreviousItem()
        {
            Owner.Inventory.SelectPrevious();
            if (Owner.Inventory.SelectedItem != null)
                SelectSlot(Owner.Inventory.SelectedItem);
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
            Game.SpriteBatch.End();

            for (int i = 0; i < Owner.Inventory.Size; i++)
            {
                slots[i].Draw(gameTime);
            }

            buttonClose.Draw(gameTime);
            buttonInfo.Draw(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            //if (contextMenu.HandleInput(gameTime) == HandleInputResult.Handled)
                //return HandleInputResult.Handled;

            // Previous item
            if (InputBindings.SelectLeft.IsPressed(PlayerIndex.One) || stick.IsLeft(PlayerIndex.One))
                SelectPreviousItem();

            // Next item
            else if (InputBindings.SelectRight.IsPressed(PlayerIndex.One) || stick.IsRight(PlayerIndex.One))
                SelectNextItem();

            /*
            // Menu Option
            else if (InputBindings.Select.IsPressed(VladInputHelper.PlayerIndex))
                PerformMenuOption();

            // Diary
            else if (buttonDiary.TestPressed(VladInputHelper.PlayerIndex))
            {
                var scene = new DiaryScene(Owner.Session);
                Game.SceneManager.Push(scene);
            }
            */

            // Close
            else if (InputBindings.Close.IsPressed(PlayerIndex.One))
            {
                //SoundManager.Play(SoundNames.InventoryClose.ToString());
                SceneController.Pop();
            }

            return HandleInputResult.Handled;
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            Owner.Stand();

            base.OnLoadContent();
            
            LayoutSlots();

            // Populate items
            var selectedIndex = -1;
            for (int i = 0; i < Owner.Inventory.Items.Count; i++)
            {
                slots[i].Item = Owner.Inventory.Items[i];
                if (Owner.Inventory.Items[i].IsSelected)
                    selectedIndex = Owner.Inventory.Items[i].Index;
            }

            if (selectedIndex != -1)
                SelectSlot(slots[selectedIndex]);

            Invalidate();

            //SoundManager.Play(SoundNames.InventoryOpen.ToString());
        }

        // OnUnloadContent
        protected override void OnUnloadContent()
        {
            base.OnUnloadContent();

            for (var i = 0; i < slots.Length; i++)
            {
                slots[i].Reset();
            }

            selectedSlot = null;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            //contextMenu.Update(gameTime);
            bottomGradient.Update(gameTime);
            stick.Stick = GamePadThumbStick.Left;
            stick.Update(gameTime);
            buttonClose.Update(gameTime);
            buttonInfo.Update(gameTime);

            // Update slots
            for (int i = 0; i < Owner.Inventory.Items.Count; i++)
            {
                slots[i].Update(gameTime);
            }

            base.OnUpdate(gameTime);
        }

        #endregion

        // CanHandleInput
        //public override bool CanHandleInput => !Owner.Session.IsOutcomeInProgress && base.CanHandleInput;

        // Close
        public void Close()
        {
            //SoundManager.Play(SoundNames.InventoryClose.ToString());
            SceneController.Pop();
        }

        // Owner
        public Actor Owner { get; set; }
    }
}
