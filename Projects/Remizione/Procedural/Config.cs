using Engendro;
using SharpDX.Direct3D9;
using System;
using System.IO;
using System.Text.Json;

namespace Remizione
{
    /// <summary>
    /// Config
    /// </summary>
    public abstract class Config
    {
        // Constructor
        protected Config(JsonElement element)
        {
            // Name
            this.Name = element.GetProperty("name").GetString() ?? throw new InvalidDataException("Name not found.");

            // Weight
            Weight = 1;
            if (element.TryGetProperty("weight", out JsonElement weightElement))
                Weight = weightElement.GetSingle();

            // MaxPerRun
            if (element.TryGetProperty("maxPerRun", out JsonElement maxPerRunElement))
                MaxPerRun = Math.Max(MaxPerRun, maxPerRunElement.GetInt32());

            // RequiredCompletedRuns
            if (element.TryGetProperty("requiredCompletedRuns", out JsonElement requiredCompletedRunsElement))
                RequiredCompletedRuns = requiredCompletedRunsElement.GetInt32();

            // RequiredRuns
            if (element.TryGetProperty("requiredRuns", out JsonElement requiredRunsElement))
                RequiredRuns = requiredRunsElement.GetInt32();

            // Tags
            Tags = ConfigHelper.GetTags(element, "tags");

            // LootTable
            LootTable = ConfigHelper.GetLootTable(element);

            // Pools
            ConfigHelper.GetTags(element, "pools");

            // Unlocked
            if (element.TryGetProperty("unlocked", out JsonElement unlockedElement))
                Unlocked = unlockedElement.GetBoolean();
        }

        // LootTable
        public ChanceTable LootTable { get; }

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

            // RequiredCompletedRuns
            if (RequiredCompletedRuns > 0)
            {
                if (session.CompletedRuns < RequiredCompletedRuns)
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

        // RequiredCompletedRuns
        public int RequiredCompletedRuns { get; }

        // RequiredRuns
        public int RequiredRuns { get; }

        // Tags
        public Tags Tags { get; }

        // ToString
        public override string ToString()
        {
            return Name;
        }

        // Unlocked
        public bool Unlocked { get; }

        // Weight
        public float Weight { get; }
    }
}
