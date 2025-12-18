using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
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

            // Get an unlocked meta item based on realm, category or name
            if (Enum.IsDefined(typeof(Realm), lootItem.Name))
            {
                metaItem = room.Session.UnlockedPool.GetRandomMetaItem(Enum.Parse<Realm>(lootItem.Name));
            }
            else if (Enum.IsDefined(typeof(ItemCategory), lootItem.Name))
            {
                metaItem = room.Session.UnlockedPool.GetRandomMetaItem(Enum.Parse<ItemCategory>(lootItem.Name));
            }
            else
            {
                metaItem = room.Session.UnlockedPool.FindMetaItem(lootItem.Name);
            }

            if (metaItem != null)
            {
                // Avoid looting unique items already in inventory
                if (metaItem.Category == ItemCategory.Gadget && room.Session.Inventory.Find(metaItem.Name) != null)
                    return false;

                room.Session.ObjectPools.Pickups.Get()?.Drop(room, position, metaItem);

                return true;
            }

            return false;
        }
    }
}
