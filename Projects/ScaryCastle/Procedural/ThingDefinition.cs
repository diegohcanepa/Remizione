using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// ThingDefinition
    /// </summary>
    public sealed class ThingDefinition : EntityDefinition
    {
        private static readonly Dictionary<string, ThingDefinition> data = [];
        private static readonly List<ThingDefinition> dataList = [];
        private readonly List<EffectDefinition> effects = [];
        private readonly List<PlacementType> placements = [];

        #region Constructor

        // Constructor
        public ThingDefinition(JsonElement element)
            : base(element)
        {
            // MaxPerRoom
            MaxPerRoom = -1;
            if (element.TryGetProperty("maxPerRoom", out JsonElement maxPerRoomElement))
                MaxPerRoom = Math.Max(MaxPerRoom, maxPerRoomElement.GetInt32());

            // MaxSpawnAmount
            MaxSpawnAmount = 1;
            if (element.TryGetProperty("maxSpawnAmount", out JsonElement maxSpawnAmountElement))
                MaxSpawnAmount = Math.Max(1, maxSpawnAmountElement.GetInt32());

            // MinSpawnAmount
            MinSpawnAmount = 1;
            if (element.TryGetProperty("minSpawnAmount", out JsonElement minSpawnAmountElement))
                MinSpawnAmount = Math.Max(1, minSpawnAmountElement.GetInt32());

            if (MinSpawnAmount > MaxSpawnAmount)
                throw new InvalidOperationException($"[{Name}]: {nameof(MinSpawnAmount)} cannot be greater than MaxSpawnAmount.");

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
                        throw new InvalidOperationException($"Cannot parse placement value.");
                }
            }

            // RequiresDeadEnd
            if (element.TryGetProperty("requiresDeadEnd", out JsonElement requiresDeadEndElement))
                RequiresDeadEnd = requiresDeadEndElement.GetBoolean();

            this.Placements = placements.AsReadOnly();

            if (element.TryGetProperty("effects", out JsonElement effectsArray))
            {
                foreach (var effectJson in effectsArray.EnumerateArray())
                {
                    effects.Add(new(effectJson));
                }
            }

            Effects = effects.AsReadOnly();

            data.Add(Name, this);
            dataList.Add(this);
        }

        #endregion

        #region Static members

        // All
        public static ReadOnlyCollection<ThingDefinition> All { get; } = new(dataList);

        // Find
        public static ThingDefinition? Find(string name)
        {
            return data.TryGetValue(name, out ThingDefinition? definition) ? definition : null;
        }

        // Load
        public static void Load(params string[] fileNames)
        {
            for (var i = 0; i < fileNames.Length; i++)
            {
                Utils.LoadJsonData<ThingDefinition>(fileNames[i], element => new ThingDefinition(element));
            }
        }

        #endregion

        // Effects
        public ReadOnlyCollection<EffectDefinition> Effects { get; }

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

        // Validate
        public override void Validate(GameSession session)
        {
            base.Validate(session);

            if (session.FindDeclaredThing(Name) == null)
                throw new InvalidOperationException($"[{Name}] has no script declaration.");
        }
    }
}