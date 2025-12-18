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
        public Item? Add(string name, int amount)
        {
            var metaItem = MetaItem.Find(name) ?? throw new InvalidOperationException("Meta item not found.");
            return Add(metaItem, amount);
        }

        // Add
        public Item? Add(MetaItem metaItem, int amount)
        {
            var item = metaItem.StackMode != StackMode.None ? Find(metaItem.Name) : null;

            if (item == null)
            {
                item = new Item(this, metaItem) { Count = amount };
                items.Add(item);

                var category = item.MetaItem.Category;

                if ((category == ItemCategory.RightHand && RightHand == null) ||
                    (category == ItemCategory.LeftHand && LeftHand == null))
                {
                    Equip(item);
                }
            }
            else
                item.Count += amount;

            return item;
        }

        // Clear
        public void Clear()
        {
            SelectedItem = null;
            LeftHand = null;
            RightHand = null;
            Gadget = null;
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

        // Equip
        public void Equip(Item item)
        {
            if (!item.MetaItem.IsEquipment)
                return;

            switch (item.MetaItem.Category)
            {
                // Left hand
                case ItemCategory.LeftHand:
                    LeftHand = item;
                    break;

                // Right hand
                case ItemCategory.RightHand:
                    RightHand = item;
                    break;

                // Gadget
                case ItemCategory.Gadget:
                    Gadget = item;
                    break;
            }
        }

        // EquipNext
        public Item? EquipNext(ItemCategory category)
        {
            if (items.Count == 0)
                return null;

            var currentItem = GetEquippedItem(category);
            var index = currentItem == null ? -1 : items.IndexOf(currentItem);

            for (var i = 1; i <= items.Count; i++)
            {
                var nextIndex = (index + i) % items.Count;
                var nextItem = items[nextIndex];

                if (nextItem.MetaItem.Category == category)
                {
                    Equip(nextItem);
                    return nextItem;
                }
            }

            return null;
        }

        // EquipPrevious
        public Item? EquipPrevious(ItemCategory category)
        {
            if (items.Count == 0)
                return null;

            var currentItem = GetEquippedItem(category);
            var index = currentItem == null ? items.Count : items.IndexOf(currentItem);

            for (var i = 1; i <= items.Count; i++)
            {
                var prevIndex = (index - i + items.Count) % items.Count;
                var prevItem = items[prevIndex];

                if (prevItem.MetaItem.Category == category)
                {
                    Equip(prevItem);
                    return prevItem;
                }
            }

            return null;
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

        // FindNotNull
        public Item FindNotNull(string name)
        {
            return Find(name) ?? throw new InvalidOperationException($"Item '{name}' not found.");
        }

        // Gadget
        public Item? Gadget { get; private set; }

        // GetEquippedItem
        public Item? GetEquippedItem(ItemCategory category)
        {
            return category switch
            {
                ItemCategory.RightHand => RightHand,
                ItemCategory.LeftHand => LeftHand,
                ItemCategory.Gadget => Gadget,
                _ => null,
            };
        }

        // GetItems
        public Item[] GetItems()
        {
            return items.ToArray();
        }

        // GetItems
        public Item[] GetItems(ItemCategory? category)
        {
            var result = new List<Item>();

            for (var i = 0; i < items.Count; i++)
            {
                if (category == null || items[i].MetaItem.Category == category)
                    result.Add(items[i]);
            }

            return result.ToArray();
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
                if (items[i].MetaItem.Category == category)
                    return true;
            }

            return false;
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
        public bool IsFull => items.Count >= MaximumSize;

        // LeftHand
        public Item? LeftHand { get; private set; }

        // MaximumSize
        public const int MaximumSize = 12;

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
                if (LeftHand == item)
                    LeftHand = null;

                if (RightHand == item)
                    RightHand = null;

                if (Gadget == item)
                    Gadget = null;

                if (SelectedItem == item)
                {
                    if (itemIndex > 0)
                        Select(items[itemIndex - 1]);
                    else if (itemIndex < items.Count - 1)
                        Select(items[itemIndex + 1]);
                    else
                        SelectedItem = null;
                }

                EquipNext(item.MetaItem.Category);

                return true;
            }
            else
                return false;
        }

        // RightHand
        public Item? RightHand { get; private set; }

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

                    if (MetaItem.Find(itemData[0]) != null)
                        Add(itemData[0], int.Parse(itemData[1], CultureInfo.InvariantCulture));
                }
            }
        }

        // Session
        public GameSession Session { get; }

        // Unequip
        public void Unequip(Item item)
        {
            if (!item.MetaItem.IsEquipment)
                return;

            switch (item.MetaItem.Category)
            {
                // Gadgets
                case ItemCategory.Gadget:
                    Gadget = null;
                    break;

                // Left hand
                case ItemCategory.LeftHand:
                    LeftHand = null;
                    break;

                // Right hand
                case ItemCategory.RightHand:
                    RightHand = null;
                    break;
            }
        }
    }
}
