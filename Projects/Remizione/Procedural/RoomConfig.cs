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
            // Enemy scope
            var allowPools = ConfigHelper.GetStringArrayValues(element, "enemyAllowPools");
            var denyPools = ConfigHelper.GetStringArrayValues(element, "enemyDenyPools");
            var allowTags = ConfigHelper.GetStringArrayValues(element, "enemyAllowTags");
            var denyTags = ConfigHelper.GetStringArrayValues(element, "enemyDenyTags");
            var maxPerRoom = -1;
            if (element.TryGetProperty("maxEnemies", out JsonElement maxEnemiesElement))
                maxPerRoom = maxEnemiesElement.GetInt32();
            this.EnemyScope = new ScopeRules(allowPools, denyPools, allowTags, denyTags, maxPerRoom);

            // Prop scope
            allowPools = ConfigHelper.GetStringArrayValues(element, "propAllowPools");
            denyPools = ConfigHelper.GetStringArrayValues(element, "propDenyPools");
            allowTags = ConfigHelper.GetStringArrayValues(element, "propAllowTags");
            denyTags = ConfigHelper.GetStringArrayValues(element, "propDenyTags");
            maxPerRoom = -1;
            if (element.TryGetProperty("maxProps", out JsonElement maxPropsElement))
                maxPerRoom = maxPropsElement.GetInt32();
            this.PropScope = new ScopeRules(allowPools, denyPools, allowTags, denyTags, maxPerRoom);

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
