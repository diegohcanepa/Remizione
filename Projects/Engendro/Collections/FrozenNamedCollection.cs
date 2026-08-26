using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Engendro.Collections
{
    /// <summary>
    /// FrozenNamedCollection
    /// </summary>
    public sealed class FrozenNamedCollection<T> : ReadOnlyCollection<T> where T : INamedObject
    {
        private readonly FrozenDictionary<string, T> data;

        // Constructor
        public FrozenNamedCollection(IList<T> items)
            : base(items.ToArray())
        {
            var dict = new Dictionary<string, T>();
            for (var i = 0; i < items.Count; i++)
            {
                dict.Add(items[i].Name, items[i]);
            }

            this.data = dict.ToFrozenDictionary();
        }

        // Contains
        public bool Contains(string name)
        {
            return Find(name) != null;
        }

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