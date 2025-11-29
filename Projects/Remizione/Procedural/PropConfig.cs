using Engendro;
using Microsoft.Xna.Framework;
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
        private PropConfig(JsonElement element)
            : base(element)
        {
            // MaxPerRoom
            if (element.TryGetProperty("maxPerRoom", out JsonElement maxPerRoomElement))
                MaxPerRoom = maxPerRoomElement.GetInt32();
        }

        #region Static members

        // GetConfig
        public static PropConfig? GetConfig(string propName)
        {
            return data.TryGetValue(propName, out PropConfig? config) ? config : null;
        }

        // Load
        public static void Load(string fileName)
        {
            var props = LoadCore<PropConfig>(fileName, "props", (JsonElement element) => new PropConfig(element));

            foreach (var propConfig in props)
            {
                data.Add(propConfig.Name, propConfig);
            }
        }

        // Validate
        public static void Validate()
        {
        }

        #endregion

        // MaxPerRoom
        public int MaxPerRoom { get; }
    }
}
