using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Engendro.PathFinding
{
    /// <summary>
    /// PathNode
    /// </summary>
    public sealed class PathNode : IHeapItem<PathNode>
    {
        private Vector2 position;

        #region Constructors

        // Constructor
        public PathNode()
        {
        }

        // Constructor
        public PathNode(Vector2 position)
        {
            this.position = position;
        }

        #endregion

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
        public List<int> Links { get; } = [];

        // Parent
        public PathNode? Parent { get; set; }

        // Position
        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;
                Reset();
            }
        }

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
