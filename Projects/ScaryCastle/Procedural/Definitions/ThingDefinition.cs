using Engendro;
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
        #region Private fields

        private readonly List<EffectDescriptor> effects = [];
        private readonly List<PlacementType> placements = [];

        #endregion

        #region Constructor

        // Constructor
        protected ThingDefinition(JsonElement element)
            : base(element)
        {
            // AllowStartRoomSpawn
            AllowStartRoomSpawn = element.GetBool("allowStartRoomSpawn", false);

            // MaxPerRoom
            MaxPerRoom = element.GetInt32("maxPerRoom", -1);
            if (MaxPerRoom < 0)
                MaxPerRoom = -1;

            // MaxSpawnAmount
            MaxSpawnAmount = element.GetInt32("maxSpawnAmount", 1);
            if (MaxSpawnAmount < 0)
                MaxSpawnAmount = 1;

            // MinSpawnAmount
            MinSpawnAmount = element.GetInt32("minSpawnAmount", 1);
            if (MinSpawnAmount < 0)
                MinSpawnAmount = 1;

            if (MinSpawnAmount > MaxSpawnAmount)
                RaiseValidationError(this, $"{nameof(MinSpawnAmount)} cannot be greater than {nameof(MaxSpawnAmount)}.");

            RequiresDeadEnd = element.GetBool("requiresDeadEnd", false);

            // Placements
            if (element.TryGetProperty("placements", out JsonElement placementsElement))
            {
                foreach (var item in placementsElement.EnumerateArray())
                {
                    if (Enum.TryParse<PlacementType>(item.GetString(), out var value))
                    {
                        placements.Add(value);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Cannot parse placement value.");
                    }
                }
            }

            this.Placements = placements.AsReadOnly();

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

        // AllowStartRoomSpawn
        public bool AllowStartRoomSpawn { get; }

        // AssertScriptDeclaration
        public void AssertScriptDeclaration(GameSession session)
        {
            if (session.FindDeclaredThing(Name) == null)
                RaiseValidationError(this, "No script declaration.");
        }

        // Effects
        public ReadOnlyCollection<EffectDescriptor> Effects { get; }

        // MaxPerRoom
        public int MaxPerRoom { get; }

        // MaxSpawnAmount
        public int MaxSpawnAmount { get; }

        // MinSpawnAmount
        public int MinSpawnAmount { get; }

        // PassesMaxPerRoomConstraint
        public bool PassesMaxPerRoomConstraint(int instanceCount)
        {
            return MaxPerRoom == -1 || instanceCount < MaxPerRoom;
        }

        // Placements
        public ReadOnlyCollection<PlacementType> Placements { get; }

        // RequiresDeadEnd
        public bool RequiresDeadEnd { get; }
    }
}