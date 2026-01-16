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
            // Difficulty
            if (element.TryGetProperty("difficulty", out JsonElement difficultyElement))
                Difficulty = Enum.Parse<Difficulty>(difficultyElement.GetString() ?? "");

            // MaxPerRun
            if (element.TryGetProperty("maxPerRun", out JsonElement maxPerRunElement))
                MaxPerRun = Math.Max(MaxPerRun, maxPerRunElement.GetInt32());

            // MinFloor
            if (element.TryGetProperty("minFloor", out JsonElement minFloorElement))
                MinFloor = minFloorElement.GetInt32();

            // PreferredLootCategory
            if (element.TryGetProperty("preferredLootCategory", out JsonElement preferredLootCategoryElement))
                PreferredLootCategory = Enum.Parse<ItemCategory>(preferredLootCategoryElement.GetString() ?? string.Empty);

            // PreferredLootRealm
            if (element.TryGetProperty("preferredLootRealm", out JsonElement preferredLootRealmElement))
                PreferredLootRealm = Enum.Parse<Realm>(preferredLootRealmElement.GetString() ?? string.Empty);

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

        // MinFloor
        public int MinFloor { get; }

        // PassesFloorConstraints
        public bool PassesFloorConstraints(GameSession session)
        {
            // MaxPerRun
            if (MaxPerRun > 0 && RunManager.SpawnCounter.GetCount(Name) >= MaxPerRun)
                return false;

            // Min floor
            if (session.FloorIndex < MinFloor)
                return false;

            return true;
        }

        // PassesMaxPerRunConstraint
        public bool PassesMaxPerRunConstraint()
        {
            return MaxPerRun == 0 || RunManager.SpawnCounter.GetCount(Name) < MaxPerRun;
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

        // Tags
        public Tags Tags { get; }

        // ToString
        public override string ToString()
        {
            return Name;
        }

        // Validate
        public virtual void Validate(GameSession session)
        {
        }

        // ValidateIntegrity
        public static void ValidateIntegrity(GameSession session)
        {
            // TODO: Hay que validar que los nombres en los configs existan como static things o los templates de los rooms
            // que esten en el registry. Sino puede pasar como con expending machine que ahora es vending machine y al
            // cambiar el nombre y no haber actualizado el config, esa prop nunca aparece graficamente.

            foreach (var def in definitions.Values)
            {
                def.Validate(session);
            }
        }
    }
}
