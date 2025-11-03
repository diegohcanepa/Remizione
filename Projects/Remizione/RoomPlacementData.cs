using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
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
