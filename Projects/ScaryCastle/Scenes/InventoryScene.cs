using System;
using System.Collections.Generic;
using System.Text;

namespace ScaryCastle
{
    // InventoryScene
    public sealed class InventoryScene : ItemContainerScene
    {
        // Constructor
        public InventoryScene(ItemContainer container)
            : base(container, Atlases.UI.InventoryItemSlot)
        {
        }
    }
}
