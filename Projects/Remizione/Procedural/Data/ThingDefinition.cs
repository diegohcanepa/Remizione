using Engendro;
using Engendro.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Remizione
{
    /// <summary>
    /// ThingDefinition
    /// </summary>
    public abstract class ThingDefinition : EntityDefinition
    {
        private readonly List<EffectDescriptor> effects = [];

        #region Constructor

        // Constructor
        protected ThingDefinition(JsonElement element, SpawnScope defaultSpawnScope)
            : base(element)
        {
            // BaseDropChance
            BaseDropChance = MathF.Max(0, element.GetFloat("baseDropChance", .5f));

            // Faction
            Faction = element.GetEnum("faction", Faction.Evil);

            // GraceReward
            GraceReward = element.GetInt32("graceReward", 0);

            // LootPool
            var tempPool = new ChanceTable();
            if (element.TryGetProperty("lootPool", out JsonElement poolArray))
            {
                foreach (var itemJson in poolArray.EnumerateArray())
                {
                    string item = itemJson.GetString("item", string.Empty);
                    if (GameData.Items.Find(item) is not ItemDefinition itemDefinition)
                    {
                        RaiseValidationError(this, $"The item '{item}' does not exist.", nameof(LootPool));
                    }
                    else
                    {
                        int weight = itemJson.GetInt32("weight", 0);
                        tempPool.Add(item, weight, itemDefinition);
                    }
                }
            }

            LootPool = tempPool.AsReadOnly();

            // MaxPerRoom
            MaxPerRoom = element.GetInt32("maxPerRoom", -1);

            // RoomTheme
            RoomTheme = element.GetEnum<RoomTheme>("roomTheme");

            // SpawnScope
            SpawnScope = element.GetEnum("spawnScope", defaultSpawnScope);

            // TargetRoomCategory
            TargetRoomCategory = element.GetEnum<RoomCategory>("targetRoomCategory");

            // Effects
            if (element.TryGetProperty("effects", out JsonElement effectsArray))
            {
                foreach (var effectJson in effectsArray.EnumerateArray())
                {
                    var effect = new EffectDescriptor(effectJson);
                    effects.Add(effect);
                }
            }

            Effects = effects.AsReadOnly();
        }

        #endregion

        // AssertScriptDeclaration
        public void AssertScriptDeclaration(GameSession session)
        {
            if (session.FindProceduralThing(Name) is null)
                RaiseValidationError(this, $"'{Name}' has no script declaration.");
        }

        // BaseDropChance
        public Ratio BaseDropChance { get; }

        // Effects
        public ReadOnlyCollection<EffectDescriptor> Effects { get; }

        // Faction
        public Faction Faction { get; init; }

        // GraceReward
        public int GraceReward { get; }

        // LootPool
        public ReadOnlyChanceTable LootPool { get; }

        // MaxPerRoom
        public int MaxPerRoom { get; }

        // PassesMaxPerRoomConstraint
        public bool PassesMaxPerRoomConstraint(int instanceCount)
        {
            return MaxPerRoom == -1 || instanceCount < MaxPerRoom;
        }

        // RoomTheme
        public RoomTheme? RoomTheme { get; }

        // SpawnScope
        public SpawnScope SpawnScope { get; }

        // TargetRoomCategory
        public RoomCategory? TargetRoomCategory { get; }
    }
}