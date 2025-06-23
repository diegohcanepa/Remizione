using Microsoft.Xna.Framework;
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
        private string? toString;
        private readonly List<Item> items = [];

        // Constructor
        public ItemContainer(GameThing owner, string displayName)
        {
            this.Owner = owner;
            this.DisplayName = displayName;

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

        // GetSerializationData
        public string GetSerializationData()
        {
            var result = new List<string>();

            foreach (var item in Items)
            {
                result.Add($"{item.Name}:{item.Count}:{item.Durability}");
            }

            return string.Join(";", result);
        }

        // IsEmpty
        public bool IsEmpty => items.Count == 0;

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
        public bool SelectNext()
        {
            if (items.Count <= 1)
                return false;

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

            return true;
        }

        // SelectPrevious
        public bool SelectPrevious()
        {
            if (items.Count <= 1)
                return false;

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

            return true;
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
            if (toString == null)
                toString = DisplayName;
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
