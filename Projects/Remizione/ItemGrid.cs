using Adberration;
using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// InventoryGrid
    /// </summary>
    public sealed class ItemGrid : GameObject, IInputHandler
    {
        #region Private fields

        const int slotSize = 20;

        private ItemCategory categoryFilter;
        private readonly int columns;
        private Vector2 position;
        private readonly int rows;
        private int selectedSlotIndex;
        private readonly List<ItemGridSlot> slots;
        private readonly StickInputController stick = new(GamePadThumbStick.Left) { AutoRepeatRate = 100 };

        #endregion

        #region Constructor

        // Constructor
        public ItemGrid(PilgrimSack pilgrimSack, ItemCategory categoryFilter, int columns, int rows)
            : base(pilgrimSack.Session.Game)
        {
            this.PilgrimSack = pilgrimSack;
            this.columns = columns;
            this.rows = rows;
            this.slots = new List<ItemGridSlot>(columns * rows);

            for (int i = 0; i < columns * rows; i++)
            {
                slots.Add(new ItemGridSlot(this));
            }

            this.CategoryFilter = categoryFilter;
            
            Layout();
        }

        #endregion

        #region Private members

        // Layout
        private void Layout()
        {
            var pos = Position;

            var index = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    slots[index].Position = pos;
                    pos.X += slotSize;
                    index++;
                }

                pos.X = Position.X;
                pos.Y += slotSize;
            }

            BoundingBox = new(slots[0].BoundingBox.Left, slots[0].BoundingBox.Top, columns * slotSize, rows * slotSize);
        }

        // Move
        private bool Move(Direction direction)
        {
            int index, col, row;
            switch (direction)
            {
                case Direction.Down:
                    row = SelectedSlotIndex / columns;
                    row = (row + 1) % rows; // Movimiento cíclico vertical
                    index = row * columns + (SelectedSlotIndex % columns);
                    break;

                case Direction.Left:
                    col = SelectedSlotIndex % columns;
                    col = (col - 1 + columns) % columns; // Movimiento cíclico horizontal
                    index = (SelectedSlotIndex / columns) * columns + col;
                    break;

                case Direction.Right:
                    col = SelectedSlotIndex % columns;
                    col = (col + 1) % columns; // Movimiento cíclico horizontal
                    index = (SelectedSlotIndex / columns) * columns + col;
                    break;

                default:
                    row = SelectedSlotIndex / columns;
                    row = (row - 1 + rows) % rows; // Movimiento cíclico vertical
                    index = row * columns + (SelectedSlotIndex % columns);
                    break;
            }

            if (slots[index].Item != null)
            {
                SelectedSlotIndex = index;
                Sound.Play(SoundNames.UIHover);
                return true;
            }
            else
                return false;
        }

        // Populate
        private void Populate(ItemCategory category)
        {
            Clear();

            int index = 0;

            foreach (var item in PilgrimSack.GetItems(category))
            {
                if (index >= slots.Count)
                    break;

                slots[index].Item = item;
                index++;
            }

            if (PilgrimSack.SelectedItem != null)
                SelectSlot(PilgrimSack.SelectedItem.Name);
            else
                SelectSlot(0);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            stick.Update(gameTime);

            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].Update(gameTime);
            }
        }

        #endregion

        // BoundingBox
        public RectangleF BoundingBox { get; private set; }

        // CategoryFilter
        public ItemCategory CategoryFilter
        {
            get => categoryFilter;
            set
            {
                categoryFilter = value;
                Populate(categoryFilter);
            }
        }

        // Clear
        public void Clear()
        {
            for (var i = 0; i < slots.Count; i++)
            {
                slots[i].Item = null;
            }
        }

        // DiscardSelectedItem
        public void DiscardSelectedItem()
        {
            if (SelectedItem is Item item)
            {
                SelectedSlot.Item = null;
                item.Discard();
                if (PilgrimSack.SelectedItem != null)
                    SelectSlot(PilgrimSack.SelectedItem.Name);
            }
        }

        // GetSlot
        public ItemGridSlot? GetSlot(Item item)
        {
            for (var i = 0; i < slots.Count; i++)
            {
                if (slots[i].Item == item)
                    return slots[i];
            }

            return null;
        }

        // GetSlotAt
        public ItemGridSlot? GetSlotAt(Vector2 position) => GetSlotAt((int)position.X, (int)position.Y);

        // GetSlotAt
        public ItemGridSlot? GetSlotAt(int x, int y)
        {
            if (!BoundingBox.Contains(x, y))
                return null;

            int col = (x - (int)Position.X) / slotSize;
            int row = (y - (int)Position.Y) / slotSize;

            if (row >= rows)
                return null;

            int index = row * columns + col;

            return slots[index];
        }

        // GetSlotPosition
        public Vector2 GetSlotPosition(int index)
        {
            int row = index / columns;
            int col = index % columns;

            return new Vector2(Position.X + col * slotSize,
                               Position.Y + row * slotSize);
        }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (HandleMouseInput())
                return HandleInputResult.Handled;

            // Move up
            if (InputBindings.SelectUp.IsPressed(0) || stick.IsUp(PlayerIndex.One))
            {
                Move(Direction.Up);
                return HandleInputResult.Handled;
            }

            // Move left
            else if (InputBindings.SelectLeft.IsPressed(0) || stick.IsLeft(PlayerIndex.One))
            {
                Move(Direction.Left);
                return HandleInputResult.Handled;
            }

            // Move down
            if (InputBindings.SelectDown.IsPressed(0) || stick.IsDown(PlayerIndex.One))
            {
                Move(Direction.Down);
                return HandleInputResult.Handled;
            }

            // Move right
            if (InputBindings.SelectRight.IsPressed(0) || stick.IsRight(PlayerIndex.One))
            {
                Move(Direction.Right);
                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // HandleMouseInput
        public bool HandleMouseInput()
        {
            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
            {
                if (GetSlotAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) is ItemGridSlot slot)
                {
                    MouseCursor.Instance.AnimateClick();

                    if (slot.Item == null)
                    {
                        Sound.Play(SoundNames.Error);
                    }
                    else if (slot != SelectedSlot)
                    {
                        SelectSlot(slot);
                        Sound.Play(SoundNames.UIHover);
                        return true;
                    }
                }
            }

            return false;
        }

        // IndexOf
        public int IndexOf(ItemGridSlot slot) => slots.IndexOf(slot);

        // PilgrimSack
        public PilgrimSack PilgrimSack { get; }

        // Position
        public Vector2 Position
        {
            get => position;
            set
            {
                if (value != position)
                {
                    position = value;
                    Layout();
                }
            }
        }

        // SelectSlot
        public void SelectSlot(int index)
        {
            if (index >= 0 && index < slots.Count)
                SelectedSlotIndex = index;
        }

        // SelectSlot
        public void SelectSlot(Vector2 position)
        {
            if (GetSlotAt(position) is ItemGridSlot slot)
                SelectSlot(slot);
        }

        // SelectSlot
        public bool SelectSlot(string itemName)
        {
            for (var i = 0; i < slots.Count; i++)
            {
                if (slots[i].Item?.Name == itemName)
                {
                    SelectSlot(i);
                    return true;
                }
            }

            return false;
        }

        // SelectSlot
        public bool SelectSlot(ItemGridSlot slot)
        {
            for (var i = 0; i < slots.Count; i++)
            {
                if (slots[i] == slot)
                {
                    SelectSlot(i);
                    return true;
                }
            }

            return false;
        }

        // SelectedItem
        public Item? SelectedItem => SelectedSlot?.Item;

        // SelectedSlot
        public ItemGridSlot SelectedSlot => slots[selectedSlotIndex];

        // SelectedSlotIndex
        public int SelectedSlotIndex
        {
            get => selectedSlotIndex;
            set
            {
                SelectedSlot?.Deactivate();
                selectedSlotIndex = value;
                SelectedSlot?.Activate();
            }
        }
    }
}
