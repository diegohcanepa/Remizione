using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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

        /// <summary>
        /// RoomPlacementData
        /// </summary>
        public sealed class RoomPlacementData
        {
            private readonly Dictionary<string, List<PlacementData>> thingData = [];

            // Add
            public void Add(string thingName, PlacementData placementData)
            {
                if (thingData.TryGetValue(thingName, out var existingList))
                    existingList.Add(placementData);
                else
                    thingData.Add(thingName, [placementData]);
            }

            // Count
            public int Count => thingData.Count;

            // GetList
            public ReadOnlyCollection<PlacementData>? GetList(string thingName)
            {
                if (thingData.TryGetValue(thingName, out var existingList))
                    return existingList.AsReadOnly();
                else
                    return null;
            }
        }
    }
}
