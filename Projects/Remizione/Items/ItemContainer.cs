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

        // Activate
        public bool Activate(string name)
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

        // ActivateNext
        public void ActivateNext()
        {
            if (items.Count == 0)
                return;

            if (SelectedItem == null)
            {
                if (items.Count > 0)
                    Activate(items[0].Name);
            }
            else if (items.Count > 1)
            {
                var index = items.IndexOf(SelectedItem);
                if (index == items.Count - 1)
                    Activate(items[0].Name);
                else
                    Activate(items[index + 1].Name);
            }
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

        // SelectedIndex
        public int SelectedIndex { get; private set; }

        // SelectedItem
        public Item? SelectedItem { get; private set; }
    }
}
