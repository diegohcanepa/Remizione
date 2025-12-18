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

            AddPlaceholders(GetPlaceholders());

            // Doors
            DoorAnchorUp = new Vector2(126, 41);
            DoorAnchorLeft = new Vector2(21, 80);
            DoorAnchorRight = new Vector2(220, 80);
            DoorAnchorDown = new Vector2(152, 145);
        }

        // GetPlaceholderNames
        public static string[] GetPlaceholderNames()
        {
            var placeholders = GetPlaceholders();
            var result = new string[placeholders.Count];
            for (var i = 0; i < placeholders.Count; i++)
            {
                result[i] = placeholders[i].Name;
            }

            return result;
        }

        // GetPlaceholders
        public static List<Placeholder> GetPlaceholders()
        {
            return
            [
                new("Floor1", .5f, false, "41,32;56,32;56,47;41,47", ["floor","expendingMachine"]),
                new("Floor2", .5f, false, "64,34;79,34;79,49;64,49", ["floor"]),
                new("Floor3", .5f, false, "4170,34;185,34;185,49;170,49", ["floor"]),
                new("Floor4", .5f, false, "189,32;204,32;204,47;189,47", ["floor"]),
                new("Floor5", .5f, false, "219,92;234,92;234,107;219,107", ["floor"]),
                new("Floor6", .5f, false, "8,92;23,92;23,107;8,107", ["floor"]),
                new("LeftWall", .5f, false, "64,4;79,4;79,19;64,19", ["wall"]),
                new("LeftColumn", .5f, false, "64,4;79,4;79,19;64,19", ["wall"]),
                new("RightColumn", .5f, false, "170,5;185,5;185,20;170,20", ["wall"])
            ];
        }
    }
}
