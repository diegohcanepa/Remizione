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
        internal static ItemDefinition? Get(GameSession session, RoomDefinition roomDefinition, Realm? lootRealm, ItemCategory? lootCategory, ItemCategory[]? denyCategories = null)
        {
            var candidates = new List<ItemDefinition>();
            int maxQ = ((int)roomDefinition.Difficulty * 2) + 1; // Tu escala 0-5

            lootRealm ??= roomDefinition.PreferredLootRealm;
            lootCategory ??= roomDefinition.PreferredLootCategory;

            foreach (var definition in ItemDefinition.All)
            {
                if (denyCategories.Contains(definition.Category))
                    continue;

                if (definition.Quality > maxQ)
                    continue;

                // Realm scope?
                if (lootRealm != null && definition.Realm != lootRealm)
                    continue;

                // Category scope?
                if (lootCategory != null && definition.Category != lootCategory)
                    continue;

                // Discard unique items
                // TODO: Must check inventory and all room bags
                if (!definition.IsStackable && session.Inventory.Find(definition.Name) != null)
                    continue;

                candidates.Add(definition);
            }

            // Pick con pesos y el multiplicador AdjustWeightByQuality que ya tenemos
            var table = new ChanceTable();
            foreach (var c in candidates)
            {
                float weight = AdjustWeightByQuality(roomDefinition.Difficulty, c.Quality, c.SpawnWeight);
                table.Add(c.Name, weight, 1, c);
            }

            return table.GetValue()?.Context as ItemDefinition;
        }

        // GetForVending
        internal static ItemDefinition GetForVending(GameSession session, RoomDefinition roomDefinition, Realm? lootRealm, ItemCategory? lootCategory)
        {
            /*
            // 1. Intentamos obtener el ítem ideal para esta habitación
            if (Get(session, roomDefinition, lootRealm, lootCategory, [ItemCategory.Pickup]) is MetaItem result)
                return result;
            */

            // 2. PLAN B: Si no hay nada que cumpla los filtros, 
            // buscamos cualquier item de calidad 0-1 que esté desbloqueado.
            var fallbackCandidates = new List<ItemDefinition>();

            foreach (var definition in ItemDefinition.All)
            {
                // Solo calidad baja para que sea un "item de relleno" seguro
                if (definition.Quality > 1)
                    continue;

                fallbackCandidates.Add(definition);
            }

            // Si por alguna razón bizarra no hay candidatos (muy raro), 
            // devolvemos un item básico por nombre que sepamos que existe.
            if (fallbackCandidates.Count == 0)
                throw new InvalidOperationException("Unable to find meta item.");

            // Devolvemos uno al azar de los básicos
            return fallbackCandidates[session.Random.Next(fallbackCandidates.Count)];
        }

        // GetPrice
        internal static int GetPrice(ItemDefinition item)
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

        // RollCoins
        internal static int RollCoins(GameSession session, RoomDefinition roomDefinition, ThingDefinition thingDefinition)
        {
            // 1. CHANCE BASE (Depende de la entidad y la suerte del pasivo)
            Ratio coinChance = thingDefinition.Difficulty switch
            {
                Difficulty.Easy => 0.20f,
                Difficulty.Normal => 0.40f,
                Difficulty.Hard => 0.60f,
                _ => 0.15f
            };

            // TODO: Reimplement
            /*
            if (session.Inventory.PassiveItem is Item gadget)
                coinChance += gadget.MetaItem.Effect.LuckBonus;
            */

            if (!coinChance.Roll())
                return 5;

            // 2. CANTIDAD BASE (Basada en la dificultad intrínseca del enemigo)
            int amount = thingDefinition.Difficulty switch
            {
                Difficulty.Easy => 1,   // Una moneda
                Difficulty.Normal => 2, // Dos monedas
                Difficulty.Hard => 3,   // Tres monedas
                _ => 1
            };

            // 3. EL FACTOR ROOM (AQUÍ usamos RoomDefinition)
            // Si la habitación es 'Hard', hay una chance bizarra de duplicar el drop
            // Esto hace que en zonas avanzadas sea más fácil llegar a los 15 coins
            if (roomDefinition.Difficulty == Difficulty.Hard && session.Random.NextDouble() < 0.4f)
            {
                amount += 1; // Bonus por estar en una zona peligrosa
            }
            else if (roomDefinition.Difficulty == Difficulty.Easy && amount > 2)
            {
                // Opcional: En zonas iniciales, limitamos el drop para evitar inflación temprana
                amount = 2;
            }

            return amount;
        }
    }
}
