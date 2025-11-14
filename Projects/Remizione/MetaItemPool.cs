using Engendro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// MetaItemPool
    /// </summary>
    public sealed class MetaItemPool
    {
        private readonly Dictionary<string, MetaItem> unlockedDictionary = [];
        private readonly List<MetaItem> unlockedList = [];

        // Constructor
        public MetaItemPool()
        {
            UnlockedItems = new(unlockedList);
        }

        // Deserialize
        public void Deserialize(string data)
        {
            InitializeDefaults();

            var names = data.Split(';', StringSplitOptions.RemoveEmptyEntries);

            foreach (var name in names)
            {
                Unlock(name);
            }
        }

        // Find
        public MetaItem? Find(string name)
        {
            if (unlockedDictionary.TryGetValue(name, out var result))
                return result;
            else
                return null;
        }

        // FindNotNull
        public MetaItem FindNotNull(string name)
        {
            return Find(name) ?? throw new InvalidOperationException($"MetaItem '{name}' not found.");
        }

        // GetItems
        public List<MetaItem> GetItems(ItemCategory category)
        {
            var result = new List<MetaItem>();

            foreach (var metaItem in unlockedList)
            {
                if (metaItem.Category == category)
                    result.Add(metaItem);
            }

            return result;
        }

        // GetItems
        public List<MetaItem> GetItems(ItemRealm realm)
        {
            var result = new List<MetaItem>();

            foreach (var metaItem in unlockedList)
            {
                if (metaItem.Realm == realm)
                    result.Add(metaItem);
            }

            return result;
        }

        // GetRandomItem
        public MetaItem? GetRandomItem()
        {
            return UnlockedItems.GetRandomItem();
        }

        // GetRandomItem
        public MetaItem? GetRandomItem(ItemCategory category)
        {
            return GetItems(category).GetRandomItem();
        }

        // GetRandomItem
        public MetaItem? GetRandomItem(ItemRealm realm)
        {
            return GetItems(realm).GetRandomItem();
        }

        // InitializeDefaults
        public void InitializeDefaults()
        {
            unlockedDictionary.Clear();
            unlockedList.Clear();

            foreach (var metaItem in MetaItem.AllItems)
            {
                if (metaItem.Unlocked)
                    Unlock(metaItem.Name);
            }
        }

        // IsUnlocked
        public bool IsUnlocked(string name)
        {
            return unlockedDictionary.ContainsKey(name);
        }

        // IsUnlocked
        public bool IsUnlocked(MetaItem metaItem)
        {
            return IsUnlocked(metaItem.Name);
        }

        // Unlock
        public void Unlock(string itemName)
        {
            if (MetaItem.Find(itemName) is MetaItem metaItem)
                Unlock(metaItem);
        }

        // Unlock
        public void Unlock(MetaItem metaItem)
        {
            unlockedDictionary.Add(metaItem.Name, metaItem);
            unlockedList.Add(metaItem);
        }

        // UnlockedItems
        public ReadOnlyCollection<MetaItem> UnlockedItems { get; }

        // Serialize
        public string Serialize()
        {
            var result = new List<string>();

            foreach (var item in unlockedList)
            {
                result.Add(item.Name);
            }

            return string.Join(";", result);
        }
    }
}
