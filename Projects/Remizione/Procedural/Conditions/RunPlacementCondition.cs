using System;

namespace Remizione
{
    /// <summary>
    /// RunPlacementCondition
    /// </summary>
    public sealed class RunPlacementCondition : PlacementCondition
    {
        // Constructor
        public RunPlacementCondition(int minimumRuns)
        {
            this.MinimumRuns = minimumRuns;
        }

        // IsAvailable
        public override bool IsAvailable(GameThing thing, Random random)
        {
            return thing.Session.Runs >= MinimumRuns;
        }

        // MinimumRuns
        public float MinimumRuns { get; }
    }
}
