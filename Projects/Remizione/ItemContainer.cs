using Engendro;
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

        #region Serialization

        // Deserialize
        public string Deserialize(string content)
        {
            Clear();

            if (string.IsNullOrWhiteSpace(content))
                return string.Empty;

            var values = content.Split(";");

            foreach (var value in values)
            {
                var data = value.Split("|");

                if (GameData.Items.Find(data[0]) is not ItemDefinition definition)
                    continue;

                var amount = XmlConvert.ToInt32(data[1]);

                var item = new Item(this, definition)
                {
                    Amount = amount,
                    PerishState = Enum.TryParse<ItemPerishState>(data[2], out ItemPerishState perishState) ? perishState : ItemPerishState.Fresh,
                    Uses = XmlConvert.ToInt32(data[3]),
                    BreakChance = XmlConvert.ToSingle(data[4])
                };

                Add(item);
            }

            return string.Join(";", values);
        }

        // Serialize
        public string Serialize()
        {
            var values = new List<string>();

            // Name | Amount | PerishState | Uses | BreakChance
            foreach (var item in this)
            {
                values.Add($"{item.Name}|{XmlConvert.ToString(item.Amount)}|{(int)item.PerishState}|{item.Uses}|{XmlConvert.ToString(item.BreakChance)}");
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
        public Item Add(ItemDefinition definition, int amount = 1)
        {
            return TryAdd(definition, amount) ?? throw new InvalidOperationException($"Cannot add item '{definition.Name}' to container.");
        }

        // CanAddItem
        public bool CanAddItem(ItemDefinition definition)
        {
            if (definition.IsStackable)
            {
                if (Find(definition.Name) is Item existingItem)
                    return existingItem.Amount < ItemDefinition.MaxStack;
                else
                    return !IsFull;
            }

            return !IsFull;
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
        public const int MaximumCapacity = 12;

        // MinimumCapacity
        public const int MinimumCapacity = 3;

        // Session
        public GameSession Session { get; }

        // TryAdd
        public Item? TryAdd(ItemDefinition definition, int amount = 1)
        {
            if (amount < 1)
                amount = 1;

            if (!CanAddItem(definition))
                return null;

            if (definition.IsStackable)
            {
                if (Find(definition.Name) is Item existingItem)
                {
                    var spaceLeft = ItemDefinition.MaxStack - existingItem.Amount;
                    var toAdd = Math.Min(amount, spaceLeft);

                    existingItem.Amount += toAdd;
                    return existingItem;
                }

                var initialAmount = Math.Min(amount, ItemDefinition.MaxStack);
                var newItem = new Item(this, definition) { Amount = initialAmount };

                Add(newItem);
                return newItem;
            }

            var spaceAvailable = Capacity - Count;
            var slotsToAdd = Math.Min(amount, spaceAvailable);
            Item? lastCreated = null;

            for (var i = 0; i < slotsToAdd; i++)
            {
                var newItem = new Item(this, definition) { Amount = 1 };
                Add(newItem);
                lastCreated = newItem;
            }

            return lastCreated;
        }
    }
}