using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace ScaryCastle
{
    /// <summary>
    /// Inventory
    /// </summary>
    public sealed class Inventory : Collection<Item>
    {
        #region Constructor

        // Constructor
        public Inventory(GameSession session)
            : base()
        {
            this.Session = session;
        }

        #endregion

        #region Protected members

        // ClearItems
        protected override void ClearItems()
        {
            base.ClearItems();
            Invalidate();
        }

        // InsertItem
        protected override void InsertItem(int index, Item item)
        {
            base.InsertItem(index, item);
            Invalidate();
        }

        // RemoveItem
        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            Invalidate();
        }

        #endregion

        // Add
        public Item? Add(string name, int amount = 1)
        {
            var definition = ItemDefinition.Definitions.Find(name) ?? throw new InvalidOperationException("Item definition not found.");
            return Add(definition, amount);
        }

        // Add
        public Item? Add(ItemDefinition definition, int amount = 1)
        {
            if (!HasSpace(definition))
                return null;

            var item = Find(definition.Name);

            if (item == null)
            {
                if (IsFull)
                    return null;

                item = new Item(this, definition) { Count = amount };
                Add(item);
            }
            else
            {
                item.Count += amount;
            }

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
                    field = int.Clamp(value, MinimumCapacity, MaximumCapacity);
                    Invalidate();
                }
            }
        } = 6;

        // ContentVersion
        public int ContentVersion { get; private set; }

        // DropItem
        // TODO: Should be in other class (What about sacrifice in exchange for life?)
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
            for (var i = 0; i < Count; i++)
            {
                if (this[i].Name == name)
                    return this[i];
            }

            return null;
        }

        // GetItem
        public Item GetItem(string name)
        {
            return Find(name) ?? throw new InvalidOperationException($"Item '{name}' not found.");
        }

        // GetItems
        public Item[] GetItems(ItemCategory? category)
        {
            var result = new List<Item>();

            for (var i = 0; i < Count; i++)
            {
                if (category == null || this[i].Definition.Category == category)
                    result.Add(this[i]);
            }

            return [.. result];
        }

        // GetLuckFactor
        public float GetLuckFactor()
        {
            float result = 0;

            for (var i = 0; i < Count; i++)
            {
                result += this[i].Definition.LuckFactor;
            }

            return result;
        }

        // HasSpace
        public bool HasSpace(ItemDefinition definition)
        {
            var item = Find(definition.Name);

            return item != null || !IsFull;
        }

        // IsEmpty
        public bool IsEmpty => Count == 0;

        // IsFull
        public bool IsFull => Count == Capacity;

        // Invalidate
        public void Invalidate()
        {
            unchecked { ContentVersion++; }
        }

        // LoadState
        public void LoadState(string data)
        {
            Clear();

            if (string.IsNullOrEmpty(data))
                return;

            var itemList = data.Split(';');

            for (var i = 0; i < itemList.Length; i++)
            {
                var itemData = itemList[i].Split(':');

                if (ItemDefinition.Definitions.Find(itemData[0]) != null)
                {
                    if (Add(itemData[0], int.Parse(itemData[1], CultureInfo.InvariantCulture)) is Item addedItem)
                    {
                        addedItem.Durability = float.Parse(itemData[2], CultureInfo.InvariantCulture);
                    }
                }
            }
        }

        // MaximumCapacity
        public const int MaximumCapacity = 8;

        // MinimumCapacity
        public const int MinimumCapacity = 3;

        // Remove
        public bool Remove(string name)
        {
            return Find(name) is Item item && Remove(item);
        }

        // SaveState
        public string SaveState()
        {
            var result = new List<string>();

            foreach (var item in this)
            {
                result.Add($"{item.Name}:{item.Count}:{item.Durability}");
            }

            return string.Join(";", result);
        }

        // Session
        public GameSession Session { get; }
    }
}
