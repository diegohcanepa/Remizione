using Engendro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// TraitDescriptor
    /// </summary>
    public sealed class TraitDescriptor : NamedDescriptor
    {
        // Constructor
        public TraitDescriptor(JsonElement element)
            : base(element)
        {
            this.Image = Atlases.UI.GetImage($"Trait{Name}Icon");
            this.Value = element.GetFloat("value", 0);

            Data.Add(this);
        }

        // Data
        public static DataContainer<TraitDescriptor> Data { get; } = new(element => new(element));

        // Image
        public AtlasImage Image { get; }

        // Value
        public float Value { get; }
    }
}
