using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// CommonRoom
    /// </summary>
    public sealed class CommonRoom : RideRoom
    {
        // Constructor
        public CommonRoom(GameSession session, RoomGraph graph)
            : base(session, graph)
        {
            // Walk area    
            AddWalkArea("WalkArea", "207,46;237,109;5,109;36,46;83,46;88,44;164,44;170,46");

            // Walls
            AddWall("36,0;36,46;5,111;0,111;0,0");
            AddWall("206,0;206,46;237,111;240,111;240,0");

            AddPlaceholder(49, 44, PlacementType.Floor, .5f);
            AddPlaceholder(196, 44, PlacementType.Floor, .5f);
            AddPlaceholder(20, 102, PlacementType.Floor, .5f);
            AddPlaceholder(222, 102, PlacementType.Floor, .5f);

            AddPlaceholder(71, 46, PlacementType.WallFrontBase, .5f);
            AddPlaceholder(177, 46, PlacementType.WallFrontBase, .5f);

            AddPlaceholder(71, 17, PlacementType.WallFrontHang, .5f);
            AddPlaceholder(177, 17, PlacementType.WallFrontHang, .5f);

            // Doors
            DoorAnchorUp = new Vector2(126, 41);
            DoorAnchorLeft = new Vector2(21, 80);
            DoorAnchorRight = new Vector2(220, 80);
            DoorAnchorDown = new Vector2(152, 145);
        }
    }
}
