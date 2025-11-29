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
    public sealed class PropConfig : ThingConfig
    {
        private static readonly Dictionary<string, PropConfig> data = [];

        // Constructor
        private PropConfig(JsonElement element)
            : base(element)
        {
            data.Add(Name, this);
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
            Utils.LoadJsonData<PropConfig>(fileName, (JsonElement element) => new PropConfig(element));
        }

        // Validate
        public static void Validate()
        {
        }

        #endregion
    }
}
