using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// Loot
    /// </summary>
    internal static class Loot
    {
        // TryDropLoot
        internal static bool TryDropLoot(GameRoom room, ChanceTable chanceTable, Vector2 position, out MetaItem? metaItem)
        {
            metaItem = null;

            if (chanceTable.GetValue() is not ChanceTableItem lootItem)
                return false;

            // Get meta item based on realm, category or name
            if (Enum.IsDefined(typeof(Realm), lootItem.Name))
            {
                metaItem = room.Session.MetaItemPool.GetRandomItem(Enum.Parse<Realm>(lootItem.Name));
            }
            else if (Enum.IsDefined(typeof(ItemCategory), lootItem.Name))
            {
                metaItem = room.Session.MetaItemPool.GetRandomItem(Enum.Parse<ItemCategory>(lootItem.Name));
            }
            else
            {
                metaItem = room.Session.MetaItemPool.Find(lootItem.Name);
            }

            if (metaItem != null)
            {
                // Avoid looting unique items already in inventory
                if (metaItem.Category == ItemCategory.Gadgets && room.Session.Inventory.Find(metaItem.Name) != null)
                    return false;

                room.Session.ObjectPools.Pickups.Get()?.Drop(room, position, metaItem);

                return true;
            }

            return false;
        }
    }
}
