namespace ScaryCastle
{
    /// <summary>
    /// RoomGraph 
    /// </summary>
    public sealed class RoomGraph
    {
        // Constructor
        public RoomGraph(int index, int x, int y, RoomType roomType)
        {
            this.Index = index;
            this.X = x;
            this.Y = y;
            this.RoomType = roomType;
        }

        #region Private members

        // UpdateConnectionCount
        // Calculates how many active neighbors this node has.
        private void UpdateConnectionCount()
        {
            int count = 0;
            if (Up != null)
            {
                count++;
            }
            if (Down != null)
            {
                count++;
            }
            if (Left != null)
            {
                count++;
            }
            if (Right != null)
            {
                count++;
            }
            ConnectionCount = count;
        }

        #endregion

        // ConnectionCount
        public int ConnectionCount { get; private set; }

        // Definition
        public RoomDefinition Definition { get; set; } = null!;

        // DistanceFromStart
        public int DistanceFromStart { get; set; }

        // Down
        public RoomGraph? Down
        {
            get => field;
            set
            {
                field = value;
                UpdateConnectionCount();
            }
        }

        // Fits
        // Validates if a room asset can fit into this graph node's connectivity.
        public bool Fits(RoomDefinition def)
        {
            bool needsUp = Up != null;
            bool needsDown = Down != null;
            bool needsLeft = Left != null;
            bool needsRight = Right != null;

            if (def.ExactMatch)
            {
                return needsUp == def.HasUpDoor && needsDown == def.HasDownDoor &&
                       needsLeft == def.HasLeftDoor && needsRight == def.HasRightDoor;
            }

            if (needsUp && !def.HasUpDoor)
                return false;

            if (needsDown && !def.HasDownDoor)
                return false;

            if (needsLeft && !def.HasLeftDoor)
                return false;

            if (needsRight && !def.HasRightDoor)
                return false;

            return true;
        }

        // GetDoorAssetName
        public string GetDoorAssetName(RoomGraph neighbor)
        {
            // Usage: "BlueStone_Gate" or "BlueStone_Wooden"
            return $"{Definition.Theme}_{neighbor.Definition.DoorStyle}";
        }

        // Index
        public int Index { get; }

        // Left
        public RoomGraph? Left
        {
            get => field;
            set
            {
                field = value;
                UpdateConnectionCount();
            }
        }

        // RideRoom
        public RideRoom RideRoom { get; set; } = null!;

        // Right
        public RoomGraph? Right
        {
            get => field;
            set
            {
                field = value;
                UpdateConnectionCount();
            }
        }

        // RoomType
        public RoomType RoomType { get; set; }

        // ToString
        public override string ToString()
        {
            return $"[Room_{RoomType}_{Index} ({X},{Y})]";
        }

        // Up
        public RoomGraph? Up
        {
            get => field;
            set
            {
                field = value;
                UpdateConnectionCount();
            }
        }

        // Visited
        public bool Visited { get; set; }

        // X
        public int X { get; }

        // Y
        public int Y { get; }
    }
}