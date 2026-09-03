using Engendro;
using System;
using System.Text.Json;

namespace Remizione
{
    /// <summary>
    /// TraitDefinition
    /// </summary>
    public sealed class TraitDefinition : Definition
    {
        // Constructor
        public TraitDefinition(JsonElement element)
            : base(element, NameValidationRule.Unique)
        {
            this.Image = Atlases.UI.GetImage($"Trait{Name}Icon");
            this.TraitType = Enum.Parse<TraitType>(Name);
            this.Value = element.GetFloat("value", 0);
        }

        // Image
        public AtlasImage Image { get; }

        // TraitType
        public TraitType TraitType { get; }

        // Value
        public float Value { get; }
    }
}
