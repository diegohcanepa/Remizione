using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Engendro
{
    /// <summary>
    /// ChanceTable
    /// </summary>
    public sealed class ChanceTable
    {
        private readonly List<ChanceTableItem> items = [];
        private Random random = new();

        // Constructor
        public ChanceTable()
        {
            this.Items = new ReadOnlyCollection<ChanceTableItem>(items);
        }

        // Add
        public void Add(string value, float weight, int amount = 1, object? context = null)
        {
            if (string.IsNullOrEmpty(value))
                return;

            CodeContract.GreaterThanZero(weight, nameof(weight));
            items.Add(new(value, amount, weight, context));
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

        // IsEmpty
        public bool IsEmpty => items.Count == 0;

        // Items
        public ReadOnlyCollection<ChanceTableItem> Items { get; }

        // Nothing
        public const string Nothing = "<Nothing>";

        // SetSeed
        public void SetSeed(int seed) => random = new Random(seed);
    }
}
