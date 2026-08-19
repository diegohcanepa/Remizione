using Engendro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// DataContainer
    /// </summary>
    public class DataContainer<T> where T : INamedObject
    {
        private readonly Dictionary<string, T> data = [];
        private readonly List<T> dataList = [];
        private readonly Func<JsonElement, T> onCreate;

        // Constructor
        public DataContainer(Func<JsonElement, T> onCreate)
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
            return data.TryGetValue(name, out T? definition) ? definition : default;
        }

        // Get
        public T Get(string name)
        {
            return data[name];
        }

        // IsLoaded
        public bool IsLoaded { get; private set; }

        // Load
        public void Load(string fileName)
        {
            if (data.Count > 0)
                throw new InvalidOperationException("Data already loaded.");

            Utils.LoadJsonData(fileName, onCreate);

            IsLoaded = true;
        }
    }
}