using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// Inventory
    /// </summary>
    public sealed class Inventory
    {
        private readonly List<Item> items = [];
        private const string NoneValue = "[None]";

        // Constructor
        public Inventory(GameThing owner, InventoryCategory category)
        {
            this.Owner = owner;
            this.Category = category;
        }

        // Add
        public Item? Add(string name, int amount)
        {
            var metaItem = MetaItem.Find(name) ?? throw new InvalidOperationException("Meta item not found.");
            return Add(metaItem, amount);
        }

        // Add
        public Item? Add(MetaItem metaItem, int amount)
        {
            if (metaItem.Category != Category)
                throw new InvalidOperationException($"Meta item '{metaItem.Name}' does not belong to the category '{Category}'.");

            if (!metaItem.IsStackable)
                amount = 1;

            var item = GetItem(metaItem.Name);

            if (item != null && metaItem.IsStackable)
            {
                item.Count += amount;
            }
            else
            {
                item = new Item(this, metaItem) { Count = amount };
                items.Add(item);
            }

            if (SelectedItem == null)
                SelectedItem = item;

            return item;
        }

        // Category
        public InventoryCategory Category { get; }

        // Contains
        public bool Contains(Item item) => items.Contains(item);

        // Count
        public int Count => items.Count;

        // GetItem
        public Item? GetItem(string name)
        {
            for (var i = 0; i < items.Count; i++)
            {
                if (items[i].Name == name)
                    return items[i];
            }

            return null;
        }

        // GetItems
        public Item[] GetItems() => items.ToArray();

        // GetSerializationData
        public string GetSerializationData()
        {
            var result = new List<string>
            {
                SelectedItem is null ? NoneValue : SelectedItem.Name
            };

            foreach (var item in items)
            {
                result.Add($"{item.Name}:{item.Count}:{item.Durability}");
            }

            return string.Join(";", result);
        }

        // IndexOf
        public int IndexOf(Item item) => items.IndexOf(item);

        // Indexer
        public Item this[int index] => items[index];

        // IsEmpty
        public bool IsEmpty => items.Count == 0;

        // Owner
        public GameThing Owner { get; }

        // Remove
        public bool Remove(string name)
        {
            if (GetItem(name) is Item item)
                return Remove(item);
            else
                return false;
        }

        // Remove
        public bool Remove(Item item)
        {
            if (items.Remove(item))
            {
                if (SelectedItem == item)
                    SelectedItem = null;

                return true;
            }
            else
                return false;
        }

        // Select
        public bool Select(string name)
        {
            if (GetItem(name) is Item item)
                return Select(item);
            else
                return false;
        }

        // Select
        public bool Select(Item item)
        {
            if (items.Contains(item))
            {
                SelectedItem = item;
                return true;
            }
            else
                return false;
        }

        // SelectedItem
        public Item? SelectedItem { get; private set; }

        // SelectFirst
        public Item? SelectFirst()
        {
            if (items.Count > 0)
            {
                Select(items[0]);
                return SelectedItem;
            }

            return null;
        }

        // SelectLast
        public Item? SelectLast()
        {
            if (items.Count > 0)
            {
                Select(items[^1]);
                return SelectedItem;
            }

            return null;
        }

        // SelectNext
        public Item? SelectNext()
        {
            if (items.Count <= 1)
                return null;

            if (SelectedItem == null)
            {
                if (items.Count > 0)
                    Select(items[0]);
            }
            else if (items.Count > 1)
            {
                var index = items.IndexOf(SelectedItem);
                if (index == items.Count - 1)
                    Select(items[0]);
                else
                    Select(items[index + 1]);
            }

            return SelectedItem;
        }

        // SelectPrevious
        public Item? SelectPrevious()
        {
            if (items.Count <= 1)
                return null;

            if (SelectedItem == null)
            {
                if (items.Count > 0)
                    Select(items[^1]);
            }
            else if (items.Count > 1)
            {
                var index = items.IndexOf(SelectedItem);
                if (index == 0)
                    Select(items[^1]);
                else
                    Select(items[index - 1]);
            }

            return SelectedItem;
        }

        // SetSerializationData
        public void SetSerializationData(string data)
        {
            items.Clear();

            if (string.IsNullOrEmpty(data))
                return;

            var itemList = data.Split(';');

            for (var i = 0; i < itemList.Length; i++)
            {
                if (i == 0)
                {
                    SelectedItem = GetItem(itemList[i]);
                }
                else
                {
                    var itemData = itemList[i].Split(':');
                    Add(itemData[0], int.Parse(itemData[1]));
                }
            }
        }

        // Size
        public int Size { get; set; } = 6;
    }
}
