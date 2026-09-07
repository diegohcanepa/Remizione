using System;
using System.Collections;
using System.Collections.Generic;

namespace Engendro.Collections
{
    /// <summary>
    /// ReadOnlyChanceTable
    /// </summary>
    public sealed class ReadOnlyChanceTable : IReadOnlyList<ChanceTableItem>
    {
        private readonly ChanceTable table;

        // Constructor
        public ReadOnlyChanceTable(ChanceTable table)
        {
            this.table = table;
        }

        // Count
        public int Count => table.Count;

        // Find
        public ChanceTableItem? Find(string name)
        {
            return table.Find(name);
        }

        // GetEnumerator
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // GetEnumerator
        public IEnumerator<ChanceTableItem> GetEnumerator()
        {
            return table.GetEnumerator();
        }

        // GetItem
        public ChanceTableItem? GetItem()
        {
            return table.GetItem();
        }

        // GetItem
        public ChanceTableItem? GetItem(Random random)
        {
            return table.GetItem(random);
        }

        // Indexer
        public ChanceTableItem this[int index] => table[index];
    }
}