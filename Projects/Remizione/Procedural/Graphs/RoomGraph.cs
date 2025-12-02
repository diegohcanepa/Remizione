namespace Remizione
{
    /// <summary>
    /// RoomGraph
    /// </summary>
    public sealed class RoomGraph
    {
        // Constructor
        public RoomGraph(int id, bool isRoot, int pathIndex)
        {
            this.Id = id;
            this.IsRoot = isRoot;
            this.PathIndex = pathIndex;
        }

        // RoomConfig
        public RoomConfig? Config { get; set; }

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

        // Realm
        public Realm Realm { get; set; }

        // Right
        public RoomGraph? Right { get; set; }

        // Up
        public RoomGraph? Up { get; set; }
    }
}
