using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;

namespace ScaryCastle
{
    /// <summary>
    /// FloorLayout
    /// </summary>
    public sealed class FloorLayout(ReadOnlyDictionary<Point, RoomNode> map, RoomNode startNode)
    {
        // Map
        public ReadOnlyDictionary<Point, RoomNode> Map { get; } = map;

        // StartNode
        public RoomNode StartNode { get; } = startNode;
    }
}
