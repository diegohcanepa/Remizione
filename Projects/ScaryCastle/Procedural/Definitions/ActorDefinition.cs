using Engendro;
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

            // GroupCount
            GroupCount = element.GetInt32("groupCount", 0);

            Definitions.Add(this);
        }

        // Definitions
        public static DataContainer<ActorDefinition> Definitions { get; } = new(element => new ActorDefinition(element));

        // GroupCount
        public int GroupCount { get; }
    }
}