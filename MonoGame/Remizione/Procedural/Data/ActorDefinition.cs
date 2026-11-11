using Engendro;
using System;
using System.Text.Json;

namespace Remizione
{
    /// <summary>
    /// ActorDefinition
    /// </summary>
    public sealed class ActorDefinition : ThingDefinition
    {
        // Constructor
        public ActorDefinition(JsonElement element)
            : base(element, false)
        {
            // Faction
            Faction = element.GetEnum("faction", Faction.Neutral);

            // MinPackSize
            MinPackSize = element.GetInt32("minPackSize", 1);

            // MaxPackSize
            MaxPackSize = element.GetInt32("maxPackSize", 1);

            // Name cannot be a category
            if (MinPackSize > MaxPackSize)
                RaiseValidationError(this, $"Minimum pack size exceeds the maximum pack size.");
        }

        // Faction
        public Faction Faction { get; }

        // MinPackSize
        public int MinPackSize { get; }

        // MaxPackSize
        public int MaxPackSize { get; }

        // RollPackSize
        public int RollPackSize(Random rng)
        {
            return rng.Next(MinPackSize, MaxPackSize + 1);
        }
    }
}