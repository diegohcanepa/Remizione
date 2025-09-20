using System;

namespace Remizione
{
    /// <summary>
    /// RoomPositionPlacementCondition   
    /// </summary>
    public sealed class RoomPositionPlacementCondition : PlacementCondition
    {
        // Constructor
        public RoomPositionPlacementCondition(RoomPosition roomPosition)
        {
            this.RoomPosition = roomPosition;
        }

        // IsAvailable
        public override bool IsAvailable(GameThing thing, Random random)
        {
            return thing.Session.Room is ProceduralRoom room && room.RoomPosition == this.RoomPosition;
        }

        // RoomPosition
        public RoomPosition RoomPosition { get; }
    }
}
