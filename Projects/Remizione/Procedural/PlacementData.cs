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
        public PlacementData(PlacementDistributionStrategy distributionStrategy, PlacementCondition[] conditions, Int32Range tries)
        {
            this.DistributionStrategy = distributionStrategy;
            this.conditions.AddRange(conditions);
            this.Conditions = new(conditions);
            this.Tries = tries;
        }

        // Conditions
        public ReadOnlyCollection<PlacementCondition> Conditions { get; }

        // DistributionStrategy
        public PlacementDistributionStrategy DistributionStrategy { get; }

        // IsAvailable
        public bool IsAvailable(GameThing thing, Random random)
        {
            if (Tries.IsEmpty)
                return false;

            for (int i = 0; i < conditions.Count; i++)
            {
                if (!conditions[i].IsAvailable(thing, random))
                    return false;
            }

            return true;
        }

        // Tries
        public Int32Range Tries { get; }
    }
}
