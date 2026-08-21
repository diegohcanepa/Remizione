using Engendro;
using System;
using System.Text.Json;

namespace ScaryCastle
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

        // Data
        public static DataContainer<TraitDefinition> Data { get; } = new(element => new(element));

        // Image
        public AtlasImage Image { get; }

        // TraitType
        public TraitType TraitType { get; }

        // Value
        public float Value { get; }
    }
}
