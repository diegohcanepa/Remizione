using Engendro;
using Engendro.Collections;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Remizione
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
            MinFloor = element.GetInt32("minFloor", 0);
            PreferredLootCategory = element.GetEnum<ItemCategory>("preferredLootCategory");
            PreferredLootRealm = element.GetEnum<Realm>("preferredLootRealm");

            // Pools
            Pools = ReadOnlyEnumSet<Tag>.FromJsonOrEmpty(element, "pools");

            // Tags
            Tags = ReadOnlyEnumSet<Tag>.FromJsonOrEmpty(element, "tags");
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

        // MinFloor
        public int MinFloor { get; }

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
        public bool PassesRunConstraints(int floorCount)
        {
            return floorCount >= MinFloor;
        }

        // Pools
        public ReadOnlyEnumSet<Tag> Pools { get; }

        // PreferredLootCategory
        public ItemCategory? PreferredLootCategory { get; }

        // PreferredLootRealm
        public Realm? PreferredLootRealm { get; }

        // Tags
        public ReadOnlyEnumSet<Tag> Tags { get; }
    }
}
