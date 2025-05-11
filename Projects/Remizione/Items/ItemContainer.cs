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
        private readonly Dictionary<string, Item> itemsDictionary = [];

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
            if (string.IsNullOrEmpty(name))
                return null;

            if (itemsDictionary.TryGetValue(name, out var value))
            {
                value.Count += amount;
                return value;
            }
            else
            {
                var metaItem = MetaItem.Find(name) ?? throw new InvalidOperationException("Item type not found.");
                var item = new Item(this, metaItem) { Count = amount };
                itemsDictionary[name] = item;
                items.Add(item);

                if (SelectedItem == null)
                    SelectedItem = item;

                return item;
            }
        }

        // Category
        public ItemContainerCategory Category { get; }

        // GetItem
        public Item? GetItem(string name) => itemsDictionary.TryGetValue(name, out var value) ? value : null;

        // Items
        public ReadOnlyCollection<Item> Items { get; }

        // Owner
        public GameThing Owner { get; }

        // Remove
        public void Remove(string name)
        {
            if (GetItem(name) is Item item)
            {
                items.Remove(item);
                itemsDictionary.Remove(item.Name);
            }
        }

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
