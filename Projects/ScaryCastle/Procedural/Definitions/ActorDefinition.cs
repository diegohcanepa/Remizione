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
            Faction = element.GetEnum("faction", Faction.Evil);

            if (element.GetEnum<ActorRole>("role") is ActorRole role)
                this.Role = role;
            else
                RaiseValidationError(this, "Undefined role.", "Role");

            Definitions.Add(this);
        }

        // Definitions
        public static DataContainer<ActorDefinition> Definitions { get; } = new(element => new ActorDefinition(element));

        // Faction
        public Faction Faction { get; }

        // Role
        public ActorRole Role { get; }
    }
}