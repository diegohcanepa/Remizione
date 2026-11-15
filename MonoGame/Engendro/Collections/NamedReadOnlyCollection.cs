using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Engendro.Collections
{
    /// <summary>
    /// NamedReadOnlyCollection
    /// </summary>
    public class NamedReadOnlyCollection<T>(IList<T> list) : ReadOnlyCollection<T>(list) where T : class, INamedObject
    {
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
