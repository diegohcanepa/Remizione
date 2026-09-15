using Engendro;
using Engendro.Collections;
using Microsoft.Xna.Framework;
using System;
using System.Text.Json;

namespace Remizione
{
    /// <summary>
    /// Placeholder
    /// </summary>
    public sealed class Placeholder
    {
        // Constructor
        public Placeholder(JsonElement element)
        {
            AllowTags = ReadOnlyEnumSet<Tag>.FromJsonOrEmpty(element, "allowTags");
            ContentType = element.GetEnum("contentType", PlaceholderContentType.Prop);
            FillChance = element.GetFloat("fillChance", 1);

            if (element.GetString("position") is string positionValue && !string.IsNullOrWhiteSpace(positionValue))
                Position = DataConvert.ToVector2(positionValue);
        }

        // AllowTags
        public ReadOnlyEnumSet<Tag> AllowTags { get; }

        // ContentType
        public PlaceholderContentType ContentType { get; }

        // FillChance
        public Ratio FillChance { get; set; }

        // Position
        public Vector2 Position { get; }
    }
}
