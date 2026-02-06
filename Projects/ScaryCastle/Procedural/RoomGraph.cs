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

        // Definition
        public RoomDefinition Definition { get; set; } = null!;

        // DistanceFromStart
        public int DistanceFromStart { get; set; }

        // Down
        public RoomGraph? Down { get; set; }

        // GetConnectionCount
        public int GetConnectionCount()
        {
            var result = 0;

            if (Up != null) result++;
            if (Down != null) result++;
            if (Left != null) result++;
            if (Right != null) result++;

            return result;
        }

        // HeartCount
        public int HeartCount { get; set; }

        // Index
        public int Index { get; }

        // Left
        public RoomGraph? Left { get; set; }

        // Realm
        public Realm Realm { get; set; }

        // RideRoom
        public RideRoom RideRoom { get; set; } = null!;

        // Right
        public RoomGraph? Right { get; set; }

        // RoomType
        public RoomType RoomType { get; set; }

        // ToString
        public override string ToString()
        {
            return $"[Room_{RoomType}_{Index}]";
        }

        // Up
        public RoomGraph? Up { get; set; }

        // Visited
        public bool Visited { get; set; }

        // X
        public int X { get; }

        // Y
        public int Y { get; }
    }
}
