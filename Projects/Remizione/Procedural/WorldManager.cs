using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// WorldManager
    /// </summary>
    public class WorldManager
    {
        #region Private fields

        private readonly List<WorldBlock> blockList = [];
        private readonly Dictionary<Point, WorldBlock> blocks = [];
        private int updateCount;

        #endregion

        #region Constructor

        // Constructor
        public WorldManager(GameSession session, Size blockSize, int gridSize)
        {
            this.Session = session;
            this.Blocks = new(blockList);
            this.BlockSize = blockSize;
            this.GridSize = gridSize;
        }

        #endregion

        #region Private members

        // IsValidPosition
        private bool IsValidPosition(Point pos)
        {
            return pos.X >= 0 && pos.X < GridSize && pos.Y >= 0 && pos.Y < GridSize;
        }

        // MergeRectangles
        private static List<Vector2> MergeRectangles(List<Rectangle> rectangles)
        {
            // Step 1: Collect unique x and y coordinates
            var xSet = new HashSet<float>();
            var ySet = new HashSet<float>();

            for (var i = 0; i < rectangles.Count; i++)
            {
                var rect = rectangles[i];

                xSet.Add(rect.Left);
                xSet.Add(rect.Right);
                ySet.Add(rect.Top);
                ySet.Add(rect.Bottom);
            }

            // Convert to sorted lists without LINQ
            var xArray = new float[xSet.Count];
            var yArray = new float[ySet.Count];
            var index = 0;

            foreach (var x in xSet) xArray[index++] = x;
            {
                index = 0;
            }

            foreach (var y in ySet)
            {
                yArray[index++] = y;
            }

            Array.Sort(xArray);
            Array.Sort(yArray);

            var xList = new List<float>(xArray);
            var yList = new List<float>(yArray);

            var m = xList.Count;
            var n = yList.Count;

            if (m < 2 || n < 2)
                return []; // No area covered

            // Step 2: Create covered array
            var covered = new bool[m - 1, n - 1];
            for (var r = 0; r < rectangles.Count; r++)
            {
                var rect = rectangles[r];

                // Binary search for i_start and i_end
                int i_start = 0, i_left = 0, i_right = m - 1;
                while (i_left <= i_right)
                {
                    var mid = (i_left + i_right) / 2;
                    if (xList[mid] >= rect.Left)
                    {
                        i_start = mid; i_right = mid - 1;
                    }
                    else
                        i_left = mid + 1;
                }

                var i_end = m - 1;
                i_left = 0; i_right = m - 1;
                while (i_left <= i_right)
                {
                    var mid = (i_left + i_right) / 2;
                    if (xList[mid] < rect.Right)
                    {
                        i_end = mid;
                        i_left = mid + 1;
                    }
                    else
                        i_right = mid - 1;
                }

                // Binary search for j_start and j_end
                int j_start = 0, j_left = 0, j_right = n - 1;
                while (j_left <= j_right)
                {
                    var mid = (j_left + j_right) / 2;
                    if (yList[mid] >= rect.Top)
                    { j_start = mid; j_right = mid - 1; }
                    else
                        j_left = mid + 1;
                }

                var j_end = n - 1;
                j_left = 0; j_right = n - 1;
                while (j_left <= j_right)
                {
                    var mid = (j_left + j_right) / 2;
                    if (yList[mid] < rect.Bottom)
                    { j_end = mid; j_left = mid + 1; }
                    else
                        j_right = mid - 1;
                }

                for (var i = i_start; i <= i_end && i < m - 1; i++)
                {
                    for (var j = j_start; j <= j_end && j < n - 1; j++)
                    {
                        covered[i, j] = true;
                    }
                }
            }

            // Step 3: Collect boundary segments
            var segments = new List<(Vector2, Vector2)>();
            for (var j = 0; j < n; j++)
            {
                for (var i = 0; i < m - 1; i++)
                {
                    var above_covered = j > 0 && covered[i, j - 1];
                    var below_covered = j < n - 1 && covered[i, j];

                    if (above_covered != below_covered)
                    {
                        Vector2 p1 = new(xList[i], yList[j]);
                        Vector2 p2 = new(xList[i + 1], yList[j]);
                        segments.Add((p1, p2));
                    }
                }
            }
            for (var i = 0; i < m; i++)
            {
                for (var j = 0; j < n - 1; j++)
                {
                    var left_covered = i > 0 && covered[i - 1, j];
                    var right_covered = i < m - 1 && covered[i, j];

                    if (left_covered != right_covered)
                    {
                        var p1 = new Vector2(xList[i], yList[j]);
                        var p2 = new Vector2(xList[i], yList[j + 1]);
                        segments.Add((p1, p2));
                    }
                }
            }

            // Step 4: Build adjacency list
            var adj = new Dictionary<Vector2, List<Vector2>>();
            for (var s = 0; s < segments.Count; s++)
            {
                var p1 = segments[s].Item1;
                var p2 = segments[s].Item2;

                if (!adj.ContainsKey(p1))
                    adj[p1] = [];

                if (!adj.ContainsKey(p2))
                    adj[p2] = [];

                adj[p1].Add(p2);
                adj[p2].Add(p1);
            }

            // Step 5: Trace the polygon
            if (adj.Count == 0)
                return [];

            Vector2 start = new(float.MaxValue, float.MaxValue);
            foreach (var p in adj.Keys)
            {
                if (p.Y < start.Y || (p.Y == start.Y && p.X < start.X))
                    start = p;
            }

            var polygon = new List<Vector2>();
            var current = start;
            Vector2? previous = null;
            do
            {
                polygon.Add(current);
                var neighbors = adj[current];
                var next = neighbors[0];
                if (neighbors.Count > 1 && previous.HasValue && neighbors[0] == previous.Value)
                    next = neighbors[1];
                previous = current;
                current = next;
            } while (current != start);

            return polygon;
        }

        #endregion

        // AddBlock
        public WorldBlock AddBlock(Point gridPosition, int worldVersion, bool populate = true)
        {
            if (updateCount == 0)
                throw new InvalidOperationException("WorldManager is not in update mode.");

            if (!IsValidPosition(gridPosition))
                throw new InvalidOperationException("Grid position is out of bounds.");

            if (blocks.ContainsKey(gridPosition))
                throw new InvalidOperationException("Grid position is already used.");

            var block = new WorldBlock(this, gridPosition, worldVersion, populate);
            blocks[gridPosition] = block;
            blockList.Add(block);

            return block;
        }

        // BeginUpdate
        public void BeginUpdate() => updateCount++;

        // Blocks
        public ReadOnlyCollection<WorldBlock> Blocks { get; }

        // BlockSize
        public Size BlockSize { get; }

        // EndUpdate
        public void EndUpdate()
        {
            if (updateCount > 0)
            {
                updateCount--;

                if (updateCount == 0)
                {
                    for (var i = 0; i < blocks.Count; i++)
                    {
                        blockList[i].Invalidate();
                    }
                }
            }
        }

        // GetBlockFromGrid
        public WorldBlock? GetBlockFromGrid(Point gridPosition)
        {
            blocks.TryGetValue(gridPosition, out var block);
            return block;
        }

        // GetBlockFromScreen
        public WorldBlock? GetBlockFromScreen(Vector2 screenPosition)
        {
            var gridPosition = new Point((int)(screenPosition.X / BlockSize.Width), (int)(screenPosition.Y / BlockSize.Height));
            blocks.TryGetValue(gridPosition, out var block);
            return block;
        }

        // GetWalkareaVertices
        public Vector2[] GetWalkareaVertices()
        {
            var rects = new List<Rectangle>();
            for (var i = 0; i < blocks.Count; i++)
            {
                rects.Add(Blocks[i].BoundingBox.ToRectangle());
            }

            var result = MergeRectangles(rects);
            return result.ToArray();
        }

        // GridSize
        public int GridSize { get; }

        // Session
        public GameSession Session { get; }
    }
}
