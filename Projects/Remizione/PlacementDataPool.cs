using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// PlacementDataPool
    /// </summary>
    public sealed class PlacementDataPool
    {
        private readonly Dictionary<RideRoomKind, RoomPlacementData> roomData = [];

        // Constructor
        public PlacementDataPool()
        {
            foreach (var roomKind in Enum.GetValues<RideRoomKind>())
            {
                roomData.Add(roomKind, new());
            }
        }

        // Add
        public void Add(RideRoomKind roomKind, string thingName, PlacementData placementData)
        {
            if (roomData.TryGetValue(roomKind, out var roomInfo))
                roomInfo.Add(thingName, placementData);
        }

        // GetRoomPlacementData
        public RoomPlacementData GetRoomPlacementData(RideRoomKind roomKind) => roomData[roomKind];
    }
}
