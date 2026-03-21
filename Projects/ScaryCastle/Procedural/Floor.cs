using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ScaryCastle
{
    /// <summary>
    /// Floor
    /// </summary>
    public sealed class Floor
    {
        // Constructor
        public Floor(List<RoomGraph> rooms, int index)
        {
            RoomGraphs = rooms.AsReadOnly();
            Index = index;
        }

        // Index
        public int Index { get; }

        // RoomGraphs
        public ReadOnlyCollection<RoomGraph> RoomGraphs { get; }
    }
}
