using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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

        #region Protected members

        // RemoveItem
        protected override void RemoveItem(int index)
        {
            //Session.PlayerStats.RemoveAllModifiers(this[index]);
            base.RemoveItem(index);
        }

        #endregion

        // Add
        public Item? Add(string name)
        {
            var definition = GameData.Items.Find(name) ?? throw new InvalidOperationException("Item definition not found.");
            return Add(definition);
        }

        // Add
        public Item? Add(ItemDefinition definition)
        {
            if (definition.Behavior is not ItemBehavior.Sack and not ItemBehavior.PlayerAction)
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

                /*
                if (Session.RunInProgress)
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
                */
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
