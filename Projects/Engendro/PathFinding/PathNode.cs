using Microsoft.Xna.Framework;
using System;
using System.Collections.ObjectModel;

namespace Engendro.PathFinding
{
    /// <summary>
    /// PathNode
    /// </summary>
    public sealed class PathNode(Vector2 position) : IHeapItem<PathNode>
    {
        #region ICompatable<T> explicit implementation

        // CompareTo
        int IComparable<PathNode>.CompareTo(PathNode? other)
        {
            if (other == null)
                return 1;

            var compare = FCost.CompareTo(other.FCost);

            if (compare == 0)
                compare = HCost.CompareTo(other.HCost);

            return -compare;
        }

        #endregion

        // FCost
        public float FCost => GCost + HCost;

        // GCost
        public float GCost { get; set; } = 1;

        // HCost
        public float HCost { get; set; }

        // HeapIndex
        public int HeapIndex { get; set; }

        // Links
        public Collection<int> Links { get; } = [];

        // Parent
        public PathNode? Parent { get; set; }

        // Position
        public Vector2 Position { get; } = position;

        // Reset
        public void Reset()
        {
            GCost = 1;
            HCost = 0;
            Links.Clear();
            Parent = null;
            HeapIndex = 0;
        }

        // ToString
        public override string ToString()
        {
            return Position.ToString();
        }
    }
}
