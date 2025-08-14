using Engendro;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// LootTable
    /// </summary>
    public sealed class LootTable
    {
        private readonly List<(string itemName, float weight)> items = [];
        private static readonly Dictionary<string, LootTable> lootTables = [];
        private readonly Random _random = new();

        #region Static members

        // Find
        public static LootTable? Find(string name) => lootTables.TryGetValue(name, out var result) ? result : null;

        // Register
        public static LootTable Register(string name)
        {
            if (lootTables.ContainsKey(name))
                throw new InvalidOperationException($"Loot table '{name}' already exists.");

            var result = new LootTable();
            lootTables[name] = result;

            return result;
        }

        #endregion

        // Add
        public void Add(string itemName, float weight)
        {
            if (string.IsNullOrEmpty(itemName))
                return;

            CodeContract.GreaterThanZero(weight, nameof(weight));
            items.Add((itemName, weight));
        }

        // GetLoot
        public MetaItem? GetLoot()
        {
            if (items.Count == 0)
                return null;

            float totalWeight = 0f;

            for (var i = 0; i < items.Count; i++)
            {
                totalWeight += items[i].weight;
            }

            float roll = (float)_random.NextDouble() * totalWeight;

            for (var i = 0; i < items.Count; i++)
            {
                if (roll < items[i].weight)
                    return MetaItem.Find(items[i].itemName);

                roll -= items[i].weight;
            }

            return null;
        }

        // Nothing
        public const string Nothing = "<Nothing>";
    }
}
