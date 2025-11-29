using Engendro;
using Microsoft.Xna.Framework;
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
        private RoomConfig(JsonElement element)
            : base(element)
        {
            this.PropScope = ConfigHelper.GetTagScope(element, "propRules");
            this.EnemyScope = ConfigHelper.GetTagScope(element, "enemyRules");
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
            var rooms = LoadCore<RoomConfig>(fileName, "rooms", (JsonElement element) => new RoomConfig(element));

            foreach (var roomConfig in rooms)
            {
                data.Add(roomConfig.Name, roomConfig);
            }
        }

        #endregion

        // EnemyScope
        public TagScope EnemyScope { get; }

        // PropScope
        public TagScope PropScope { get; }
    }
}
