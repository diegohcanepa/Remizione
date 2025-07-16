using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// InventoryGrid
    /// </summary>
    internal sealed class InventoryGrid : GameObject, IInputHandler
    {
        const int slotSize = 20;

        private readonly int columns;
        private Vector2 position;
        private readonly int rows;
        private readonly ImageSprite selectionImage;
        private int selectedSlotIndex;
        private readonly List<InventorySlot> slots;

        // Constructor
        public InventoryGrid(Inventory inventory, int columns, int rows)
            : base(inventory.Owner.Game)
        {
            this.Inventory = inventory;
            this.columns = columns;
            this.rows = rows;
            this.slots = new List<InventorySlot>(columns * rows);

            // Selection image
            this.selectionImage = new ImageSprite(Game, null)//Atlases.UI.InventorySlotSelection)
            {
                Color = ColorPalette.Text.Dark,
                Opacity = .8f,
                Scale = new(.75f)
            };
            this.selectionImage.Tweens.OpacityTween = FloatTween.Create(TweenStyle.CubicInOut, .4f, .8f, 1000, -1);

            for (int i = 0; i < columns * rows; i++)
            {
                slots.Add(new InventorySlot(inventory.Owner.Game));
            }

            Layout();
        }

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

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].Position = GetSlotPosition(i);
                slots[i].Draw(gameTime);

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

        // BoundingBox
        public RectangleF BoundingBox { get; private set; }

        // Clear
        public void Clear()
        {
            for (var i = 0; i < slots.Count; i++)
            {
                slots[i].Item = null;
            }
        }

        // Fill
        public void Fill(MetaItemCategory category)
        {
            Clear();

            int index = 0;
            
            foreach (var item in Inventory.GetItems(category))
            {
                if (index >= slots.Count)
                    break;

                slots[index].Item = item;
                index++;
            }

            SelectSlot(0);
        }

        // GetSlotAt
        public InventorySlot? GetSlotAt(Vector2 position) => GetSlotAt((int)position.X, (int)position.Y);

        // GetSlotAt
        public InventorySlot? GetSlotAt(int x, int y)
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
            row = (row + 1) % rows; // Movimiento cíclico vertical
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
            row = (row - 1 + rows) % rows; // Movimiento cíclico vertical
            SelectedSlotIndex = row * columns + (SelectedSlotIndex % columns);
        }

        // Inventory
        public Inventory Inventory { get; }

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
                }
            }
        }
    }
}
