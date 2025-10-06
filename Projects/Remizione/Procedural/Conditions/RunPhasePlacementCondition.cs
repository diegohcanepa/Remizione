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
        public override bool IsAvailable(GameThing thing, Random random)
        {
            return thing.Session.Room is ProceduralRoom room && room.RoomPhase == Phase;
        }

        // Phase
        public RunPhase Phase { get; }
    }
}
