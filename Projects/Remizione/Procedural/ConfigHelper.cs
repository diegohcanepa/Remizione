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
        // AssertMetaItem
        internal static void AssertMetaItem(string name)
        {
            if (name != ChanceTable.Nothing && MetaItem.Find(name) == null)
                throw new InvalidDataException($"Meta item {name} does not exist.");
        }

        // GetLootTable
        internal static ChanceTable GetLootTable(JsonElement parentElement)
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

                    AssertMetaItem(lootEntryName);

                    loot.Add(lootEntryName, lootEntryElement[1].GetInt32());
                }
            }

            return loot;
        }

        // GetTags
        internal static Tags GetTags(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out JsonElement tagsElement))
                return new(tagsElement.GetString());
            else
                return Tags.EmptyList;
        }
    }
}
