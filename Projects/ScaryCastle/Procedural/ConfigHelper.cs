using Engendro;
using System.IO;
using System.Text.Json;

namespace ScaryCastle
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
