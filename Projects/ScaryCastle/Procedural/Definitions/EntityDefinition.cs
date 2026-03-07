using Engendro;
using ScaryCastle.Procedural;
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

        // PassesRunConstraints
        public bool PassesRunConstraints(GameSession session)
        {
            if (!PassesMaxPerRunConstraint())
                return false;

            return session.RunCount >= MinRun;
        }

        // PassesMaxPerRunConstraint
        public bool PassesMaxPerRunConstraint()
        {
            if (MaxPerRun == 0)
                return false;

            else if (MaxPerRun < 0)
                return true;

            else
                return RunManager.SpawnCounter.GetCount(Name) < MaxPerRun;
        }

        // PassesScope
        public bool PassesScope(ScopeRules scope)
        {
            // DenyPools
            if (scope.DenyPools.Count > 0)
            {
                if (Utils.Intersects(scope.DenyPools, Pools))
                    return false;
            }

            // DenyTags
            if (scope.DenyTags.Count > 0)
            {
                if (Utils.Intersects(scope.DenyTags, Tags))
                    return false;
            }

            // AllowPools (si existe, requiere intersección)
            if (scope.AllowPools.Count > 0)
            {
                if (!Utils.Intersects(scope.AllowPools, Pools))
                    return false;
            }
            else
            {
                // AllowTags VACÍO -> aceptar todo (equivalente a "any")
                if (scope.AllowTags.Count > 0)
                {
                    // si hay al menos una tag en allow, requerimos intersección
                    if (!Utils.Intersects(scope.AllowTags, Tags))
                        return false;
                }

                // si AllowTags está vacío o es null, no filtramos por tags (aceptamos)
            }

            return true;
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
