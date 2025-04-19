using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// WorldBlockGrid
    /// </summary>
    public sealed class WorldBlockGrid
    {
        private readonly int columns;
        private readonly bool[,] occupied;
        private readonly int rows;
        private const float usagePercent = .9f;

        // Constructor
        public WorldBlockGrid(WorldBlock block)
        {
            // Maximum cells (without margin)
            int totalColumns = ((int)block.BoundingBox.Width + CellSize - 1) / CellSize;
            int totalRows = ((int)block.BoundingBox.Height + CellSize - 1) / CellSize;

            // Maximum cells (with margin)
            columns = (int)(totalColumns * usagePercent);
            rows = (int)(totalRows * usagePercent);

            // Margin
            OffsetX = (totalColumns - columns) / 2;
            OffsetY = (totalRows - rows) / 2;

            occupied = new bool[columns, rows];
        }

        // CellSize
        public const int CellSize = 10;

        // GetWorldPosition
        public Point GetWorldPosition(int col, int row)
        {
            int x = (OffsetX + col) * CellSize;
            int y = (OffsetY + row) * CellSize;
            return new Point(x, y);
        }

        // IsCellFree
        public bool IsCellFree(int col, int row)
        {
            return col >= 0 && col < columns && row >= 0 && row < rows && !occupied[col, row];
        }

        // OffsetX
        public int OffsetX { get; }

        // OffsetY 
        public int OffsetY { get; }

        // MarkOccupied
        public void MarkOccupied(int startCol, int startRow, int width, int height)
        {
            for (int x = startCol; x < startCol + width; x++)
            {
                for (int y = startRow; y < startRow + height; y++)
                {
                    if (x >= 0 && x < columns && y >= 0 && y < rows)
                        occupied[x, y] = true;
                }
            }
        }

        // TryReserveSpace
        public bool TryReserveSpace(Size required, out int col, out int row)
        {
            for (int x = 0; x <= columns - required.Width; x++)
            {
                for (int y = 0; y <= rows - required.Height; y++)
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
                        MarkOccupied(x, y, required.Width, required.Height);
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
    }
}
