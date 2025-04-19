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
        private readonly List<(ItemName itemName, float weight)> items = [];
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
        public void Add(ItemName itemName, float dropChance)
        {
            if (itemName == ItemName.None)
                return;

            CodeContract.GreaterThanZero(dropChance, nameof(dropChance));
            items.Add((itemName, dropChance));
        }

        // Get
        public ItemName GetLoot()
        {
            if (items.Count == 0)
                return ItemName.None;

            float totalWeight = 0f;
            foreach (var loot in items)
            {
                totalWeight += loot.weight;
            }

            float roll = (float)_random.NextDouble() * totalWeight;

            foreach (var loot in items)
            {
                if (roll < loot.weight)
                    return loot.itemName;

                roll -= loot.weight;
            }

            return ItemName.None;
        }
    }
}
