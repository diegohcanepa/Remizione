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
            Definitions.Add(this);
        }

        // Definitions
        public static DataContainer<ActorDefinition> Definitions { get; } = new(element => new ActorDefinition(element));
    }
}