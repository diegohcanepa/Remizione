using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// DataContainer
    /// </summary>
    public class DataContainer<T> where T : INamedObject
    {
        private FrozenDictionary<string, T> data = FrozenDictionary.Create<string, T>();
        private readonly List<T> dataList = [];
        private readonly Func<JsonElement, T> onCreate;

        // Constructor
        public DataContainer(Func<JsonElement, T> onCreate)
        {
            this.onCreate = onCreate;
            this.All = dataList.AsReadOnly();
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

            using var input = TitleContainer.OpenStream(fileName);
            using JsonDocument doc = JsonDocument.Parse(input);
            var root = doc.RootElement;

            if (!root.TryGetProperty("data", out JsonElement arrayElement) || arrayElement.ValueKind != JsonValueKind.Array)
                throw new InvalidDataException();

            var dict = new Dictionary<string, T>();

            foreach (JsonElement element in arrayElement.EnumerateArray())
            {
                var obj = onCreate(element);
                dict.Add(obj.Name, obj);
                dataList.Add(obj);
            }

            data = dict.ToFrozenDictionary();

            IsLoaded = true;
        }
    }
}