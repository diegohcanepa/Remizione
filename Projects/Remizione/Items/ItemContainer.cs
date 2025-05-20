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
        private int capacity;
        private readonly List<Item> items = [];

        // Constructor
        public ItemContainer(GameThing owner)
        {
            this.Owner = owner;

            Items = new ReadOnlyCollection<Item>(items);
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

            var existingItem = GetItem(metaItem.Name);

            if (existingItem != null && metaItem.IsStackable)
            {
                existingItem.Count += amount;
                return existingItem;
            }
            else if (!IsFull)
            {
                var item = new Item(this, metaItem) { Count = amount };
                items.Add(item);
                return item;
            }

            return null;
        }

        // Capacity
        public int Capacity
        {
            get => capacity;
            set
            {
                if (value != capacity)
                {
                    if (value < 0)
                        value = 0;

                    this.capacity = value;

                    if (value > 0 && items.Count > capacity)
                        items.RemoveRange(items.Count - 1, items.Count - capacity);
                }
            }
        }

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

        // IsFull
        public bool IsFull => capacity > 0 && items.Count >= capacity;

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
    }
}
