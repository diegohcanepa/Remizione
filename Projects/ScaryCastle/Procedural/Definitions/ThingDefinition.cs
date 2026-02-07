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

        private static readonly Dictionary<string, ThingDefinition> data = [];
        private static readonly List<ThingDefinition> dataList = [];
        private readonly List<EffectDescriptor> effects = [];
        private readonly List<PlacementType> placements = [];

        #endregion

        #region Constructor

        // Constructor
        protected ThingDefinition(JsonElement element)
            : base(element)
        {
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
                throw new InvalidOperationException($"[{Name}]: {nameof(MinSpawnAmount)} cannot be greater than MaxSpawnAmount.");

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

        // Get
        public static ThingDefinition Get(string name)
        {
            return data[name];
        }

        // Load
        public static void Load(params string[] fileNames)
        {
            if (data.Count > 0)
                throw new InvalidOperationException("Data already loaded.");

            for (var i = 0; i < fileNames.Length; i++)
            {
                Utils.LoadJsonData<ThingDefinition>(fileNames[i], element => new ThingDefinition(element));
            }
        }

        #endregion

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

        // Validate
        public override void Validate(GameSession session)
        {
            base.Validate(session);

            if (session.FindDeclaredThing(Name) == null)
                throw new InvalidOperationException($"[{Name}] has no script declaration.");
        }
    }
}