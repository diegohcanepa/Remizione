using ScaryCastle.Procedural;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// Config
    /// </summary>
    public abstract class Config
    {
        private static readonly Dictionary<string, Config> configs = [];

        // Constructor
        protected Config(JsonElement element)
        {
            // Name
            this.Name = element.GetProperty("name").GetString() ?? throw new InvalidDataException("Name not found.");

            // Difficulty
            if (element.TryGetProperty("difficulty", out JsonElement difficultyElement))
                Difficulty = Enum.Parse<Difficulty>(difficultyElement.GetString() ?? "");

            // MaxPerRun
            if (element.TryGetProperty("maxPerRun", out JsonElement maxPerRunElement))
                MaxPerRun = Math.Max(MaxPerRun, maxPerRunElement.GetInt32());

            // PreferredLootCategory
            if (element.TryGetProperty("preferredLootCategory", out JsonElement preferredLootCategoryElement))
                PreferredLootCategory = Enum.Parse<ItemCategory>(preferredLootCategoryElement.GetString() ?? "");

            // PreferredLootRealm
            if (element.TryGetProperty("preferredLootRealm", out JsonElement preferredLootRealmElement))
                PreferredLootRealm = Enum.Parse<Realm>(preferredLootRealmElement.GetString() ?? "");

            // RequiredRuns
            if (element.TryGetProperty("requiredRuns", out JsonElement requiredRunsElement))
                RequiredRuns = requiredRunsElement.GetInt32();

            // RequiresDeadEnd
            if (element.TryGetProperty("requiresDeadEnd", out JsonElement requiresDeadEndElement))
                RequiresDeadEnd = requiresDeadEndElement.GetBoolean();

            // Tags
            Tags = ConfigHelper.GetTags(element, "tags");

            // Pools
            Pools = ConfigHelper.GetTags(element, "pools");

            // Unlocked
            if (element.TryGetProperty("unlocked", out JsonElement unlockedElement))
                Unlocked = unlockedElement.GetBoolean();

            // Weight
            Weight = 1;
            if (element.TryGetProperty("weight", out JsonElement weightElement))
                Weight = weightElement.GetSingle();
        }

        #region Protected members

        // ValidateNames
        protected void ValidateNames(string properyName, IList<string> names)
        {
            for (var i = 0; i < names.Count; i++)
            {
                if (!configs.ContainsKey(names[i]))
                    throw new InvalidOperationException($"'{names[i]}' listed in [{Name}.{properyName}] does not exist.");
            }
        }

        #endregion

        // Difficulty
        public Difficulty Difficulty { get; }

        // MaxPerRun
        public int MaxPerRun { get; }

        // Name
        public string Name { get; }

        // PassesMaxPerRunConstraint
        public bool PassesMaxPerRunConstraint()
        {
            return MaxPerRun == 0 || RunManager.SpawnCounter.GetCount(Name) < MaxPerRun;
        }

        // PassesRunConstraints
        public bool PassesRunConstraints(GameSession session)
        {
            // MaxPerRun
            if (MaxPerRun > 0 && RunManager.SpawnCounter.GetCount(Name) >= MaxPerRun)
                return false;

            // RequiredRuns
            if (RequiredRuns > 0)
            {
                if (session.TotalRuns < RequiredRuns)
                    return false;
            }

            return true;
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

        // RequiredRuns
        public int RequiredRuns { get; }

        // RequiresDeadEnd
        public bool RequiresDeadEnd { get; }

        // Tags
        public Tags Tags { get; }

        // ToString
        public override string ToString()
        {
            return Name;
        }

        // Unlocked
        public bool Unlocked { get; }

        // Validate
        public virtual void Validate()
        {
        }

        // ValidateAllConfigurations
        public static void ValidateAllConfigurations()
        {
            foreach (var config in configs.Values)
            {
                config.Validate();
            }
        }

        // Weight
        public float Weight { get; }
    }
}
