using Microsoft.Xna.Framework;
using Remizione.Procedural;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace Remizione
{
    /// <summary>
    /// PropConfig
    /// </summary>
    public sealed class PropConfig : Config
    {
        private static readonly Dictionary<string, PropConfig> data = [];

        // Constructor
        private PropConfig(string name, IList<string> tags, ChanceTable lootTable, IList<string> pools, float weight, PlaceholderSize placeholderSize)
            : base(name, tags, lootTable)
        {
            this.Pools = new(pools);
            this.Weight = weight;
            this.PlaceholderSize = placeholderSize;
        }

        #region Static members

        // GetConfig
        public static PropConfig GetConfig(string propName)
        {
            return data[propName];
        }

        // Load
        public static void Load(string fileName)
        {
            try
            {
                using var input = TitleContainer.OpenStream(fileName);
                using JsonDocument doc = JsonDocument.Parse(input);
                var root = doc.RootElement;

                if (!root.TryGetProperty("props", out JsonElement propsArray) || propsArray.ValueKind != JsonValueKind.Array)
                    throw new InvalidDataException("Prop is not an array.");

                foreach (JsonElement propElement in propsArray.EnumerateArray())
                {
                    // Name
                    if (propElement.GetProperty("name").GetString() is not string propName)
                        throw new InvalidDataException("Prop name not found.");

                    // PlaceholderSize
                    PlaceholderSize placeholderSize = PlaceholderSize.Any;
                    if (propElement.TryGetProperty("placeholderSize", out JsonElement placeholderSizeElement))
                    {
                        if (placeholderSizeElement.GetString() is string placeholderSizeValue)
                            placeholderSize = Enum.Parse<PlaceholderSize>(placeholderSizeValue);
                    }

                    // Weight
                    float weight = 1;
                    if (propElement.TryGetProperty("weight", out JsonElement weightElement))
                        weight = weightElement.GetSingle();

                    // Tags
                    var tags = ConfigHelper.GetTags(propElement);

                    // Loot
                    var loot = ConfigHelper.GetLoot(propElement);

                    // Pools
                    var pools = ConfigHelper.GetPools(propElement);

                    // Add configuration
                    var propConfig = new PropConfig(propName, tags, loot, pools, weight, placeholderSize);
                    data.Add(propName, propConfig);
                }
            }
            catch (FileNotFoundException)
            {
            }
        }

        #endregion

        // PlaceholderSize
        public PlaceholderSize PlaceholderSize { get; }

        // Pools
        public ReadOnlyCollection<string> Pools { get; }

        // Weight
        public float Weight { get; }
    }
}
