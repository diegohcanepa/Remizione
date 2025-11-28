using Engendro;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Remizione
{
    /// <summary>
    /// ConfigHelper
    /// </summary>
    internal static class ConfigHelper
    {
        // GetStringArrayValues
        private static List<string> GetStringArrayValues(JsonElement parentElement, string propertyName)
        {
            var result = new List<string>();

            if (parentElement.TryGetProperty(propertyName, out JsonElement property))
            {
                foreach (JsonElement element in property.EnumerateArray())
                {
                    if (element.GetString() is not string name)
                        throw new InvalidDataException();

                    result.Add(name);
                }
            }

            return result;
        }

        // GetLoot
        internal static ChanceTable GetLoot(JsonElement parentElement)
        {
            var loot = new ChanceTable();

            if (parentElement.TryGetProperty("loot", out JsonElement lootElement))
            {
                foreach (JsonElement lootEntryElement in lootElement.EnumerateArray())
                {
                    if (lootEntryElement.ValueKind != JsonValueKind.Array || lootEntryElement.GetArrayLength() != 2)
                        throw new InvalidDataException("Invalid loot entry definition.");

                    if (lootEntryElement[0].GetString() is not string lootEntryName)
                        throw new InvalidDataException("Loot entry name not found.");

                    loot.Add(lootEntryName, lootEntryElement[1].GetInt32());
                }
            }

            return loot;
        }

        // GetScopeRule
        internal static RoomConfigScopeRule GetScopeRule(JsonElement roomElement, string propertyName)
        {
            static void Populate(JsonElement tags, List<string> list)
            {
                foreach (var tag in tags.EnumerateArray())
                {
                    if (tag.GetString() is string tagName)
                        list.Add(tagName);
                }
            }

            var allowPools = new List<string>();
            var denyPools = new List<string>();

            var allowTags = new List<string>();
            var denyTags = new List<string>();

            if (roomElement.TryGetProperty(propertyName, out JsonElement rulesElement))
            {
                // Allow pools
                if (rulesElement.TryGetProperty("allowPools", out JsonElement allowPoolsElement))
                    Populate(allowPoolsElement, allowPools);

                // Deny pools
                if (rulesElement.TryGetProperty("denyPools", out JsonElement denyPoolsElement))
                    Populate(denyPoolsElement, denyPools);

                // Allow tags
                if (rulesElement.TryGetProperty("allowTags", out JsonElement allowTagsElement))
                    Populate(allowTagsElement, allowTags);

                // Deny tags
                if (rulesElement.TryGetProperty("denyTags", out JsonElement denyTagsElement))
                    Populate(denyTagsElement, denyTags);
            }

            return new RoomConfigScopeRule(allowPools, denyPools, allowTags, denyTags);
        }

        // GetPools
        internal static List<string> GetPools(JsonElement parentElement)
        {
            return GetStringArrayValues(parentElement, "pools");
        }

        // GetTags
        internal static List<string> GetTags(JsonElement parentElement)
        {
            return GetStringArrayValues(parentElement, "tags");
        }
    }
}
