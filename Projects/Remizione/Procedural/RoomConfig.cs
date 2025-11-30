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
            this.PropScope = ConfigHelper.GetScopeRules(element, "propRules");
            this.EnemyScope = ConfigHelper.GetScopeRules(element, "enemyRules");

            data.Add(Name, this);
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
            Utils.LoadJsonData<RoomConfig>(fileName, (JsonElement element) => new RoomConfig(element));
        }

        #endregion

        // EnemyScope
        public ScopeRules EnemyScope { get; }

        // PropScope
        public ScopeRules PropScope { get; }
    }
}
