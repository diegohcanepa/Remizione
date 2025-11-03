/*
using System;
using System.Linq;

namespace Remizione
{
    internal static class ProceduralHelper
    {
        // Populate
        internal static void Populate(RoomPlacementData data)
        {
            if (data.Count == 0)
                return;

            foreach (var phase in Enum.GetValues<PlacementPhase>())
            {
                if (phase == PlacementPhase.None)
                    continue;

                var grid = GetTargetGrid(phase);

                var staticThings = GetStaticThings(phase);
                staticThings.Shuffle(Random);

                foreach (var thing in staticThings)
                {
                    var placementDataList = data.GetList(thing.StaticName);
                    if (placementDataList == null)
                        continue;

                    for (var i = 0; i < placementDataList.Count; i++)
                    {
                        var placementData = placementDataList[i];

                        if (!placementData.IsAvailable(this, thing, Random))
                            continue;

                        placementData.ResetSpawnCount();

                        switch (placementData.DistributionStrategy)
                        {
                            // Random
                            case PlacementDistributionStrategy.Random:
                                DistributeRandomly(grid, thing, placementData);
                                break;

                            // Clump
                            case PlacementDistributionStrategy.Clump:
                                DistributeClumped(grid, thing, placementData);
                                break;

                            // NoiseMap 
                            case PlacementDistributionStrategy.NoiseMap:
                                DistributeWithNoiseMap(grid, thing, placementData, randomSeed);
                                break;
                        }
                    }
                }
            }
        }
    }
}
*/