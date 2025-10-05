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
            this.Thingies = new ItemContainer(owner, InventoryCategory.Thingies);
            this.Junk = new ItemContainer(owner, InventoryCategory.Junk);
            this.KeyItems = new ItemContainer(owner, InventoryCategory.KeyItems);
            this.Quirks = new ItemContainer(owner, InventoryCategory.Quirks)
            {
                Size = 12
            };

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
                InventoryCategory.Quirks => Quirks,
                InventoryCategory.Thingies => Thingies,
                InventoryCategory.Trinkets => Trinkets,
                _ => throw new ArgumentException($"Invalid inventory category: {category}", nameof(category)),
            };
        }

        // Junk
        public ItemContainer Junk { get; }

        // KeyItems
        public ItemContainer KeyItems { get; }

        // Quirks
        public ItemContainer Quirks { get; }

        // Thingies
        public ItemContainer Thingies { get; }

        // Trinkets
        public ItemContainer Trinkets { get; }
    }
}
