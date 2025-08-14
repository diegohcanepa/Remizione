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
            this.Junk = new ItemContainer(owner, InventoryCategory.Junk);
            this.KeyItems = new ItemContainer(owner, InventoryCategory.KeyItems);
            this.Traits = new ItemContainer(owner, InventoryCategory.Traits)
            {
                Size = 24
            };
     
            this.Trinkets = new ItemContainer(owner, InventoryCategory.Trinkets);
        }

        #endregion

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
                InventoryCategory.Traits => Traits,
                InventoryCategory.Trinkets => Trinkets,
                _ => throw new ArgumentException($"Invalid inventory category: {category}", nameof(category)),
            };
        }

        // Junk
        public ItemContainer Junk { get; }

        // KeyItems
        public ItemContainer KeyItems { get; }

        // NotifyRoomChanged
        public void NotifyRoomChanged()
        {
            foreach (var category in Enum.GetValues<InventoryCategory>())
            {
                if (category != InventoryCategory.None)
                {
                    var items = GetContainer(category).GetItems();

                    for (var i = 0; i < items.Length; i++)
                    {
                        if (items[i].MetaItem.ReplenishPerRoom)
                            items[i].Replenish();
                    }
                }
            }
        }

        // Traits
        public ItemContainer Traits { get; }

        // Trinkets
        public ItemContainer Trinkets { get; }
    }
}
