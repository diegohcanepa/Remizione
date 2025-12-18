namespace Remizione
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

        // Config
        public RoomConfig? Config { get; set; }

        // DistanceFromStart
        public int DistanceFromStart { get; set; }

        // Down
        public RoomGraph? Down { get; set; }

        // Index
        public int Index { get; }

        // IsDeadEnd
        public bool IsDeadEnd
        {
            get
            {
                return (Up == null ? 1 : 0) + (Down == null ? 1 : 0) +
                       (Left == null ? 1 : 0) + (Right == null ? 1 : 0) >= 3;
            }
        }

        // Left
        public RoomGraph? Left { get; set; }

        // Realm
        public Realm Realm { get; set; }

        // RideRoom
        public RideRoom? RideRoom { get; set; }

        // Right
        public RoomGraph? Right { get; set; }

        // RoomType
        public RoomType RoomType { get; set; }

        // SackCount
        public int SackCount { get; set; }

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
