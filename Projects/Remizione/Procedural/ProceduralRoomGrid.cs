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
        private const float usagePercent = .8f;

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
        public int CellSize { get; private set; } = 14;

        // ColCount
        public int ColCount { get; private set; }

        // GetPixelArea
        public RectangleF GetPixelArea(GameThing thing)
        {
            if (thing.Collider.IsEmpty)
                return thing.BoundingBox;
            else
                return thing.Collider.BoundingRectangleF;
        }

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
            var bbox = GetPixelArea(thing);
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
        public void MarkOccupied(int startCol, int startRow, Size size)
        {
            for (int x = startCol; x < startCol + size.Width; x++)
            {
                for (int y = startRow; y < startRow + size.Height; y++)
                {
                    if (x >= 0 && x < ColCount && y >= 0 && y < RowCount)
                        occupied[x, y] = true;
                }
            }
        }

        // Name
        public string Name { get; }

        // ReserveSpace
        public bool ReserveSpace(GameThing thing)
        {
            return ReserveSpace(GetPixelArea(thing).ToRectangle());
        }

        // ReserveSpace
        public bool ReserveSpace(Rectangle pixelArea)
        {
            int startCol = (pixelArea.X / CellSize);
            int startRow = (pixelArea.Y / CellSize);
            int cellWidth = (int)Math.Ceiling(pixelArea.Width / (float)CellSize);
            int cellHeight = (int)Math.Ceiling(pixelArea.Height / (float)CellSize);

            Size size = new(cellWidth, cellHeight);

            if (CanFitAt(startCol, startRow, size))
            {
                MarkOccupied(startCol, startRow, size);
                return true;
            }

            return false;
        }

        // Resize
        public void Resize(int width, int height)
        {
            // Maximum cells (without margin)
            int totalColumns = (width + CellSize - 1) / CellSize;
            int totalRows = (height + CellSize - 1) / CellSize;

            // Maximum cells (with margin)
            ColCount = (int)(totalColumns * usagePercent);
            RowCount = (int)(totalRows * usagePercent);

            occupied = new bool[ColCount, RowCount];
        }

        // RowCount
        public int RowCount { get; private set; }

        // ToString
        public override string ToString() => Name;

        // TryReserveSpace
        public bool TryReserveSpace(Size required, out int col, out int row)
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
                        MarkOccupied(x, y, required);
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
        public bool TryReserveSpace(Size required, out int col, out int row, int suggestedCol, int suggestedRow)
        {
            // Try suggested cell
            if (CanFitAt(suggestedCol, suggestedRow, required))
            {
                MarkOccupied(suggestedCol, suggestedRow, required);
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
