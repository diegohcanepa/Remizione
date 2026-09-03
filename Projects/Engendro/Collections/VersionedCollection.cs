using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Engendro
{
    /// <summary>
    /// VersionedCollection
    /// </summary>
    public abstract class VersionedCollection<T> : Collection<T>
    {
        private readonly HashSet<T> hashSet;

        // Constructor
        protected VersionedCollection()
            : base()
        {
            hashSet = [];
        }

        #region Protected members

        // ClearItems
        protected override void ClearItems()
        {
            base.ClearItems();
            hashSet.Clear();
            Version++;
        }

        // InsertItem
        protected override void InsertItem(int index, T item)
        {
            if (!hashSet.Add(item))
                throw new InvalidOperationException("Duplicates not allowed.");

            base.InsertItem(index, item);
            Version++;
        }

        // RemoveItem
        protected override void RemoveItem(int index)
        {
            T item = this[index];
            base.RemoveItem(index);
            hashSet.Remove(item);
            Version++;
        }

        // SetItem
        protected override void SetItem(int index, T item)
        {
            T oldItem = this[index];

            // Si es el mismo objeto en la misma posición, no hay cambio de estado
            if (EqualityComparer<T>.Default.Equals(oldItem, item))
                return;

            if (hashSet.Contains(item))
                throw new InvalidOperationException("Duplicates not allowed.");

            base.SetItem(index, item);
            hashSet.Remove(oldItem);
            hashSet.Add(item);
            Version++;
        }

        #endregion

        // Version
        public int Version { get; set; }
    }
}