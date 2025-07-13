using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// Inventory
    /// </summary>
    public sealed class Inventory
    {
        private string? toString;
        private readonly List<Item> items = [];

        // Constructor
        public Inventory(GameThing owner, string displayName)
        {
            this.Owner = owner;
            this.DisplayName = displayName;
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

            toString = null;

            return item;
        }

        // Contains
        public bool Contains(Item item) => items.Contains(item);

        // Count
        public int Count => items.Count;

        // DisplayName
        public string DisplayName { get; }

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
        public Item[] GetItems(MetaItemCategory category)
        {
            var result = new List<Item>();

            for (var i = 0; i < items.Count; i++)
            {
                if (items[i].MetaItem.Category == category)
                    result.Add(items[i]);
            }

            return result.ToArray();
        }

        // GetSerializationData
        public string GetSerializationData()
        {
            var result = new List<string>();

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
                toString = null;
                return true;
            }
            else
                return false;
        }

        // RemoveSelected
        public bool RemoveSelected()
        {
            var itemToRemove = SelectedItem;

            if (itemToRemove == null)
                return false;

            SelectNext();
            Remove(itemToRemove);
            return true;
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

        // SelectNext
        public Item? SelectNext(MetaItemCategory category)
        {
            for (var i = 0; i < items.Count; i++)
            {
                SelectNext();
                if (SelectedItem?.MetaItem is MetaItem metaItem && metaItem.Category == category)
                    return SelectedItem;
            }

            return null;
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

        // SelectPrevious
        public Item? SelectPrevious(MetaItemCategory category)
        {
            for (var i = items.Count - 1; i >= 0; i--)
            {
                SelectPrevious();
                if (SelectedItem?.MetaItem is MetaItem metaItem && metaItem.Category == category)
                    return SelectedItem;
            }

            return null;
        }

        // SetSerializationData
        public void SetSerializationData(string data)
        {
            items.Clear();

            if (string.IsNullOrEmpty(data))
                return;

            var itemList = data.Split(';');

            foreach (var item in itemList)
            {
                var itemData = item.Split(':');
                Add(itemData[0], int.Parse(itemData[1]));
            }
        }

        // ToString
        public override string ToString()
        {
            toString ??= DisplayName;
            return toString;
        }

        // Update
        public void Update(GameTime gameTime)
        {
            for (var i = 0; i < items.Count; i++)
            {
                items[i].Update(gameTime);
            }
        }
    }
}
