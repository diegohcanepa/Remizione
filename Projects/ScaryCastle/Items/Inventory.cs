
using Engendro;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ScaryCastle
{
    /// <summary>
    /// Inventory
    /// </summary>
    public sealed class Inventory
    {
        private readonly List<Item> items = [];
        private const string NoneValue = "[None]";

        // Constructor
        public Inventory(GameSession session)
        {
            this.Session = session;
        }

        // Add
        public Item? Add(string name, int amount = 1)
        {
            var definition = ItemDefinition.Find(name) ?? throw new InvalidOperationException("Item definition not found.");
            return Add(definition, amount);
        }

        // Add
        public Item? Add(ItemDefinition definition, int amount = 1)
        {
            var item = Find(definition.Name);

            if (item == null)
            {
                item = new Item(this, definition) { Count = amount };
                items.Add(item);
            }
            else
            {
                item.Count += amount;
            }

            return item;
        }

        // Capacity
        public int Capacity { get; set; } = 6;

        // Clear
        public void Clear()
        {
            SelectedItem = null;
            items.Clear();
        }

        // ClearSelection
        public void ClearSelection()
        {
            SelectedItem = null;
        }

        // Contains
        public bool Contains(Item item)
        {
            return items.Contains(item);
        }

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

        // GetItem
        public Item GetItem(string name)
        {
            return Find(name) ?? throw new InvalidOperationException($"Item '{name}' not found.");
        }

        // GetItems
        public Item[] GetItems()
        {
            return [.. items];
        }

        // GetItems
        public Item[] GetItems(ItemCategory? category)
        {
            var result = new List<Item>();

            for (var i = 0; i < items.Count; i++)
            {
                if (category == null || items[i].Definition.Category == category)
                    result.Add(items[i]);
            }

            return [.. result];
        }

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

        // HasItems
        public bool HasItems(ItemCategory category)
        {
            for (var i = 0; i < items.Count; i++)
            {
                if (items[i].Definition.Category == category)
                    return true;
            }

            return false;
        }

        // HeldItem
        public Item? HeldItem
        {
            get;
            set
            {
                if (value == null)
                {
                    MouseCursor.Reset();
                }
                else if (value.Definition.Image is AtlasImage image)
                {
                    MouseCursor.SetCustomImage(image, field);
                }

                field = value;
            }
        }

        // IndexOf
        public int IndexOf(Item item)
        {
            return items.IndexOf(item);
        }

        // Indexer
        public Item this[int index] => items[index];

        // IsEmpty
        public bool IsEmpty => items.Count == 0;

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
            var itemIndex = items.IndexOf(item);
            if (items.Remove(item))
            {
                if (SelectedItem == item)
                {
                    if (itemIndex > 0)
                        Select(items[itemIndex - 1]);
                    else if (itemIndex < items.Count - 1)
                        Select(items[itemIndex + 1]);
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

        // SelectedItem
        public Item? SelectedItem { get; private set; }

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
                    SelectedItem = Find(itemList[i]);
                }
                else
                {
                    var itemData = itemList[i].Split(':');

                    if (ItemDefinition.Find(itemData[0]) != null)
                        Add(itemData[0], int.Parse(itemData[1], CultureInfo.InvariantCulture));
                }
            }
        }

        // Session
        public GameSession Session { get; }
    }
}
