using Adberration.Scripting;

namespace Remizione
{
    /// <summary>
    /// RideDoor
    /// </summary>
    public class RideDoor : Prop
    {
        // Constructor
        public RideDoor(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Environment;
            DisplayNameKey = "Prop.Door";
        }

        // Connect
        [ScriptMethod(CodingContext.Execution)]
        public bool Connect()
        {
            if (NextRoom == null)
                return false;

            if (RunManager.GetRoom(NextRoom.Descriptor.Id) is not RideRoom nextRoom)
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
        public RideRoom? NextRoom { get; set; }
    }
}
