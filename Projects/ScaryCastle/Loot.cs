using Engendro;
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
        internal static MetaItem? Get(GameSession session, RoomConfig roomConfig, Realm? lootRealm, ItemCategory? lootCategory, ItemCategory[]? denyCategories = null)
        {
            var candidates = new List<MetaItem>();
            int maxQ = ((int)roomConfig.Difficulty * 2) + 1; // Tu escala 0-5

            lootRealm ??= roomConfig.PreferredLootRealm;
            lootCategory ??= roomConfig.PreferredLootCategory;

            foreach (var metaItem in MetaItem.AllItems)
            {
                if (denyCategories.Contains(metaItem.Category))
                    continue;

                if (metaItem.Quality > maxQ)
                    continue;

                // Realm scope?
                if (lootRealm != null && metaItem.Realm != lootRealm)
                    continue;

                // Category scope?
                if (lootCategory != null && metaItem.Category != lootCategory)
                    continue;

                // Discard unique items
                if (metaItem.IsUnique && session.Inventory.Find(metaItem.Name) != null)
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

        // GetForVending
        internal static MetaItem GetForVending(GameSession session, RoomConfig roomConfig, Realm? lootRealm, ItemCategory? lootCategory)
        {
            // 1. Intentamos obtener el ítem ideal para esta habitación
            if (Get(session, roomConfig, lootRealm, lootCategory, [ItemCategory.Pickup]) is MetaItem result)
                return result;

            // 2. PLAN B: Si no hay nada que cumpla los filtros, 
            // buscamos cualquier item de calidad 0-1 que esté desbloqueado.
            var fallbackCandidates = new List<MetaItem>();

            foreach (var metaItem in MetaItem.AllItems)
            {
                // Solo calidad baja para que sea un "item de relleno" seguro
                if (metaItem.Quality > 1)
                    continue;

                fallbackCandidates.Add(metaItem);
            }

            // Si por alguna razón bizarra no hay candidatos (muy raro), 
            // devolvemos un item básico por nombre que sepamos que existe.
            if (fallbackCandidates.Count == 0)
                throw new InvalidOperationException("Unable to find meta item.");

            // Devolvemos uno al azar de los básicos
            return fallbackCandidates[session.Random.Next(fallbackCandidates.Count)];
        }

        // GetPrice
        internal static int GetPrice(MetaItem item)
        {
            // Mapeamos la Quality (0-5) a tus precios simples (5, 10, 15)
            return item.Quality switch
            {
                0 or 1 => 5,  // Items básicos o consumibles
                2 or 3 => 10, // Herramientas y gadgets de nivel medio
                4 or 5 => 15, // Items poderosos o de alta calidad
                _ => 5
            };
        }

        // RollTickets
        internal static int RollTickets(GameSession session, RoomConfig roomConfig, ThingConfig entityConfig)
        {
            // 1. CHANCE BASE (Depende de la entidad y la suerte del pasivo)
            Ratio ticketChance = entityConfig.Difficulty switch
            {
                Difficulty.Easy => 0.20f,
                Difficulty.Normal => 0.40f,
                Difficulty.Hard => 0.60f,
                _ => 0.15f
            };

            if (session.Inventory.Gadget is Item gadget)
                ticketChance += gadget.MetaItem.Effect.LuckBonus;

            if (!ticketChance.Roll())
                return 0;

            // 2. CANTIDAD BASE (Basada en la dificultad intrínseca del enemigo)
            int amount = entityConfig.Difficulty switch
            {
                Difficulty.Easy => 1,   // Una moneda
                Difficulty.Normal => 2, // Dos monedas
                Difficulty.Hard => 3,   // Tres monedas
                _ => 1
            };

            // 3. EL FACTOR ROOM (AQUÍ usamos RoomConfig)
            // Si la habitación es 'Hard', hay una chance bizarra de duplicar el drop
            // Esto hace que en zonas avanzadas sea más fácil llegar a los 15 tickets
            if (roomConfig.Difficulty == Difficulty.Hard && session.Random.NextDouble() < 0.4f)
            {
                amount += 1; // Bonus por estar en una zona peligrosa
            }
            else if (roomConfig.Difficulty == Difficulty.Easy && amount > 2)
            {
                // Opcional: En zonas iniciales, limitamos el drop para evitar inflación temprana
                amount = 2;
            }

            return amount;
        }
    }
}
