using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// PlacementDataPool
    /// </summary>
    public sealed class PlacementDataPool
    {
        private readonly Dictionary<RoomKind, RoomPlacementData> roomData = [];

        // Constructor
        public PlacementDataPool()
        {
            foreach (var roomKind in Enum.GetValues<RoomKind>())
            {
                roomData.Add(roomKind, new());
            }
        }

        // Add
        public void Add(RoomKind roomKind, string thingName, PlacementData placementData)
        {
            if (roomData.TryGetValue(roomKind, out var roomInfo))
                roomInfo.Add(thingName, placementData);
        }

        // GetRoomPlacementData
        public RoomPlacementData GetRoomPlacementData(RoomKind roomKind) => roomData[roomKind];
    }
}
