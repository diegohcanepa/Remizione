using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ScaryCastle
{
    /// <summary>
    /// Stage
    /// </summary>
    public sealed class Stage
    {
        // Constructor
        public Stage(List<RoomGraph> rooms, int index)
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
