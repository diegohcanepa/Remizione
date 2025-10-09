using Engendro;
using System;

namespace Remizione
{
    /// <summary>
    /// RoomWidthPlacementCondition
    /// </summary>
    public sealed class RoomWidthPlacementCondition : PlacementCondition
    {
        // Constructor
        public RoomWidthPlacementCondition(Int32Range size)
        {
            this.Size = size;
        }

        // IsAvailable
        public override bool IsAvailable(ProceduralRoom room, GameThing thing, Random random)
        {
            return Size.Contains(room.Width);
        }

        // Size
        public Int32Range Size { get; }
    }
}
