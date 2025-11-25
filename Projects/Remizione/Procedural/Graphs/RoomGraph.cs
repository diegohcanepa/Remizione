namespace Remizione
{
    /// <summary>
    /// RoomGraph
    /// </summary>
    public sealed class RoomGraph
    {
        // Constructor
        public RoomGraph(int id, bool isRoot, int pathIndex, RideRoomStyle roomStyle)
        {
            this.Id = id;
            this.IsRoot = isRoot;
            this.PathIndex = pathIndex;
            this.RoomStyle = roomStyle;
        }

        // Down
        public RoomGraph? Down { get; set; }

        // HasCoin
        public bool HasCoin { get; set; }

        // Id
        public int Id { get; }

        // IsRoot
        public bool IsRoot { get; }

        // Left
        public RoomGraph? Left { get; set; }

        // PathIndex
        public int PathIndex { get; }

        // Right
        public RoomGraph? Right { get; set; }

        // RoomShape
        public RideRoomShape RoomShape { get; }

        // RoomStyle
        public RideRoomStyle RoomStyle { get; }

        // Up
        public RoomGraph? Up { get; set; }
    }
}
