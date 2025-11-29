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
    public sealed class EnemyConfig : ThingConfig
    {
        private static readonly Dictionary<string, EnemyConfig> data = [];

        // Constructor
        private EnemyConfig(JsonElement element)
            : base(element)
        {
            data.Add(Name, this);
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
            Utils.LoadJsonData<EnemyConfig>(fileName, (JsonElement element) => new EnemyConfig(element));
        }

        #endregion

        // Data
        public static ReadOnlyDictionary<string, EnemyConfig> Data { get; } = new(data);
    }
}
