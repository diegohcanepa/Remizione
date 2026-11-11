using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Engendro.Collections
{
    /// <summary>
    /// NamedCollection
    /// </summary>
    public class NamedCollection<T> : Collection<T> where T : class, INamedObject
    {
        #region Constructors

        // Constructor
        public NamedCollection()
            : base()
        {
        }

        // Constructor
        public NamedCollection(IList<T> list)
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

        // SetItem
        protected override void SetItem(int index, T item)
        {
            var existingIndex = IndexOf(item.Name);

            // Si el nombre ya existe y NO es el elemento que estamos reemplazando
            if (existingIndex != -1 && existingIndex != index)
                CodeContract.ThrowDuplicatedNameException(nameof(item));

            base.SetItem(index, item);
        }

        #endregion

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
