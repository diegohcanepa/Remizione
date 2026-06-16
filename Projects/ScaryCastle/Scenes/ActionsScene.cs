using System;
using System.Collections.Generic;
using System.Text;

namespace ScaryCastle
{
    // ActionsScene
    public sealed class ActionsScene : ItemContainerScene
    {
        // Constructor
        public ActionsScene(ItemContainer container)
            : base(container, Atlases.UI.InventoryActionSlot)
        {
        }
    }
}
