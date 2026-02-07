using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// PropDefinition
    /// </summary>
    public sealed class PropDefinition : ThingDefinition
    {
        #region Constructor

        // Constructor
        public PropDefinition(JsonElement element)
            : base(element)
        {
            Definitions.Add(this);
        }

        #endregion

        // Definitions
        public static DefinitionContainer<PropDefinition> Definitions { get; } = new(element => new PropDefinition(element));
    }
}