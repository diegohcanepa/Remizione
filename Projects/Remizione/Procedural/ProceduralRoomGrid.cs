using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// ProceduralRoomGrid
    /// </summary>
    public sealed class ProceduralRoomGrid
    {
        private bool[,] occupied = new bool[0, 0];
        private string[,] occupiedName = new string[0, 0];

        #region Constructor

        // Constructor
        public ProceduralRoomGrid(string name)
            : this(name, 0, 0)
        {
        }

        // Constructor
        public ProceduralRoomGrid(string name, int width, int height)
        {
            this.Name = name;
            Resize(width, height);
        }

        #endregion

        #region Private fields

        // CanFitAt
        private bool CanFitAt(int startCol, int startRow, Size required)
        {
            if (startCol < 0 || startRow < 0 || startCol + required.Width > ColCount || startRow + required.Height > RowCount)
                return false;

            for (int dx = 0; dx < required.Width; dx++)
            {
                for (int dy = 0; dy < required.Height; dy++)
                {
                    if (occupied[startCol + dx, startRow + dy])
                        return false;
                }
            }

            return true;
        }

        #endregion

        // CellSize
        public int CellSize { get; private set; } = 15;

        // ColCount
        public int ColCount { get; private set; }

        // GetCellLabel
        public string GetCellLabel(int col, int row) => occupiedName[col, row];

        // GetPosition
        public Vector2 GetPosition(int col, int row)
        {
            int x = col * CellSize;
            int y = row * CellSize;

            return new(x, y);
        }

        // GetRequiredGridSpace
        public Size GetRequiredGridSpace(GameThing thing)
        {
            var bbox = thing.GetGridPixelArea();
            
            int width = (int)Math.Ceiling(bbox.Width / CellSize) + thing.CellMargin * 2;
            int height = (int)Math.Ceiling(bbox.Height / CellSize) + thing.CellMargin * 2;

            return new Size(width, height);
        }

        // IsCellFree
        public bool IsCellFree(int col, int row)
        {
            return col >= 0 && col < ColCount && row >= 0 && row < RowCount && !occupied[col, row];
        }

        // MarkOccupied
        public void MarkOccupied(string label, int startCol, int startRow, Size size)
        {
            MarkOccupied(label, startCol, startRow, size.Width, size.Height);
        }

        // MarkOccupied
        public void MarkOccupied(string label, int startCol, int startRow, int colCount, int rowCount)
        {
            for (int col = startCol; col < startCol + colCount; col++)
            {
                for (int row = startRow; row < startRow + rowCount; row++)
                {
                    if (col >= 0 && col < ColCount && row >= 0 && row < RowCount)
                    {
                        occupied[col, row] = true;
                        occupiedName[col, row] = label;
                    }
                }
            }
        }

        // MarkOccupiedMargin
        public void MarkOccupiedMargin(string label, int marginLeft, int marginTop, int marginRight, int marginBottom)
        {
            // Top
            if (marginTop > 0)
                MarkOccupied(label, 0, 0, ColCount, marginTop);

            // Bottom
            if (marginBottom > 0)
                MarkOccupied(label, 0, RowCount - marginBottom, ColCount, marginBottom);

            // Left
            if (marginLeft > 0)
                MarkOccupied(label, 0, marginTop, marginLeft, RowCount - marginTop - marginBottom);

            // Right
            if (marginRight > 0)
                MarkOccupied(label, ColCount - marginRight, marginTop, marginRight, RowCount - marginTop - marginBottom);
        }

        // Name
        public string Name { get; }

        // ReserveSpace
        public bool ReserveSpace(GameThing thing)
        {
            return ReserveSpace(thing.StaticName, thing.GetGridPixelArea().ToRectangle());
        }

        // ReserveSpace
        public bool ReserveSpace(string label, Rectangle pixelArea)
        {
            int startCol = (pixelArea.X / CellSize);
            int startRow = (pixelArea.Y / CellSize);
            int cellWidth = (int)Math.Ceiling(pixelArea.Width / (float)CellSize);
            int cellHeight = (int)Math.Ceiling(pixelArea.Height / (float)CellSize);

            Size size = new(cellWidth, cellHeight);

            if (CanFitAt(startCol, startRow, size))
            {
                MarkOccupied(label, startCol, startRow, size);
                return true;
            }

            return false;
        }

        // Resize
        public void Resize(int width, int height)
        {
            ColCount = width / CellSize;
            RowCount = height / CellSize;
            occupied = new bool[ColCount, RowCount];
            occupiedName = new string[ColCount, RowCount];
        }

        // RowCount
        public int RowCount { get; private set; }

        // ToString
        public override string ToString() => Name;

        // TryReserveSpace
        public bool TryReserveSpace(string label, Size required, out int col, out int row)
        {
            for (int x = 0; x <= ColCount - required.Width; x++)
            {
                for (int y = 0; y <= RowCount - required.Height; y++)
                {
                    bool fits = true;

                    for (int dx = 0; dx < required.Width && fits; dx++)
                    {
                        for (int dy = 0; dy < required.Height; dy++)
                        {
                            if (occupied[x + dx, y + dy])
                            {
                                fits = false;
                                break;
                            }
                        }
                    }

                    if (fits)
                    {
                        // Reservar el espacio
                        MarkOccupied(label, x, y, required);
                        col = x;
                        row = y;
                        return true;
                    }
                }
            }

            // No se encontró espacio
            col = -1;
            row = -1;
            return false;
        }

        // TryReserveSpace
        public bool TryReserveSpace(string label, Size required, out int col, out int row, int suggestedCol, int suggestedRow)
        {
            // Try suggested cell
            if (CanFitAt(suggestedCol, suggestedRow, required))
            {
                MarkOccupied(label, suggestedCol, suggestedRow, required);
                col = suggestedCol;
                row = suggestedRow;
                return true;
            }

            // If can't fit in suggested cell, then fail
            col = -1;
            row = -1;
            return false;
        }
    }
}
