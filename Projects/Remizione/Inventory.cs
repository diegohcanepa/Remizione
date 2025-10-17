using System;

namespace Remizione
{
    /// <summary>
    /// Inventory
    /// </summary>
    public sealed class Inventory
    {
        #region Constructor

        // Constructor
        public Inventory(Actor owner)
        {
            this.Consumables = new ItemContainer(owner, InventoryCategory.Consumables);
            this.Gadgets = new ItemContainer(owner, InventoryCategory.Gadgets);
            this.Junk = new ItemContainer(owner, InventoryCategory.Junk);
            this.KeyItems = new ItemContainer(owner, InventoryCategory.KeyItems);
            this.Trinkets = new ItemContainer(owner, InventoryCategory.Trinkets);
        }

        #endregion

        // Clear
        public void Clear()
        {
            foreach (var category in Enum.GetValues<InventoryCategory>())
            {
                if (category != InventoryCategory.None)
                    GetContainer(category).Clear();
            }
        }

        // Consumables
        public ItemContainer Consumables { get; }

        // Find
        public Item? Find(string item)
        {
            if (MetaItem.Find(item) is MetaItem metaItem)
                return Find(metaItem);
            else
                return null;
        }

        // Find
        public Item? Find(MetaItem metaItem)
        {
            return GetContainer(metaItem.Category).Find(metaItem.Name);
        }

        // GetContainer
        public ItemContainer GetContainer(InventoryCategory category)
        {
            return category switch
            {
                InventoryCategory.Consumables => Consumables,
                InventoryCategory.Junk => Junk,
                InventoryCategory.KeyItems => KeyItems,
                InventoryCategory.Gadgets => Gadgets,
                InventoryCategory.Trinkets => Trinkets,
                _ => throw new ArgumentException($"Invalid inventory category: {category}", nameof(category)),
            };
        }

        // Gadgets
        public ItemContainer Gadgets { get; }

        // Junk
        public ItemContainer Junk { get; }

        // KeyItems
        public ItemContainer KeyItems { get; }

        // Trinkets
        public ItemContainer Trinkets { get; }
    }
}
