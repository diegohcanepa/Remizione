using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// BlueStoneRoom
    /// </summary>
    public sealed class BlueStoneRoom : RideRoom
    {
        // Constructor
        public BlueStoneRoom(GameSession session, RoomGraph graph)
            : base(session, graph)
        {
            // Walk area    
            AddWalkArea("WalkArea", "207,46;237,109;5,109;36,46;83,46;88,44;164,44;170,46");

            // Walls
            Children.Add(new RideRoomWall(session, "36,0;36,46;5,111;0,111;0,0"));
            Children.Add(new RideRoomWall(session, "206,0;206,46;237,111;240,111;240,0"));

            // Placeholders
            AddPlaceholder("1", PlaceholderType.Floor, PlaceholderSize.Medium, 1, false, "41,32;56,32;56,47;41,47");
            AddPlaceholder("2", PlaceholderType.Floor, PlaceholderSize.Medium, 1, false, "64,34;79,34;79,49;64,49");
            AddPlaceholder("3", PlaceholderType.Floor, PlaceholderSize.Medium, 1, false, "4170,34;185,34;185,49;170,49");
            AddPlaceholder("4", PlaceholderType.Floor, PlaceholderSize.Medium, 1, false, "189,32;204,32;204,47;189,47");
            AddPlaceholder("5", PlaceholderType.Floor, PlaceholderSize.Medium, 1, false, "219,92;234,92;234,107;219,107");
            AddPlaceholder("6", PlaceholderType.Floor, PlaceholderSize.Medium, 1, false, "8,92;23,92;23,107;8,107");

            // Doors
            DoorUpPosition = new Vector2(126, 41);
            DoorLeftPosition = new Vector2(21, 80);
            DoorRightPosition = new Vector2(220, 80);
            DoorDownPosition = new Vector2(152, 145);
        }
    }
}
