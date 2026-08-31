using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Engendro.Collections
{
    /// <summary>
    /// Conjunto de solo lectura optimizado para valores de tipo Enum.
    /// </summary>
    public sealed class ReadOnlyEnumSet<T> : ReadOnlyCollection<T> where T : struct, Enum
    {
        #region Constructors

        public ReadOnlyEnumSet(IList<T> items)
            : base(items)
        {
        }

        #endregion

        // Empty
        public static new ReadOnlyEnumSet<T> Empty { get; } = new(Array.Empty<T>());

        // FromString
        public static ReadOnlyEnumSet<T> FromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Empty;

            var list = new List<T>();

            foreach (var segment in value.AsSpan().Split(','))
            {
                var entry = value.AsSpan(segment).Trim();
                if (entry.IsEmpty)
                    continue;

                var parsedEnum = Enum.Parse<T>(entry, ignoreCase: false);

                if (!list.Contains(parsedEnum))
                    list.Add(parsedEnum);
            }

            return list.Count == 0 ? Empty : new ReadOnlyEnumSet<T>(list);
        }

        // FromJson
        public static ReadOnlyEnumSet<T> FromJson(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement tagsElement))
                throw new KeyNotFoundException($"Required JSON property '{propertyName}' was not found.");

            if (tagsElement.ValueKind != JsonValueKind.String || tagsElement.GetString() is not string tags)
                throw new JsonException($"Property '{propertyName}' in JSON element is not a valid string.");

            return FromString(tags);
        }

        // FromJsonOrEmpty
        public static ReadOnlyEnumSet<T> FromJsonOrEmpty(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out JsonElement tagsElement) &&
                tagsElement.ValueKind == JsonValueKind.String &&
                tagsElement.GetString() is string tags)
            {
                return FromString(tags);
            }

            return Empty;
        }

        // Intersects
        public bool Intersects(IList<T> other)
        {
            if (Count == 0 || other is null || other.Count == 0)
                return false;

            for (int i = 0; i < Count; i++)
            {
                T item = this[i];
                for (int j = 0; j < other.Count; j++)
                {
                    if (EqualityComparer<T>.Default.Equals(item, other[j]))
                        return true;
                }
            }

            return false;
        }
    }
}