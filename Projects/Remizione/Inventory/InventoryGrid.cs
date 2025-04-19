using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Remizione.Inventory
{
    /// <summary>
    /// InventoryGrid
    /// </summary>
    internal sealed class InventoryGrid : GameObject, IInputHandler
    {
        const int slotSize = 15;

        private readonly int columns;
        private int currentRowOffset = 0;
        private readonly ImageSprite selectionImage;
        private int selectedSlotIndex;
        private readonly List<InventorySlot> slots;
        private readonly int totalRows;
        private readonly int visibleRows;
        private readonly InventorySlot[] visibleSlots;

        // Constructor
        public InventoryGrid(EngendroGame game, int columns, int totalRows, int visibleRows)
            : base(game)
        {
            this.columns = columns;
            this.totalRows = totalRows;
            this.visibleRows = visibleRows;
            this.slots = new List<InventorySlot>(columns * totalRows);

            // Selection image
            this.selectionImage = new ImageSprite(Game, Atlases.UI.InventorySlotSelection)
            {
                Color = ColorPalette.Text.Dark,
                Opacity = .8f,
                Scale = new(.75f)
            };
            this.selectionImage.Tweens.OpacityTween = FloatTween.Create(TweenStyle.CubicInOut, .4f, .8f, 1000, -1);

            for (int i = 0; i < columns * totalRows; i++)
            {
                slots.Add(new InventorySlot(game));
            }

            visibleSlots = new InventorySlot[columns * visibleRows];
        }

        // InvalidateVisibleSlots
        private void InvalidateVisibleSlots()
        {
            int startIndex = currentRowOffset * columns;
            int endIndex = startIndex + (visibleRows * columns);
            int outputIndex = 0;

            for (int i = startIndex; i < endIndex; i++)
            {
                visibleSlots[outputIndex++] = slots[i];
            }
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            for (int i = 0; i < visibleSlots.Length; i++)
            {
                visibleSlots[i].Position = GetSlotPosition(i);
                visibleSlots[i].Draw(gameTime);

                if (selectedSlotIndex == i)
                {
                    Game.SpriteBatch.Begin(Game.Camera);
                    selectionImage.Position = SelectedSlot.Position;
                    selectionImage.Draw(gameTime);
                    Game.SpriteBatch.End();
                }
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            selectionImage.Update(gameTime);
        }

        #endregion

        // Bounds
        public RectangleF Bounds => new(Position.X, Position.Y, columns * slotSize, visibleRows * slotSize);

        // CanHandleInput
        public bool CanHandleInput => true;

        // Clear
        public void Clear()
        {
            for (var i = 0; i < slots.Count; i++)
            {
                slots[i].Item = null;
            }
        }

        // Fill
        public void Fill(ItemStorage storage)
        {
            Clear();

            int index = 0;
            foreach (var item in storage.Items)
            {
                if (index >= slots.Count)
                    break;

                slots[index].Item = item;
                index++;
            }

            SelectSlot(storage.SelectedIndex);
            InvalidateVisibleSlots();
        }

        // GetSlotAt
        public InventorySlot? GetSlotAt(Vector2 position) => GetSlotAt((int)position.X, (int)position.Y);

        // GetSlotAt
        public InventorySlot? GetSlotAt(int x, int y)
        {
            if (!Bounds.Contains(x, y))
                return null;

            int col = (x - (int)Position.X) / slotSize;
            int row = (y - (int)Position.Y) / slotSize + currentRowOffset;

            if (row >= totalRows)
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
                               Position.Y + (row - currentRowOffset) * slotSize);
        }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
            {
                if (GetSlotAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) is InventorySlot slot)
                {
                    if (slot != SelectedSlot)
                    {
                        SelectSlot(slot);
                        return HandleInputResult.Handled;
                    }
                }
            }

            return HandleInputResult.Unhandled;
        }

        // MoveDown
        public void MoveDown()
        {
            int row = SelectedSlotIndex / columns;
            row = (row + 1) % totalRows; // Movimiento cíclico vertical
            SelectedSlotIndex = row * columns + (SelectedSlotIndex % columns);
        }

        // MoveLeft
        public void MoveLeft()
        {
            int col = SelectedSlotIndex % columns;
            col = (col - 1 + columns) % columns; // Movimiento cíclico horizontal
            SelectedSlotIndex = (SelectedSlotIndex / columns) * columns + col;
        }

        // MoveRight
        public void MoveRight()
        {
            int col = SelectedSlotIndex % columns;
            col = (col + 1) % columns; // Movimiento cíclico horizontal
            SelectedSlotIndex = (SelectedSlotIndex / columns) * columns + col;
        }

        // MoveUp
        public void MoveUp()
        {
            int row = SelectedSlotIndex / columns;
            row = (row - 1 + totalRows) % totalRows; // Movimiento cíclico vertical
            SelectedSlotIndex = row * columns + (SelectedSlotIndex % columns);
        }

        // Position
        public Vector2 Position { get; set; }

        // Scroll
        public void Scroll(int direction)
        {
            currentRowOffset += direction;
            if (currentRowOffset < 0) currentRowOffset = 0;
            if (currentRowOffset > totalRows - visibleRows)
                currentRowOffset = totalRows - visibleRows;

            // Asegurar que el slot activo sigue siendo visible
            int firstVisibleIndex = currentRowOffset * columns;
            int lastVisibleIndex = firstVisibleIndex + (visibleRows * columns) - 1;

            if (SelectedSlotIndex < firstVisibleIndex)
                SelectedSlotIndex = firstVisibleIndex;

            if (SelectedSlotIndex > lastVisibleIndex)
                SelectedSlotIndex = lastVisibleIndex;
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
            if (GetSlotAt(position) is InventorySlot slot)
                SelectSlot(slot);
        }

        // SelectSlot
        public bool SelectSlot(InventorySlot slot)
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

        // SelectedSlot
        public InventorySlot SelectedSlot => slots[selectedSlotIndex];

        // SelectedSlotIndex
        public int SelectedSlotIndex
        {
            get => selectedSlotIndex;
            set
            {
                if (value != selectedSlotIndex)
                {
                    if (SelectedSlot?.Item is Item item)
                        item.Unread = false;

                    selectedSlotIndex = value;
                    InvalidateVisibleSlots();
                }
            }
        }
    }
}
