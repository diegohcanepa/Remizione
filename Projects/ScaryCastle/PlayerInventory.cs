using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ScaryCastle
{
    /// <summary>
    /// PlayerInventory
    /// </summary>
    public sealed class PlayerInventory : Collection<Item>
    {
        #region Constructor

        // Constructor
        public PlayerInventory(GameSession session)
            : base()
        {
            this.Session = session;
        }

        #endregion

        #region Private members

        // InvalidateAmbientLightColor
        private void InvalidateAmbientLightColor()
        {
            AmbientLightColor = null;

            Item? selectedItem = null;
            for (var i = 0; i < Items.Count; i++)
            {
                if (Items[i].Definition.LightModifier > 0)
                {
                    if (selectedItem == null || selectedItem.Definition.LightModifier < Items[i].Definition.LightModifier)
                        selectedItem = Items[i];
                }
            }

            if (selectedItem != null && selectedItem.Definition.LightColor != null)
                AmbientLightColor = selectedItem.Definition.LightColor;
        }

        #endregion

        #region Protected members

        // ClearItems
        protected override void ClearItems()
        {
            base.ClearItems();
            InvalidateContentVersion();
        }

        // InsertItem
        protected override void InsertItem(int index, Item item)
        {
            base.InsertItem(index, item);
            InvalidateContentVersion();
        }

        // RemoveItem
        protected override void RemoveItem(int index)
        {
            Session.PlayerStats.RemoveAllModifiers(this[index]);
            base.RemoveItem(index);
            InvalidateAmbientLightColor();
            InvalidateContentVersion();
        }

        #endregion

        // Add
        public Item? Add(string name)
        {
            var definition = ItemDefinition.Definitions.Find(name) ?? throw new InvalidOperationException("Item definition not found.");
            return Add(definition);
        }

        // Add
        public Item? Add(ItemDefinition definition)
        {
            if (definition.Behavior is not ItemBehavior.Common and not ItemBehavior.Skill)
                return null;

            if (!HasSpace(definition))
                return null;

            var item = Find(definition.Name);

            if (item == null)
            {
                if (IsFull)
                    return null;

                item = new Item(this, definition);
                Add(item);

                if (Session.CurrentRun != null)
                {
                    if (item.Definition.LuckModifier != 0)
                    {
                        Session.PlayerStats.Luck.AddModifier(new(item.Definition.LuckModifier, item));
                    }
                    else if (item.Definition.LightModifier != 0)
                    {
                        Session.PlayerStats.AmbientLight.AddModifier(new(item.Definition.LightModifier, item));
                        InvalidateAmbientLightColor();
                    }
                }
            }
            else
            {
                item.Amount += 1;
            }

            return item;
        }

        // AmbientLightColor
        public Color? AmbientLightColor { get; private set; }

        // Capacity
        public int Capacity
        {
            get;
            set
            {
                if (value != field)
                {
                    field = int.Clamp(value, MinimumCapacity, MaximumCapacity);
                    InvalidateContentVersion();
                }
            }
        } = 6;

        // ContentVersion
        public int ContentVersion { get; private set; }

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

        // HasSpace
        public bool HasSpace(ItemDefinition definition)
        {
            var item = Find(definition.Name);

            return item != null || !IsFull;
        }

        // InvalidateContentVersion
        public void InvalidateContentVersion()
        {
            unchecked { ContentVersion++; }
        }

        // IsEmpty
        public bool IsEmpty => Count == 0;

        // IsFull
        public bool IsFull => Count == Capacity;

        // MaximumCapacity
        public const int MaximumCapacity = 8;

        // MinimumCapacity
        public const int MinimumCapacity = 3;

        // Remove
        public bool Remove(string name)
        {
            if (Find(name) is Item item)
                return Remove(item);

            return false;
        }

        // Session
        public GameSession Session { get; }
    }
}
