using Engendro;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Remizione
{
    /// <summary>
    /// EnemyConfig
    /// </summary>
    public sealed class EnemyConfig : Config
    {
        private static readonly Dictionary<string, EnemyConfig> data = [];

        // Constructor
        private EnemyConfig(string name, IList<string> tags, ChanceTable lootTable)
            : base(name, tags, lootTable)
        {
        }

        #region Static members

        // GetConfig
        public static EnemyConfig GetConfig(string propName)
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

                    // Tags
                    var tags = ConfigHelper.GetTags(propElement);

                    // Loot
                    var loot = ConfigHelper.GetLoot(propElement);

                    // Add configuration
                    var enemyConfig = new EnemyConfig(propName, tags, loot);
                    data.Add(propName, enemyConfig);
                }
            }
            catch (FileNotFoundException)
            {
            }
        }

        #endregion
    }
}
