using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Engendro
{
    /// <summary>
    /// NamedObjectReadOnlyCollection
    /// </summary>
    public class NamedObjectReadOnlyCollection<T> : ReadOnlyCollection<T> where T : class, INamedObject
    {
        // Constructor
        public NamedObjectReadOnlyCollection(IList<T> list)
            : base(list)
        {
        }

        // Contains
        public bool Contains(string name)
        {
            return NamedObjectCollectionHelper.Contains(this, name);
        }

        // Find
        public T? Find(string name)
        {
            return NamedObjectCollectionHelper.Find(this, name);
        }

        // IndexOf
        public int IndexOf(string name)
        {
            return NamedObjectCollectionHelper.IndexOf(this, name);
        }

        // ToArray
        public T[] ToArray()
        {
            return Items.ToArray();
        }
    }
}
