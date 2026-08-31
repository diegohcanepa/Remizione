using Engendro;
using Engendro.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// ThingDefinition
    /// </summary>
    public abstract class ThingDefinition : EntityDefinition
    {
        private readonly List<EffectDescriptor> effects = [];

        #region Constructor

        // Constructor
        protected ThingDefinition(JsonElement element)
            : base(element)
        {
            // AllowedRoomCategories
            AllowedRoomCategories = ReadOnlyEnumSet<RoomCategory>.FromJsonOrEmpty(element, "allowedRoomCategories");

            // DropCoinChanceBonus
            DropCoinChanceBonus = MathF.Max(0, element.GetFloat("dropCoinChanceBonus", 0));

            // DropSackChanceBonus
            DropSackChanceBonus = MathF.Max(0, element.GetFloat("dropSackChanceBonus", 0));

            // DropMode
            DropMode = element.GetEnum("dropMode", LootDropMode.Standard);

            // DropTrigger
            DropTrigger = element.GetEnum("dropTrigger", LootDropTrigger.OnDeath);

            // Faction
            Faction = element.GetEnum("faction", Faction.Evil);

            // MaxPerRoom
            MaxPerRoom = element.GetInt32("maxPerRoom", -1);

            // RoomTheme
            RoomTheme = element.GetEnum<RoomTheme>("roomTheme");

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

        // AllowedRoomCategories
        public ReadOnlyEnumSet<RoomCategory> AllowedRoomCategories { get; }

        // AssertScriptDeclaration
        public void AssertScriptDeclaration(GameSession session)
        {
            if (session.FindProceduralThing(Name) is null)
                RaiseValidationError(this, $"'{Name}' has no script declaration.");
        }

        // DropMode
        public LootDropMode DropMode { get; }

        // DropTrigger
        public LootDropTrigger DropTrigger { get; }

        // DropCoinChanceBonus
        public Ratio DropCoinChanceBonus { get; }

        // DropSackChanceBonus
        public Ratio DropSackChanceBonus { get; }

        // Effects
        public ReadOnlyCollection<EffectDescriptor> Effects { get; }

        // Faction
        public Faction Faction { get; init; }

        // MaxPerRoom
        public int MaxPerRoom { get; }

        // PassesMaxPerRoomConstraint
        public bool PassesMaxPerRoomConstraint(int instanceCount)
        {
            return MaxPerRoom == -1 || instanceCount < MaxPerRoom;
        }

        // RoomTheme
        public RoomTheme? RoomTheme { get; }
    }
}