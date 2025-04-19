using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Engendro
{
    /// <summary>
    /// NamedObjectCollection
    /// </summary>
    public class NamedObjectCollection<T> : Collection<T> where T : class, INamedObject
    {
        #region Constructors

        // Constructor
        public NamedObjectCollection()
            : base()
        {
        }

        // Constructor
        public NamedObjectCollection(IList<T> list)
            : base(list)
        {
        }

        #endregion

        #region Protected members

        // InsertItem
        protected override void InsertItem(int index, T item)
        {
            if (Contains(item.Name))
                CodeContract.ThrowDuplicatedNameException(nameof(item));

            base.InsertItem(index, item);
        }

        #endregion

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

        // Remove
        public bool Remove(string name)
        {
            var item = Find(name);

            if (item != null)
                Remove(item);

            return item != null;
        }
    }
}
