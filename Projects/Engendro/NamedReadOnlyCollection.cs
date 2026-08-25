using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Engendro
{
    /// <summary>
    /// NamedReadOnlyCollection
    /// </summary>
    public class NamedReadOnlyCollection<T> : ReadOnlyCollection<T> where T : class, INamedObject
    {
        // Constructor
        public NamedReadOnlyCollection(IList<T> list)
            : base(list)
        {
        }

        // Contains
        public bool Contains(string name)
        {
            return Find(name) != null;
        }

        // Find
        public T? Find(string name)
        {
            for (var i = 0; i < Count; i++)
            {
                if (this[i].Name == name)
                    return this[i];
            }

            return default;
        }

        // IndexOf
        public int IndexOf(string name)
        {
            for (var i = 0; i < Count; i++)
            {
                if (this[i].Name == name)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
