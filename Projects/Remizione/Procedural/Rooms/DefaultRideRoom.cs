using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// DefaultRideRoom
    /// </summary>
    public sealed class DefaultRideRoom : RideRoom
    {
        // Constructor
        public DefaultRideRoom(GameSession session, RoomGraph graph)
            : base(session, graph)
        {
            // Default
            if (graph.RoomShape == RideRoomShape.Default)
            {
                if (AddWalkArea("WalkArea", "207,46;237,111;5,111;36,46;83,46;88,44;164,44;170,46") is WalkArea walkArea)
                {
                    Children.Add(new RideRoomWall(session, "36,0;36,46;5,111;0,111;0,0"));
                    Children.Add(new RideRoomWall(session, "206,0;206,46;237,111;240,111;240,0"));
                }

                DoorUpPosition = new Vector2(120, 41);
                DoorLeftPosition = new Vector2(21, 80);
                DoorRightPosition = new Vector2(220, 80);
                DoorDownPosition = new Vector2(152, 145);
            }
        }
    }
}
