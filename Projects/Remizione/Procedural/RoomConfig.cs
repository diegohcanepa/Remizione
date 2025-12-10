using System;
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
            // LockType
            if (element.TryGetProperty("lockType", out JsonElement lockTypeElement))
                LockType = Enum.Parse<LockType>(lockTypeElement.GetString() ?? string.Empty);

            // Placement
            if (element.TryGetProperty("placement", out JsonElement placementElement))
                Placement = Enum.Parse<RoomPlacement>(placementElement.GetString() ?? string.Empty);

            // Enemy scope
            var allowPools = ConfigHelper.GetTags(element, "enemyAllowPools");
            var denyPools = ConfigHelper.GetTags(element, "enemyDenyPools");
            var allowTags = ConfigHelper.GetTags(element, "enemyAllowTags");
            var denyTags = ConfigHelper.GetTags(element, "enemyDenyTags");
            var maxPerRoom = -1;
            if (element.TryGetProperty("maxEnemies", out JsonElement maxEnemiesElement))
                maxPerRoom = maxEnemiesElement.GetInt32();
            this.EnemyScope = new ScopeRules(allowPools, denyPools, allowTags, denyTags, maxPerRoom);

            // Hazard scope
            allowPools = ConfigHelper.GetTags(element, "hazardAllowPools");
            denyPools = ConfigHelper.GetTags(element, "hazardDenyPools");
            allowTags = ConfigHelper.GetTags(element, "hazardAllowTags");
            denyTags = ConfigHelper.GetTags(element, "hazardDenyTags");
            maxPerRoom = -1;
            if (element.TryGetProperty("maxHazards", out JsonElement maxHazardsElement))
                maxPerRoom = maxHazardsElement.GetInt32();
            this.HazardScope = new ScopeRules(allowPools, denyPools, allowTags, denyTags, maxPerRoom);

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

        // HazardScope
        public ScopeRules HazardScope { get; }

        // LockType
        public LockType LockType { get; }

        // PassesPlacementConstraint
        public bool PassesPlacementConstraint(RoomGraph roomGraph)
        {
            if (Placement == RoomPlacement.Any)
                return true;

            if (roomGraph.IsSide)
            {
                if (roomGraph.Right != null)
                    return Placement is RoomPlacement.Left or RoomPlacement.MiddleOrLeft;

                else if (roomGraph.Left != null)
                    return Placement is RoomPlacement.Right or RoomPlacement.MiddleOrRight;

                else
                    return false;
            }
            else
            {
                return Placement == RoomPlacement.Middle;
            }
        }

        // Placement
        public RoomPlacement Placement { get; }

        // PropScope
        public ScopeRules PropScope { get; }
    }
}
