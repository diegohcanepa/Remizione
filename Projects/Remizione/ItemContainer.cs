using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Remizione
{
    /// <summary>
    /// ItemContainer
    /// </summary>
    public sealed class ItemContainer : VersionedCollection<Item>
    {
        #region Constructor

        // Constructor
        public ItemContainer(GameSession session)
            : base()
        {
            this.Session = session;
        }

        #endregion

        #region Serialization methods

        // Deserialize
        public string Deserialize(string content)
        {
            Clear();

            var values = content.Split(";");

            foreach (var value in values)
            {
                var data = value.Split("|");
                if (Add(data[0]) is Item item)
                    item.Amount = XmlConvert.ToInt32(data[1]);
            }

            return string.Join(";", values);
        }

        // Serialize
        public string Serialize()
        {
            var values = new List<string>();

            foreach (var item in this)
            {
                values.Add($"{item.Name}|{XmlConvert.ToString(item.Amount)}");
            }

            return string.Join(";", values);
        }

        #endregion

        // Add
        public Item Add(string name)
        {
            return Add(GameData.Items.Get(name));
        }

        // Add
        public Item Add(ItemDefinition definition)
        {
            if (!CanAddItem(definition))
                throw new InvalidOperationException("Item container is full.");

            if (Find(definition.Name) is Item item)
            {
                item.Amount += 1;
            }
            else
            {
                item = new Item(this, definition);
                Add(item);
            }

            return item;
        }

        // AmbientLightColor
        public Color? AmbientLightColor { get; private set; }

        // CanAddItem
        public bool CanAddItem(ItemDefinition definition)
        {
            return Find(definition.Name) is Item item ? !item.IsFull : !IsFull;
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
                    Version++;
                }
            }
        } = GameSettings.DefaultInventoryCapacity;

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

        // IsEmpty
        public bool IsEmpty => Count == 0;

        // IsFull
        public bool IsFull => Count == Capacity;

        // MaximumCapacity
        public const int MaximumCapacity = 30;

        // MinimumCapacity
        public const int MinimumCapacity = 3;

        // Remove
        public bool Remove(string name)
        {
            return Find(name) is Item item && Remove(item);
        }

        // Session
        public GameSession Session { get; }
    }
}
