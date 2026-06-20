using Engendro;
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
        #region Private fields

        private readonly List<EffectDescriptor> effects = [];

        #endregion

        #region Constructor

        // Constructor
        protected ThingDefinition(JsonElement element)
            : base(element)
        {
            // DropCoinChanceBonus
            DropCoinChanceBonus = element.GetFloat("dropCoinChanceBonus", 0);

            // DropSackChanceBonus
            DropSackChanceBonus = element.GetFloat("dropSackChanceBonus", 0);

            // DropMode
            DropMode = element.GetEnum("dropMode", LootDropMode.Standard);

            // DropTrigger
            DropTrigger = element.GetEnum("dropTrigger", LootDropTrigger.OnDeath);

            // Faction
            Faction = element.GetEnum("faction", Faction.Evil);

            // MaxPerRoom
            MaxPerRoom = element.GetInt32("maxPerRoom", -1);
            if (MaxPerRoom < 0)
                MaxPerRoom = -1;

            // RequiresDeadEnd
            RequiresDeadEnd = element.GetBool("requiresDeadEnd", false);

            // RoomTheme
            RoomTheme = element.GetEnum<RoomTheme>("roomTheme");

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
            if (session.FindDeclaredThing(Name) == null)
                RaiseValidationError(this, "No script declaration.");
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

        // RequiresDeadEnd
        public bool RequiresDeadEnd { get; }

        // RoomTheme
        public RoomTheme? RoomTheme { get; }
    }
}