using Engendro;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// EntityDefinition
    /// </summary>
    public abstract class EntityDefinition : Definition
    {
        private static readonly Dictionary<string, EntityDefinition> definitions = [];

        #region Constructor

        // Constructor
        protected EntityDefinition(JsonElement element)
            : base(element)
        {
            Difficulty = element.GetEnum("difficulty", Difficulty.Easy);
            MaxPerRun = element.GetInt32("maxPerRun", -1);
            MinRun = element.GetInt32("minRun", 0);
            PreferredLootCategory = element.GetEnum<ItemCategory>("preferredLootCategory");
            PreferredLootRealm = element.GetEnum<Realm>("preferredLootRealm");
            QualityBoost = element.GetInt32("qualityBoost", 0);
            if (QualityBoost < 0)
                QualityBoost = 0;

            // Tags
            Tags = Tags.FromJson(element, "tags");

            // Pools
            Pools = Tags.FromJson(element, "pools");
        }

        #endregion

        #region Protected members

        // ValidateNameReferences
        protected void ValidateNameReferences(string properyName, IList<string> names)
        {
            for (var i = 0; i < names.Count; i++)
            {
                if (!definitions.ContainsKey(names[i]))
                    throw new InvalidOperationException($"'{names[i]}' listed in [{Name}.{properyName}] does not exist.");
            }
        }

        #endregion

        // Difficulty
        public Difficulty Difficulty { get; }

        // MaxPerRun
        public int MaxPerRun { get; }

        // MinRun
        public int MinRun { get; }

        // PassesMaxPerRunConstraint
        public bool PassesMaxPerRunConstraint(params CounterBank[] counters)
        {
            var count = 0;
            for (var i = 0; i < counters.Length; i++)
            {
                count += counters[i].GetCount(Name);
            }

            return PassesMaxPerRunConstraint(count);
        }

        // PassesMaxPerRunConstraint
        public bool PassesMaxPerRunConstraint(int count)
        {
            // Ilimitado
            if (MaxPerRun < 0)
                return true;

            // Prohibido
            if (MaxPerRun == 0)
                return false;

            return count < MaxPerRun;
        }

        // PassesRunConstraints
        public bool PassesRunConstraints(int runCount)
        {
            return runCount >= MinRun;
        }

        // Pools
        public Tags Pools { get; }

        // PreferredLootCategory
        public ItemCategory? PreferredLootCategory { get; }

        // PreferredLootRealm
        public Realm? PreferredLootRealm { get; }

        // QualityBoost
        public int QualityBoost { get; }

        // Tags
        public Tags Tags { get; }
    }
}
