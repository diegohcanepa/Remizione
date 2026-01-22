using Microsoft.Xna.Framework;
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
                if (IsFull)
                    return null;

                item = new Item(this, definition) { Count = amount };
                items.Add(item);
            }
            else
            {
                item.Count += amount;
            }

            Invalidate();

            return item;
        }

        // Capacity
        public int Capacity
        {
            get;
            set
            {
                if (value != field)
                {
                    field = int.Clamp(value, 3, MaximumCapacity);
                    Invalidate();
                }
            }
        } = 9;

        // Clear
        public void Clear()
        {
            items.Clear();
            Invalidate();
        }

        // Contains
        public bool Contains(Item item)
        {
            return items.Contains(item);
        }

        // ContentVersion
        public int ContentVersion { get; private set; }

        // Count
        public int Count => items.Count;

        // DropItem
        public void DropItem(Item item, Vector2 position)
        {
            if (Session.Room != null && Remove(item))
            {
                if (Session.ObjectPools.Sacks.Get() is Sack sack)
                {
                    sack.Position = position;
                    sack.Item = item;
                    Session.Room.Children.Add(sack);
                }
            }
        }

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

        // GetLuckFactor
        public float GetLuckFactor()
        {
            float result = 0;

            for (var i = 0; i < items.Count; i++)
            {
                result += items[i].Definition.LuckFactor;
            }

            return result;
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
        public int IndexOf(Item item)
        {
            return items.IndexOf(item);
        }

        // Indexer
        public Item this[int index] => items[index];

        // IsEmpty
        public bool IsEmpty => items.Count == 0;

        // IsFull
        public bool IsFull => items.Count >= Capacity;

        // Invalidate
        public void Invalidate()
        {
            unchecked { ContentVersion++; }
        }

        // MaximumCapacity
        public const int MaximumCapacity = 10;

        // Remove
        public bool Remove(string name)
        {
            return Find(name) is Item item && Remove(item);
        }

        // Remove
        public bool Remove(Item item)
        {
            if (items.Remove(item))
            {
                Invalidate();
                return true;
            }
            else
            {
                return false;
            }
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
                var itemData = itemList[i].Split(':');

                if (ItemDefinition.Find(itemData[0]) != null)
                {
                    if (Add(itemData[0], int.Parse(itemData[1], CultureInfo.InvariantCulture)) is Item addedItem)
                    {
                        addedItem.Durability = float.Parse(itemData[2], CultureInfo.InvariantCulture);
                    }
                }
            }
        }

        // Session
        public GameSession Session { get; }
    }
}
