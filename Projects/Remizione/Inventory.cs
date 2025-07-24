using System;

namespace Remizione
{
    /// <summary>
    /// Inventory
    /// </summary>
    public sealed class Inventory
    {
        // Constructor
        public Inventory(Actor owner)
        {
            this.Consumables = new ItemContainer(owner, InventoryCategory.Consumables);
            this.Junk = new ItemContainer(owner, InventoryCategory.Junk);
            this.KeyItems = new ItemContainer(owner, InventoryCategory.KeyItems);
            this.Skills = new ItemContainer(owner, InventoryCategory.Skills)
            {
                Size = 24
            };
            this.Trinkets = new ItemContainer(owner, InventoryCategory.Trinkets);
        }

        // Consumables
        public ItemContainer Consumables { get; }

        // GetContainer
        public ItemContainer GetContainer(InventoryCategory category)
        {
            return category switch
            {
                InventoryCategory.Consumables => Consumables,
                InventoryCategory.Junk => Junk,
                InventoryCategory.KeyItems => KeyItems,
                InventoryCategory.Skills => Skills,
                InventoryCategory.Trinkets => Trinkets,
                _ => throw new ArgumentException($"Invalid inventory category: {category}", nameof(category)),
            };
        }

        // Junk
        public ItemContainer Junk { get; }

        // KeyItems
        public ItemContainer KeyItems { get; }

        // Skills
        public ItemContainer Skills { get; }

        // Trinkets
        public ItemContainer Trinkets { get; }
    }
}
