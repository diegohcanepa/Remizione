using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// Tags
    /// </summary>
    public sealed class Tags : ReadOnlyCollection<string>
    {
        private static readonly HashSet<string> allowedTags =
        [
            "ceiling",
            "vendingMachine",
            "flip",
            "floor",
            "poison",
            "pottery",
            "torch",
            "trap",
            "trunk",
            "wallDecoration",
        ];

        // Constructor
        public Tags(string? tags)
            : this(tags is null ? [] : tags.Split(','))
        {
        }

        // Constructor
        public Tags(IList<string> tags)
            : base(tags)
        {
            // Validate tags
            foreach (var tag in tags)
            {
                if (!allowedTags.Contains(tag))
                    throw new ArgumentException($"'{tag}' is not a valid tag.");
            }
        }

        // EmptyList
        public static Tags EmptyList { get; } = new Tags([]);

        // FromJson
        public static Tags FromJson(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out JsonElement tagsElement))
                return new(tagsElement.GetString());
            else
                return EmptyList;
        }

        // Intersects
        public bool Intersects(ReadOnlyCollection<string> tags)
        {
            if (Count == 0 || tags.Count == 0)
                return false;

            for (int i = 0; i < Count; i++)
            {
                var va = this[i];

                for (int j = 0; j < tags.Count; j++)
                {
                    if (string.Equals(va, tags[j], StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }

            return false;
        }
    }
}
