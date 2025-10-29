using Engendro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// ChanceTable
    /// </summary>
    public sealed class ChanceTable
    {
        private readonly List<ChanceTableItem> items = [];
        private Random random = new();
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

        // Constructor
        public ChanceTable()
        {
            this.Items = new ReadOnlyCollection<ChanceTableItem>(items);
        }

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

        // GeValue
        public ChanceTableItem? GetValue(Func<ChanceTableItem, float> multiplier)
        {
            if (items.Count == 0)
                return null;

            float total = 0;
            
            // Calculate final weights
            for (int i = 0; i < items.Count; i++)
            {
                float w = items[i].Weight * (multiplier?.Invoke(items[i]) ?? 1f);

                if (w < 0)
                    w = 0;

                if (w > 0)
                    total += w;
            }

            if (total <= 0)
                return null;

            float r = (float)random.NextDouble() * total;
            
            for (int i = 0; i < items.Count; i++)
            {
                float w = items[i].Weight * (multiplier?.Invoke(items[i]) ?? 1f);
                if (w <= 0)
                    continue;
            
                if (r < w)
                    return items[i];
                
                r -= w;
            }

            return null;
        }

        // Items
        public ReadOnlyCollection<ChanceTableItem> Items { get; }

        // Nothing
        public const string Nothing = "<Nothing>";

        // SetSeed
        public void SetSeed(int seed) => random = new Random(seed);
    }
}
