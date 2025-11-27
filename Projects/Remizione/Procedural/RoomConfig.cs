using Microsoft.Xna.Framework;
using Remizione.Procedural;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Remizione
{
    /// <summary>
    /// RoomConfig
    /// </summary>
    public sealed class RoomConfig : Config
    {
        private static readonly Dictionary<string, RoomConfig> data = [];

        // Constructor
        private RoomConfig(string name, IList<string> tags, RoomConfigScopeRule propScopeRule, RoomConfigScopeRule enemyScopeRule, ChanceTable lootTable)
            : base(name, tags, lootTable)
        {
            this.PropScopeRule = propScopeRule;
            this.EnemyScopeRule = enemyScopeRule;
        }

        #region Static members

        // GetConfig
        public static RoomConfig GetConfig(string roomName)
        {
            return data[roomName];
        }

        // Load
        public static void Load(string fileName)
        {
            try
            {
                using var input = TitleContainer.OpenStream(fileName);
                using JsonDocument doc = JsonDocument.Parse(input);
                var root = doc.RootElement;

                if (!root.TryGetProperty("rooms", out JsonElement roomsArray) || roomsArray.ValueKind != JsonValueKind.Array)
                    throw new InvalidDataException("Rooms is not an array.");

                foreach (JsonElement roomElement in roomsArray.EnumerateArray())
                {
                    // Name
                    if (roomElement.GetProperty("name").GetString() is not string roomName)
                        throw new InvalidDataException("Room name not found.");

                    // Tags
                    var tags = ConfigHelper.GetTags(roomElement);

                    // Prop scope rule
                    var propScopeRule = ConfigHelper.GetScopeRule(roomElement, "propRules");

                    // Enemy scope rule
                    var enemyScopeRule = ConfigHelper.GetScopeRule(roomElement, "enemyRules");

                    // Loot
                    var loot = ConfigHelper.GetLoot(roomElement);

                    // Add configuration
                    var roomConfig = new RoomConfig(roomName, tags, propScopeRule, enemyScopeRule, loot);
                    data.Add(roomName, roomConfig);
                }
            }
            catch (FileNotFoundException)
            {
            }
        }

        #endregion

        // EnemyScopeRule
        public RoomConfigScopeRule EnemyScopeRule { get; }

        // PropScopeRule
        public RoomConfigScopeRule PropScopeRule { get; }
    }
}
