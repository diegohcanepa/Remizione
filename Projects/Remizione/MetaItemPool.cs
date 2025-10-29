using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// MetaItemPool
    /// </summary>
    public sealed class MetaItemPool
    {
        private readonly HashSet<MetaItem> unlockedList = [];

        // Deserialize
        public void Deserialize(string data)
        {
            InitializeDefaults();

            var names = data.Split(';', StringSplitOptions.RemoveEmptyEntries);

            foreach (var name in names)
            {
                if (MetaItem.Find(name) is MetaItem metaItem)
                    unlockedList.Add(metaItem);
            }
        }

        // InitializeDefaults
        public void InitializeDefaults()
        {
            unlockedList.Clear();

            foreach (var metaItem in MetaItem.AllItems)
            {
                if (metaItem.Unlocked)
                    unlockedList.Add(metaItem);
            }
        }

        // IsUnlocked
        public bool IsUnlocked(string name)
        {
            if (MetaItem.Find(name) is not MetaItem metaItem)
                return false;

            return unlockedList.Contains(metaItem);
        }

        // IsUnlocked
        public bool IsUnlocked(MetaItem metaItem)
        {
            return unlockedList.Contains(metaItem);
        }

        // Unlock
        public void Unlock(string itemName)
        {
            if (MetaItem.Find(itemName) is MetaItem metaItem)
                unlockedList.Add(metaItem);
        }

        // UnlockedItems
        public IEnumerable<MetaItem> UnlockedItems => unlockedList;

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
