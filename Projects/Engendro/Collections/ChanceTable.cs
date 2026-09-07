using System;
using System.Collections.ObjectModel;

namespace Engendro.Collections
{
    /// <summary>
    /// ChanceTable
    /// </summary>
    public sealed class ChanceTable : Collection<ChanceTableItem>
    {
        // Add
        public void Add(string name, float weight, object? context = null)
        {
            if (string.IsNullOrEmpty(name))
                return;

            Add(new(name, weight, context));
        }

        // AsReadOnly
        public ReadOnlyChanceTable AsReadOnly()
        {
            return new ReadOnlyChanceTable(this);
        }

        // Find
        public ChanceTableItem? Find(string name)
        {
            for (var i = 0; i < Count; i++)
            {
                if (this[i].Name.CompareTo(name, StringComparison.Ordinal) == 0)
                    return this[i];
            }

            return null;
        }

        // GetItem
        public ChanceTableItem? GetItem()
        {
            return GetItem(Random.Shared);
        }

        // GetItem
        public ChanceTableItem? GetItem(Random random)
        {
            if (Count == 0)
                return null;

            float totalWeight = 0f;
            for (var i = 0; i < Count; i++)
            {
                totalWeight += this[i].Weight;
            }

            if (totalWeight <= 0f)
                return null;

            // Tirada en float
            float roll = (float)random.NextDouble() * totalWeight;

            for (var i = 0; i < Count; i++)
            {
                if (roll < this[i].Weight)
                    return this[i];

                roll -= this[i].Weight;
            }

            // Fallback defensivo imprescindible con float por redondeos decimales
            return this[Count - 1];
        }

        // Nothing
        public const string Nothing = "<Nothing>";

        // Remove
        public bool Remove(string name)
        {
            if (Find(name) is ChanceTableItem item)
            {
                Remove(item);
                return true;
            }

            return false;
        }
    }
}
