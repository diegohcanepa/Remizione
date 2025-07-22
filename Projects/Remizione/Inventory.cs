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
            this.Equipment = new ItemContainer(owner, InventoryCategory.Equipment);
            this.KeyItems = new ItemContainer(owner, InventoryCategory.KeyItems);
            this.Skills = new ItemContainer(owner, InventoryCategory.Skills)
            {
                Size = 24
            };
        }

        // Consumables
        public ItemContainer Consumables { get; }

        // Equipment
        public ItemContainer Equipment { get; }

        // GetContainer
        public ItemContainer GetContainer(InventoryCategory category)
        {
            return category switch
            {
                InventoryCategory.Consumables => Consumables,
                InventoryCategory.Equipment => Equipment,
                InventoryCategory.KeyItems => KeyItems,
                InventoryCategory.Skills => Skills,
                _ => throw new ArgumentException($"Invalid inventory category: {category}", nameof(category)),
            };
        }

        // KeyItems
        public ItemContainer KeyItems { get; }

        // Skills
        public ItemContainer Skills { get; }
    }
}
