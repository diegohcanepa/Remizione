using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// DefinitionContainer
    /// </summary>
    public sealed class DefinitionContainer<T> where T : Definition
    {
        private readonly Dictionary<string, T> data = [];
        private readonly List<T> dataList = [];
        private readonly Func<JsonElement, T> onCreate;

        // Constructor
        public DefinitionContainer(Func<JsonElement, T> onCreate)
        {
            this.onCreate = onCreate;
            this.All = dataList.AsReadOnly();
        }

        // Add
        public void Add(T definition)
        {
            data.Add(definition.Name, definition);
            dataList.Add(definition);
        }

        // All
        public ReadOnlyCollection<T> All { get; }

        // Find
        public T? Find(string name)
        {
            return data.TryGetValue(name, out T? definition) ? definition : null;
        }

        // Get
        public T Get(string name)
        {
            return data[name];
        }

        // Load
        public void Load(string fileName)
        {
            if (data.Count > 0)
                throw new InvalidOperationException("Data already loaded.");

            Utils.LoadJsonData(fileName, onCreate);
        }
    }
}