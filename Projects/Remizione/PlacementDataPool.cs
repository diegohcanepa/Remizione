using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// PlacementDataPool
    /// </summary>
    public sealed class PlacementDataPool
    {
        private readonly Dictionary<RideRoomStyle, RoomPlacementData> roomData = [];

        // Constructor
        public PlacementDataPool()
        {
            foreach (var roomStyle in Enum.GetValues<RideRoomStyle>())
            {
                roomData.Add(roomStyle, new());
            }
        }

        // Add
        public void Add(RideRoomStyle roomStyle, string thingName, PlacementData placementData)
        {
            if (roomData.TryGetValue(roomStyle, out var roomInfo))
                roomInfo.Add(thingName, placementData);
        }

        // GetRoomPlacementData
        public RoomPlacementData GetRoomPlacementData(RideRoomStyle roomStyle) => roomData[roomStyle];
    }
}
