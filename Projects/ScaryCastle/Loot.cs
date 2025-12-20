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
        private static float AdjustWeightByQuality(Difficulty roomDiff, int itemQuality, float baseWeight)
        {
            float finalWeight = baseWeight;
            int roomVal = (int)roomDiff; // 0, 1, 2

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
        internal static MetaItem? Get(GameSession session, Difficulty difficulty, Realm? preferredRealm, ItemCategory? preferredCategory)
        {
            var candidates = new List<MetaItem>();
            int maxQ = ((int)difficulty * 2) + 1; // Tu escala 0-5

            foreach (var metaItem in MetaItem.AllItems)
            {
                if (!session.UnlockedPool.IsUnlocked(metaItem.Name))
                    continue;

                if (metaItem.Quality > maxQ)
                    continue;

                // Realm scope?
                if (preferredRealm != null && metaItem.Realm != preferredRealm)
                    continue;

                // Category scope?
                if (preferredCategory != null && metaItem.Category != preferredCategory)
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
                float weight = AdjustWeightByQuality(difficulty, c.Quality, c.Weight);
                table.Add(c.Name, weight, 1, c);
            }

            return table.GetValue()?.Context as MetaItem;
        }
    }
}
