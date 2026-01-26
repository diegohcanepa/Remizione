using Engendro;
using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// LootGenerator
    /// </summary>
    public sealed class LootGenerator
    {
        private readonly GameSession session;

        // Constructor
        public LootGenerator(GameSession session)
        {
            this.session = session;
        }

        #region Private members

        // AdjustWeightByQuality
        private float AdjustWeightByQuality(Difficulty difficulty, int itemQuality, float baseWeight)
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
        public ItemDefinition? Get()
        {
            return Get(null, null);
        }

        // Get
        public ItemDefinition? Get(Realm? lootRealm, ItemCategory? lootCategory, ItemCategory[]? denyCategories = null)
        {
            if (session.Room is not ProceduralRoom room)
                return null;

            return Get(room.Definition, lootRealm, lootCategory, denyCategories);
        }

        // Get
        public ItemDefinition? Get(RoomDefinition roomDefinition, Realm? lootRealm, ItemCategory? lootCategory, ItemCategory[]? denyCategories = null)
        {
            var candidates = new List<ItemDefinition>();
            int maxQ = ((int)roomDefinition.Difficulty * 2) + 1; // Tu escala 0-5

            lootRealm ??= roomDefinition.PreferredLootRealm;
            lootCategory ??= roomDefinition.PreferredLootCategory;

            foreach (var definition in ItemDefinition.All)
            {
                if (denyCategories != null && denyCategories.Contains(definition.Category))
                    continue;

                if (definition.Quality > maxQ)
                    continue;

                if (lootRealm != null && definition.Realm != lootRealm)
                    continue;

                if (lootCategory != null && definition.Category != lootCategory)
                    continue;

                if (!definition.IsStackable && session.Inventory.Find(definition.Name) != null)
                    continue;

                candidates.Add(definition);
            }

            var table = new ChanceTable();

            // --- LÓGICA DE "NADA" (EMPTY DROP) ---
            // El peso del vacío disminuye a medida que aumenta la dificultad.
            float emptyWeight = roomDefinition.Difficulty switch
            {
                Difficulty.Easy => 15.0f,   // Muy probable que no salga nada en salas fáciles
                Difficulty.Normal => 5.0f,   // Balanceado
                Difficulty.Hard => 1.5f,     // En salas Hard es casi seguro que algo cae
                _ => 10.0f
            };

            // Agregamos la opción nula a la tabla. 
            // Si sale elegida, GetValue().Context será null.
            table.Add("<None>", emptyWeight, 1, null);

            foreach (var c in candidates)
            {
                float weight = AdjustWeightByQuality(roomDefinition.Difficulty, c.Quality, c.SpawnWeight);
                table.Add(c.Name, weight, 1, c);
            }

            return table.GetValue()?.Context as ItemDefinition;
        }

        // GetPrice
        public static int GetPrice(ItemDefinition item)
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
        public int RollCoins(RoomDefinition roomDefinition, ThingDefinition thingDefinition)
        {
            // 1. CHANCE BASE (Depende de la entidad y la suerte del pasivo)
            Ratio coinChance = thingDefinition.Difficulty switch
            {
                Difficulty.Easy => 0.2f,
                Difficulty.Normal => 0.4f,
                Difficulty.Hard => 0.6f,
                _ => 0.15f
            };

            coinChance += session.Inventory.GetLuckFactor();

            if (!coinChance.Roll())
                return 0;

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
