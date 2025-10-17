using System;
using System.Collections.Generic;
using System.Globalization;

namespace Remizione
{
    /// <summary>
    /// ItemContainer
    /// </summary>
    public sealed class ItemContainer
    {
        private readonly List<Item> items = [];
        private const string NoneValue = "[None]";

        // Constructor
        public ItemContainer(Actor owner, InventoryCategory category)
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

            var item = metaItem.AllowEmpty ? Find(metaItem.Name) : null;

            if (item == null)
            {
                item = new Item(this, metaItem) { Count = amount };
                items.Add(item);
            }
            else
               item.Count += amount;

            return item;
        }

        // Category
        public InventoryCategory Category { get; }

        // Clear
        public void Clear()
        {
            SelectedItem = null;
            items.Clear();
        }

        // ClearSelection
        public void ClearSelection() => SelectedItem = null;

        // Contains
        public bool Contains(Item item) => items.Contains(item);

        // Count
        public int Count => items.Count;

        // Find
        public Item? Find(string name)
        {
            for (var i = 0; i < items.Count; i++)
            {
                if (items[i].Name == name)
                    return items[i];
            }

            return null;
        }

        // FindNotNull
        public Item FindNotNull(string name) => Find(name) ?? throw new InvalidOperationException($"Item '{name}' not found.");

        // GetItems
        public Item[] GetItems() => items.ToArray();

        // GetSerializationData
        public string GetSerializationData()
        {
            var result = new List<string>
            {
                Size.ToString(CultureInfo.InvariantCulture),
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

        // IsFull
        public bool IsFull => items.Count >= Size;

        // IsEquipment
        public bool IsEquipment => Category == InventoryCategory.Junk || Category == InventoryCategory.Gadgets || Category == InventoryCategory.Trinkets;

        // Owner
        public Actor Owner { get; }

        // Remove
        public bool Remove(string name)
        {
            if (Find(name) is Item item)
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
                {
                    if (item.Index > 0)
                        Select(items[item.Index - 1]);
                    else if (item.Index < items.Count - 1)
                        Select(items[item.Index + 1]);
                    else
                        SelectedItem = null;
                }

                return true;
            }
            else
                return false;
        }

        // Select
        public bool Select(string name)
        {
            if (Find(name) is Item item)
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
            if (items.Count == 0)
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
            if (items.Count == 0)
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
                    Size = int.Parse(itemList[i], CultureInfo.InvariantCulture);
                }
                else if (i == 1)
                {
                    SelectedItem = Find(itemList[i]);
                }
                else
                {
                    var itemData = itemList[i].Split(':');

                    if (MetaItem.Find(itemData[0]) != null)
                        Add(itemData[0], int.Parse(itemData[1]));
                }
            }
        }

        // Size
        public int Size { get; set; } = 4;
    }
}
