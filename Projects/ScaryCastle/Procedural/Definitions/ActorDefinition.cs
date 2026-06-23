using Engendro;
using System;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// ActorDefinition
    /// </summary>
    public sealed class ActorDefinition : ThingDefinition
    {
        // Constructor
        public ActorDefinition(JsonElement element)
            : base(element)
        {
            // Faction
            Faction = element.GetEnum("faction", Faction.Evil);

            // MinPackSize
            MinPackSize = element.GetInt32("minPackSize", 1);

            // MaxPackSize
            MaxPackSize = element.GetInt32("maxPackSize", 1);

            // Rank
            Rank = element.GetEnum("rank", ActorRank.Common);

            // TurnInterval
            TurnInterval = element.GetInt32("turnInterval", 2);

            // Name cannot be a category
            if (MinPackSize > MaxPackSize)
                RaiseValidationError(this, $"Minimum pack size exceeds the maximum pack size.");

            Definitions.Add(this);
        }

        // Definitions
        public static DataContainer<ActorDefinition> Definitions { get; } = new(element => new ActorDefinition(element));

        // MinPackSize
        public int MinPackSize { get; }

        // MaxPackSize
        public int MaxPackSize { get; }

        // Rank
        public ActorRank Rank { get; }

        // RollPackSize
        public int RollPackSize(Random rng)
        {
            return rng.Next(MinPackSize, MaxPackSize + 1);
        }

        // TurnInterval
        public int TurnInterval { get; }
    }
}