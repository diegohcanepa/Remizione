using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// RoomNode
    /// </summary>
    public sealed class RoomNode
    {
        // Constructor
        public RoomNode(int index, int x, int y, RoomType roomType, SideRoomCategory sideRoomCategory)
        {
            this.Index = index;
            this.X = x;
            this.Y = y;
            this.RoomType = roomType;
            this.SideRoomCategory = sideRoomCategory;
        }

        #region Private members

        // UpdateConnectionCount
        private void UpdateConnectionCount()
        {
            int count = 0;
            if (Up != null)
                count++;

            if (Down != null)
                count++;

            if (Left != null)
                count++;

            if (Right != null)
                count++;

            ConnectionCount = count;
        }

        #endregion

        // ConnectionCount
        public int ConnectionCount { get; private set; }

        // Definition
        public RoomDefinition Definition { get; set; } = null!;

        // Down
        public RoomNode? Down
        {
            get;
            set
            {
                field = value;
                UpdateConnectionCount();
            }
        }

        // Fits
        public bool Fits(RoomDefinition definition)
        {
            bool needsUp = Up != null;
            bool needsDown = Down != null;
            bool needsLeft = Left != null;
            bool needsRight = Right != null;

            if (definition.ExactMatch)
            {
                return needsUp == definition.HasUpDoor && needsDown == definition.HasDownDoor &&
                       needsLeft == definition.HasLeftDoor && needsRight == definition.HasRightDoor;
            }

            if (needsUp && !definition.HasUpDoor)
                return false;

            if (needsDown && !definition.HasDownDoor)
                return false;

            if (needsLeft && !definition.HasLeftDoor)
                return false;

            if (needsRight && !definition.HasRightDoor)
                return false;

            return true;
        }

        // GetAllNodes
        public IEnumerable<RoomNode> GetAllNodes()
        {
            var visited = new HashSet<RoomNode>();
            var stack = new Stack<RoomNode>();

            stack.Push(this);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if (visited.Contains(current))
                    continue;

                visited.Add(current);

                yield return current;

                // Metemos los vecinos al stack para procesarlos
                if (current.Up != null)
                    stack.Push(current.Up);

                if (current.Down != null)
                    stack.Push(current.Down);

                if (current.Left != null)
                    stack.Push(current.Left);

                if (current.Right != null)
                    stack.Push(current.Right);
            }
        }

        // Index
        public int Index { get; }

        // Left
        public RoomNode? Left
        {
            get;
            set
            {
                field = value;
                UpdateConnectionCount();
            }
        }

        // RideRoom
        public RideRoom RideRoom { get; set; } = null!;

        // Right
        public RoomNode? Right
        {
            get;
            set
            {
                field = value;
                UpdateConnectionCount();
            }
        }

        // RoomType
        public RoomType RoomType { get; set; }

        // SideRoomCategory
        public SideRoomCategory SideRoomCategory { get; set; }

        // ToString
        public override string ToString()
        {
            return $"[Room_{RoomType}_{Index} ({X},{Y})]";
        }

        // Up
        public RoomNode? Up
        {
            get;
            set
            {
                field = value;
                UpdateConnectionCount();
            }
        }

        // X
        public int X { get; }

        // Y
        public int Y { get; }
    }
}