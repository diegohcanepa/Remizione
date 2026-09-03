using Engendro;
using Engendro.Collections;
using Microsoft.Xna.Framework;
using System;
using System.Text.Json;

namespace ScaryCastle
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
            FillChance = element.GetFloat("fillChance", 1);
            FlipImage = element.GetBool("flipImage", false);
            SpawnRule = element.GetEnum("spawnRule", PlaceholderSpawnRule.Default);

            if (element.GetEnum<PlacementType>("placement") is { } placement)
                Placement = placement;
            else
                throw new InvalidOperationException("Undefined placement property.");

            if (element.GetString("position") is string positionValue && !string.IsNullOrWhiteSpace(positionValue))
                Position = DataConvert.ToVector2(positionValue);
        }

        // AllowTags
        public ReadOnlyEnumSet<Tag> AllowTags { get; }

        // FillChance
        public Ratio FillChance { get; set; }

        // FlipImage    
        public bool FlipImage { get; }

        // Placement
        public PlacementType Placement { get; }

        // Position
        public Vector2 Position { get; }

        // SpawnRule
        public PlaceholderSpawnRule SpawnRule { get; }
    }
}
