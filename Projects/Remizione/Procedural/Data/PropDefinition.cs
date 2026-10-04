using System.Text.Json;

namespace Remizione
{
    /// <summary>
    /// PropDefinition
    /// </summary>
    public sealed class PropDefinition : ThingDefinition
    {
        // Constructor
        public PropDefinition(JsonElement element)
            : base(element, true)
        {
        }
    }
}