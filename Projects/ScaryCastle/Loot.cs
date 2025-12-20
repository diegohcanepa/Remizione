using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// Loot
    /// </summary>
    internal static class Loot
    {
        #region Private members

        // AdjustWeightByQuality
        private static float AdjustWeightByQuality(Difficulty difficulty, int itemQuality, float baseWeight)
        {
            float finalWeight = baseWeight;
            int roomVal = (int)difficulty; // 0, 1, 2

            // Si el item es de calidad baja para una sala de nivel alto
            if (roomVal == 2 && itemQuality <= 1)
                finalWeight *= 0.1f; // Casi desaparecen los items de relleno

            // Si el item es de calidad alta (4 o 5) en sala Hard
            else if (roomVal == 2 && itemQuality >= 4)
                finalWeight *= 3.0f; // Potenciamos el loot de "Tier alto"

            return finalWeight;
        }

        #endregion

        // Get
        internal static MetaItem? Get(GameSession session, RoomConfig roomConfig)
        {
            var candidates = new List<MetaItem>();
            int maxQ = ((int)roomConfig.Difficulty * 2) + 1; // Tu escala 0-5

            foreach (var metaItem in MetaItem.AllItems)
            {
                if (!session.UnlockedPool.IsUnlocked(metaItem.Name))
                    continue;

                if (metaItem.Quality > maxQ)
                    continue;

                // Realm scope?
                if (roomConfig.PreferredLootRealm != null && metaItem.Realm != roomConfig.PreferredLootRealm)
                    continue;

                // Category scope?
                if (roomConfig.PreferredLootCategory != null && metaItem.Category != roomConfig.PreferredLootCategory)
                    continue;

                // Discard gadgets already in inventory
                if (metaItem.Category == ItemCategory.Gadget && session.Inventory.Find(metaItem.Name) != null)
                    continue;

                candidates.Add(metaItem);
            }

            // Pick con pesos y el multiplicador AdjustWeightByQuality que ya tenemos
            var table = new ChanceTable();
            foreach (var c in candidates)
            {
                float weight = AdjustWeightByQuality(roomConfig.Difficulty, c.Quality, c.Weight);
                table.Add(c.Name, weight, 1, c);
            }

            return table.GetValue()?.Context as MetaItem;
        }
    }
}
