using Engendro;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// ChanceTable
    /// </summary>
    public sealed class ChanceTable
    {
        private readonly List<ChanceTableItem> items = [];
        private readonly Random random = new();
        private static readonly Dictionary<string, ChanceTable> tables = [];

        #region Static members

        // Find
        public static ChanceTable? Find(string name) => tables.TryGetValue(name, out var result) ? result : null;

        // Register
        public static ChanceTable Register(string name)
        {
            if (tables.ContainsKey(name))
                throw new InvalidOperationException($"Table '{name}' already exists.");

            var result = new ChanceTable();
            tables[name] = result;

            return result;
        }

        #endregion

        // Add
        public void Add(string value, int amount, float weight)
        {
            if (string.IsNullOrEmpty(value))
                return;

            CodeContract.GreaterThanZero(weight, nameof(weight));
            items.Add(new(value, amount, weight));
        }

        // Count
        public int Count => items.Count;

        // GetValue
        public ChanceTableItem? GetValue() => GetValue(random);

        // GetValue
        public ChanceTableItem? GetValue(Random random)
        {
            if (items.Count == 0)
                return null;

            float totalWeight = 0f;

            for (var i = 0; i < items.Count; i++)
            {
                totalWeight += items[i].Weight;
            }

            float roll = (float)random.NextDouble() * totalWeight;

            for (var i = 0; i < items.Count; i++)
            {
                if (roll < items[i].Weight)
                    return items[i];

                roll -= items[i].Weight;
            }

            return null;
        }

        // Nothing
        public const string Nothing = "<Nothing>";
    }
}
