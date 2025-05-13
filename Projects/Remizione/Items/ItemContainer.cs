using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// ItemContainer
    /// </summary>
    public sealed class ItemContainer
    {
        private readonly List<Item> items = [];

        // Constructor
        public ItemContainer(GameThing owner, ItemContainerCategory category)
        {
            this.Owner = owner;
            this.Category = category;

            Items = new ReadOnlyCollection<Item>(items);
        }

        // Add
        public Item? Add(string name, int amount)
        {
            var metaItem = MetaItem.Find(name) ?? throw new InvalidOperationException("Meta item not found.");
            return Add(metaItem, amount);
        }

        // Add
        public Item Add(MetaItem metaItem, int amount)
        {
            if (metaItem.Maximum == 1)
                amount = 1;

            var existingItem = GetItem(metaItem.Name);

            if (existingItem != null && metaItem.Maximum > 1)
            {
                existingItem.Count += amount;
                return existingItem;
            }
            else
            {
                var item = new Item(this, metaItem) { Count = amount };
                items.Add(item);
                SelectedItem ??= item;
                return item;
            }
        }

        // Category
        public ItemContainerCategory Category { get; }

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

        // Items
        public ReadOnlyCollection<Item> Items { get; }

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
        public bool Remove(Item item) => items.Remove(item);

        // Select
        public bool Select(Item item) => Select(item.Name);

        // Select
        public bool Select(string name)
        {
            if (GetItem(name) is Item item)
            {
                SelectedItem = item;
                SelectedIndex = items.IndexOf(item);
                return true;
            }
            else
                return false;
        }

        // SelectedIndex
        public int SelectedIndex { get; private set; }

        // SelectedItem
        public Item? SelectedItem { get; private set; }

        // SelectNext
        public void SelectNext()
        {
            if (items.Count == 0)
                return;

            if (SelectedItem == null)
            {
                if (items.Count > 0)
                    Select(items[0].Name);
            }
            else if (items.Count > 1)
            {
                var index = items.IndexOf(SelectedItem);
                if (index == items.Count - 1)
                    Select(items[0].Name);
                else
                    Select(items[index + 1].Name);
            }
        }
    }
}
