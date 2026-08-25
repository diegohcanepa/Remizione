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
        private readonly FrozenDictionary<string, T> data = FrozenDictionary.Create<string, T>();

        // Constructor
        public DataContainer(FrozenDictionary<string, T> data)
        {
            this.data = data;
            this.All = data.Values.AsReadOnly();
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
    }
}