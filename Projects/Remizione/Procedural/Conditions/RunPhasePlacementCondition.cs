using System;

namespace Remizione
{
    /// <summary>
    /// RunPhasePlacementCondition   
    /// </summary>
    public sealed class RunPhasePlacementCondition : PlacementCondition
    {
        // Constructor
        public RunPhasePlacementCondition(RunPhase phase)
        {
            this.Phase = phase;
        }

        // IsAvailable
        public override bool IsAvailable(ProceduralRoom room, GameThing thing, Random random)
        {
            return room.RoomPhase == Phase;
        }

        // Phase
        public RunPhase Phase { get; }
    }
}
