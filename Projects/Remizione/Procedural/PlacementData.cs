using Engendro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// PlacementData
    /// </summary>
    public sealed class PlacementData
    {
        private readonly List<PlacementCondition> conditions = [];

        // Constructor
        public PlacementData(PlacementDistributionStrategy distributionStrategy, PlacementCondition[] conditions, Int32Range rolls, int maximum, int maximumPerRun)
        {
            this.DistributionStrategy = distributionStrategy;
            this.conditions.AddRange(conditions);
            this.Conditions = new(conditions);
            this.Rolls = rolls;
            this.Maximum = maximum;
            this.MaximumPerRun = maximumPerRun;
        }

        // CanSpawn
        public bool CanSpawn(string staticName)
        {
            if (MaximumPerRun > 0 && RunInfo.GetSpawnCount(staticName) >= MaximumPerRun)
                return false;

            if (Maximum > 0 && SpawnCount >= Maximum)
                return false;

            return true;
        }

        // Conditions
        public ReadOnlyCollection<PlacementCondition> Conditions { get; }

        // DistributionStrategy
        public PlacementDistributionStrategy DistributionStrategy { get; }

        // IsAvailable
        public bool IsAvailable(GameThing thing, Random random)
        {
            if (Rolls.IsEmpty)
                return false;

            if (MaximumPerRun > 0 && RunInfo.GetSpawnCount(thing.StaticName) >= MaximumPerRun)
                return false;

            for (int i = 0; i < conditions.Count; i++)
            {
                if (!conditions[i].IsAvailable(thing, random))
                    return false;
            }

            return true;
        }

        // LogSpawn
        public void LogSpawn(string staticName)
        {
            SpawnCount++;
            RunInfo.LogSpawn(staticName);
        }

        // Maximum
        public int Maximum { get; }

        // MaximumPerRun
        public int MaximumPerRun { get; }

        // ResetSpawnCount
        public void ResetSpawnCount() => SpawnCount = 0;

        // Rolls
        public Int32Range Rolls { get; }

        // SpawnCount
        public int SpawnCount { get; private set; }
    }
}
