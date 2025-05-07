using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// WorldBlockGrid
    /// </summary>
    public sealed class WorldBlockGrid
    {
        private readonly bool[,] occupied;
        private const float usagePercent = .7f;

        #region Constructor

        // Constructor
        public WorldBlockGrid(string name, Size blockSize)
        {
            this.Name = name;

            // Maximum cells (without margin)
            int totalColumns = ((int)blockSize.Width + CellSize - 1) / CellSize;
            int totalRows = ((int)blockSize.Height + CellSize - 1) / CellSize;

            // Maximum cells (with margin)
            ColCount = (int)(totalColumns * usagePercent);
            RowCount = (int)(totalRows * usagePercent);

            // Margin
            OffsetX = (totalColumns - ColCount) / 2;
            OffsetY = (totalRows - RowCount) / 2;

            occupied = new bool[ColCount, RowCount];
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
        public const int CellSize = 20;

        // ColCount
        public int ColCount { get; }

        // GetWorldPosition
        public Vector2 GetWorldPosition(int col, int row)
        {
            int x = (OffsetX + col) * CellSize;
            int y = (OffsetY + row) * CellSize;
            return new(x, y);
        }

        // IsCellFree
        public bool IsCellFree(int col, int row)
        {
            return col >= 0 && col < ColCount && row >= 0 && row < RowCount && !occupied[col, row];
        }

        // OffsetX
        public int OffsetX { get; }

        // OffsetY 
        public int OffsetY { get; }

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

        // RowCount
        public int RowCount { get; }

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
