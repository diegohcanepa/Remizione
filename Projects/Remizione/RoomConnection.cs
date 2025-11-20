using Adberration.Scripting;

namespace Remizione
{
    /// <summary>
    /// RoomConnection
    /// </summary>
    public abstract class RoomConnection : Prop
    {
        // Constructor
        protected RoomConnection(GameSession session, string name)
            : base(session, name)
        {
        }

        // Connect
        [ScriptMethod(CodingContext.Execution)]
        public bool Connect()
        {
            if (NextRoom == null)
                return false;

            if (RunManager.GetRoom(NextRoom.RoomGraph.Id) is not RideRoom nextRoom)
                return false;

            if (Session.Player != null)
            {
                Session.Player.Unparent();
                nextRoom.Children.Add(Session.Player);
                //Session.Player.Position = NextRoomPosition;
                //Session.Player.Direction = NextRoomDirection;
                Session.Camera.FollowTarget(Session.Player, true);
            }

            Session.EnterRoom(nextRoom);

            //OnConnected(NextRoom);

            return true;
        }

        // NextRoom
        public ProceduralRoom? NextRoom { get; set; }

        // PreviousRoom
        public ProceduralRoom? PreviousRoom { get; set; }
    }
}
