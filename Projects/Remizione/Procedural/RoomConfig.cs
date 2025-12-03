using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Remizione
{
    /// <summary>
    /// RoomConfig
    /// </summary>
    public sealed class RoomConfig : Config
    {
        private static readonly Dictionary<string, RoomConfig> data = [];
        private static readonly List<RoomConfig> dataList = [];

        // Constructor
        private RoomConfig(JsonElement element)
            : base(element)
        {
            // Enemy scope
            var allowPools = ConfigHelper.GetTags(element, "enemyAllowPools");
            var denyPools = ConfigHelper.GetTags(element, "enemyDenyPools");
            var allowTags = ConfigHelper.GetTags(element, "enemyAllowTags");
            var denyTags = ConfigHelper.GetTags(element, "enemyDenyTags");
            var maxPerRoom = -1;
            if (element.TryGetProperty("maxEnemies", out JsonElement maxEnemiesElement))
                maxPerRoom = maxEnemiesElement.GetInt32();
            this.EnemyScope = new ScopeRules(allowPools, denyPools, allowTags, denyTags, maxPerRoom);

            // Prop scope
            allowPools = ConfigHelper.GetTags(element, "propAllowPools");
            denyPools = ConfigHelper.GetTags(element, "propDenyPools");
            allowTags = ConfigHelper.GetTags(element, "propAllowTags");
            denyTags = ConfigHelper.GetTags(element, "propDenyTags");
            maxPerRoom = -1;
            if (element.TryGetProperty("maxProps", out JsonElement maxPropsElement))
                maxPerRoom = maxPropsElement.GetInt32();
            this.PropScope = new ScopeRules(allowPools, denyPools, allowTags, denyTags, maxPerRoom);

            data.Add(Name, this);
            dataList.Add(this);
        }

        #region Static members

        // All
        public static ReadOnlyCollection<RoomConfig> All { get; } = new(dataList);

        // Find
        public static RoomConfig Find(string name)
        {
            return data[name];
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
