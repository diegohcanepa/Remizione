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
        public ItemContainer(GameThing owner, ItemContainerCategory category, string displayName)
        {
            this.Owner = owner;
            this.Category = category;
            this.DisplayName = displayName;

            Items = new ReadOnlyCollection<Item>(items);
        }

        #region Private members

        // Invalidate
        private void Invalidate()
        {
            RequiresUpdate = false;

            for (var i = 0; i < items.Count; i++)
            {
                if (items[i].MetaItem.UseInterval > 0)
                {
                    RequiresUpdate = true;
                    return;
                }
            }
        }

        #endregion

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
            else if (!IsFull)
            {
                item = new Item(this, metaItem) { Count = amount };
                items.Add(item);
            }

            if (SelectedItem == null)
                SelectedItem = item;

            Invalidate();

            toString = null;

            return item;
        }

        // Category
        public ItemContainerCategory Category { get; }

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

        // IsFull
        public bool IsFull => Size > 0 && items.Count >= Size;

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
                toString = null;
                return true;
            }
            else
                return false;
        }

        // RequiresUpdate
        public bool RequiresUpdate { get; private set; }

        // Select
        public bool Select(Item item)
        {
            if (items.Contains(item))
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
        }

        // SelectPrevious
        public void SelectPrevious()
        {
            if (items.Count == 0)
                return;

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

        // Size
        public int Size => Owner.GetItemContainerSize(Category);

        // ToString
        public override string ToString()
        {
            if (toString == null)
            {
                toString = DisplayName;
                if (Size > 0)
                    toString += $" ({items.Count} / {Size})";
            }

            return toString;
        }

        // Update
        public void Update(GameTime gameTime)
        {
            if (RequiresUpdate)
            {
                for (var i = 0; i < items.Count; i++)
                {
                    items[i].Update(gameTime);
                }
            }
        }
    }
}
