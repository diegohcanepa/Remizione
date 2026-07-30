using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// TagReadOnlyCollection
    /// </summary>
    public sealed class TagReadOnlyCollection : ReadOnlyCollection<Tag>
    {
        // Constructor
        public TagReadOnlyCollection(IList<Tag> tags)
            : base(tags)
        {
        }

        // EmptyList
        public static TagReadOnlyCollection EmptyList { get; } = new([]);

        // FromJson
        public static TagReadOnlyCollection FromJson(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out JsonElement tagsElement) && tagsElement.GetString() is string tags)
                return FromString(tags);
            else
                return EmptyList;
        }

        // FromString
        public static TagReadOnlyCollection FromString(string value)
        {
            var tagList = new List<Tag>();

            var tags = value.Split(',');
            foreach (var tagName in tags)
            {
                var newTag = Enum.Parse<Tag>(tagName);
                if (!tagList.Contains(newTag))
                    tagList.Add(newTag);
            }

            return tagList.Count == 0 ? EmptyList : new TagReadOnlyCollection(tagList);
        }

        // Intersects
        public bool Intersects(IList<Tag> tags)
        {
            if (Count == 0 || tags.Count == 0)
                return false;

            for (int i = 0; i < Count; i++)
            {
                for (int j = 0; j < tags.Count; j++)
                {
                    if (this[i] == tags[j])
                        return true;
                }
            }

            return false;
        }
    }
}
