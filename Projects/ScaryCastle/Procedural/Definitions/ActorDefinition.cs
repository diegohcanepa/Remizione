using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// ActorDefinition
    /// </summary>
    public sealed class ActorDefinition : ThingDefinition
    {
        #region Constructor

        // Constructor
        public ActorDefinition(JsonElement element)
            : base(element)
        {
            Definitions.Add(this);
        }

        #endregion

        // Definitions
        public static DataContainer<ActorDefinition> Definitions { get; } = new(element => new ActorDefinition(element));
    }
}