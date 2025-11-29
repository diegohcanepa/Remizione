using Engendro;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private EnemyConfig(JsonElement element)
            : base(element)
        {
            // MaxPerRoom
            if (element.TryGetProperty("maxPerRoom", out JsonElement maxPerRoomElement))
                MaxPerRoom = maxPerRoomElement.GetInt32();
        }

        #region Static members

        // GetConfig
        public static EnemyConfig? GetConfig(string propName)
        {
            return data.TryGetValue(propName, out EnemyConfig? config) ? config : null;
        }

        // Load
        public static void Load(string fileName)
        {
            var enemies = LoadCore<EnemyConfig>(fileName, "enemies", (JsonElement element) => new EnemyConfig(element));

            foreach (var enemyConfig in enemies)
            {
                data.Add(enemyConfig.Name, enemyConfig);
            }
        }

        #endregion

        // Data
        public static ReadOnlyDictionary<string, EnemyConfig> Data { get; } = new(data);

        // MaxPerRoom
        public int MaxPerRoom { get; }
    }
}
