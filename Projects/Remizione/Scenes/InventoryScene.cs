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
        private readonly UIControl buttonInfo;
        private readonly UIControl buttonSacrifice;
        private readonly ImageSprite faithIcon;
        private readonly UIControl buttonClose;
        private readonly TextSprite itemNameText;
        private InventorySlot? selectedSlot;
        private readonly ImageSprite spiritIcon;
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
            buttonSacrifice = new UIControl(owner.Game, InputBindings.Sacrifice)
            {
                AllowContainer = true,
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, 0, -20),
            };

            // Bottom gradient
            bottomGradient = new ImageSprite(owner.Game, Atlases.UI.BottomGradient)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.Area.GetPoint(RectanglePoint.Bottom),
                Scale = new Vector2(1, 1.2f)
            };

            // Faith icon
            this.faithIcon = new(owner.Game, Atlases.UI.FaithGainIcon)
            {
                PivotOrigin = RectanglePoint.Right,
                Scale = ScaleInfo.UIElement.Small
            };

            // Spirit icon
            this.spiritIcon = new(owner.Game, Atlases.UI.SpiritGainIcon)
            {
                PivotOrigin = RectanglePoint.Right,
                Scale = ScaleInfo.UIElement.Small
            };
        }

        #endregion

        #region Private members

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
                itemNameText.Text = slot.Item.DisplayText;
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
                buttonClose.Draw(gameTime);
                buttonInfo.Draw(gameTime);
                buttonSacrifice.Draw(gameTime);

                Game.SpriteBatch.Begin(Game.Camera);
                if (selectedSlot.Item.MetaItem.SacrificeReward == SacrificeReward.Faith)
                {
                    faithIcon.Position = buttonSacrifice.BoundingBox.GetPoint(RectanglePoint.Left);
                    faithIcon.Draw(gameTime);
                }
                else
                {
                    spiritIcon.Position = buttonSacrifice.BoundingBox.GetPoint(RectanglePoint.Left);
                    spiritIcon.Draw(gameTime);
                }
                Game.SpriteBatch.End();
            }
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            // Previous item
            if (InputBindings.SelectLeft.IsPressed(PlayerIndex.One) || stick.IsLeft(PlayerIndex.One))
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
            else if (selectedSlot?.Item is Item item && InputBindings.Sacrifice.IsPressed(PlayerIndex.One))
            {
                if (item.MetaItem.SacrificeReward == SacrificeReward.Spirit)
                    Owner.HP += 1;

                else if (item.MetaItem.SacrificeReward == SacrificeReward.Faith)
                    Owner.Faith += 1;

                item.Remove();
                selectedSlot.Item = null;
                Populate();
            }

            // Close
            else if (InputBindings.Close.IsPressed(PlayerIndex.One))
                SceneController.Pop();

            return HandleInputResult.Handled;
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            Owner.Stand();

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

            // Update slots
            for (int i = 0; i < Owner.Inventory.Items.Count; i++)
            {
                slots[i].Update(gameTime);
            }

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
