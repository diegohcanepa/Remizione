namespace Remizione
{
    /// <summary>
    /// RoomDescriptor
    /// </summary>
    public sealed class RoomGraph
    {
        // Constructor
        public RoomGraph(int id, bool isRoot, int pathIndex, RideRoomKind roomKind)
        {
            this.Id = id;
            this.IsRoot = isRoot;
            this.PathIndex = pathIndex;
            this.RoomKind = roomKind;
        }

        // Down
        public RoomGraph? Down { get; set; }

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

        // RoomKind
        public RideRoomKind RoomKind { get; }

        // Up
        public RoomGraph? Up { get; set; }
    }
}
