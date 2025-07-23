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
        public PlacementData(PlacementPhase phase, PlacementDistributionStrategy distributionStrategy, PlacementCondition[] conditions, Int32Range instances)
        {
            this.Phase = phase;
            this.DistributionStrategy = distributionStrategy;
            this.conditions.AddRange(conditions);
            this.Conditions = new(conditions);
            this.Instances = instances;
        }

        // Conditions
        public ReadOnlyCollection<PlacementCondition> Conditions { get; }

        // DistributionStrategy
        public PlacementDistributionStrategy DistributionStrategy { get; }

        // Instances
        public Int32Range Instances { get; }

        // IsAvailable
        public bool IsAvailable(GameThing thing, Random random)
        {
            if (Instances.IsEmpty)
                return false;

            for (int i = 0; i < conditions.Count; i++)
            {
                if (!conditions[i].IsAvailable(thing, random))
                    return false;
            }

            return true;
        }

        // Phase
        public PlacementPhase Phase { get; }
    }
}
