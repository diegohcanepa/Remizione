using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ScaryCastle.Procedural.Definitions
{
    /// <summary>
    /// ActorDefinition
    /// </summary>
    public sealed class ActorDefinition : ThingDefinition
    {
        #region Private fields

        private static readonly Dictionary<string, ActorDefinition> data = [];
        private static readonly List<ActorDefinition> dataList = [];

        #endregion

        #region Static members

        // All
        public static ReadOnlyCollection<ActorDefinition> All { get; } = new(dataList);

        // Find
        public static ActorDefinition? Find(string name)
        {
            return data.TryGetValue(name, out ActorDefinition? definition) ? definition : null;
        }

        // Get
        public static ActorDefinition Get(string name)
        {
            return data[name];
        }

        // Load
        public static void Load(params string[] fileNames)
        {
            if (data.Count > 0)
                throw new InvalidOperationException("Data already loaded.");

            for (var i = 0; i < fileNames.Length; i++)
            {
                Utils.LoadJsonData<ActorDefinition>(fileNames[i], element => new ActorDefinition(element));
            }
        }

        #endregion
    }
}