using Engendro;
using System;
using System.Collections.Generic;
using System.Linq; // Necesario para .Contains

namespace ScaryCastle
{
    /// <summary>
    /// LootGenerator: Centraliza toda la lógica de economía y generación de ítems.
    /// </summary>
    public sealed class LootGenerator
    {
        private readonly GameSession session;

        public LootGenerator(GameSession session)
        {
            this.session = session;
        }

        #region Private members

        private float AdjustWeightByQuality(Difficulty difficulty, int itemQuality, float baseWeight)
        {
            float finalWeight = baseWeight;
            int roomVal = (int)difficulty; // 0=Easy, 1=Normal, 2=Hard

            // Lógica de ajuste de pesos según la dificultad de la sala
            if (roomVal == 2 && itemQuality <= 1)
                finalWeight *= 0.1f; // Castigamos items basura en zonas Hard
            else if (roomVal == 2 && itemQuality >= 4)
                finalWeight *= 3.0f; // Premiamos items Top en zonas Hard

            return finalWeight;
        }

        #endregion

        // Get
        public ItemDefinition? Get()
        {
            return Get(null, null);
        }

        // Get
        public ItemDefinition? Get(GameThing thing)
        {
            if (thing.Definition is ThingDefinition def)
                return Get(def.PreferredLootRealm, def.PreferredLootCategory, null, def.QualityBoost);
            else
                return null;
        }

        // Get
        public ItemDefinition? Get(Realm? lootRealm, ItemCategory? lootCategory, ItemCategory[]? denyCategories = null, int qualityBoost = 0)
        {
            if (session.Room is not ProceduralRoom room)
                return null;

            return Get(room.Definition, lootRealm, lootCategory, denyCategories, qualityBoost);
        }

        // Get
        public ItemDefinition? Get(RoomDefinition roomDefinition, Realm? lootRealm, ItemCategory? lootCategory, ItemCategory[]? denyCategories = null, int qualityBoost = 0)
        {
            var candidates = new List<ItemDefinition>();

            // 1. CALCULO DE CALIDAD MÁXIMA
            // qualityBoost puede subir el techo de calidad, pero lo clipeamos a 5 (Max Tier)
            // Si qualityBoost es negativo (ej. -1), baja la calidad máxima permitida.
            int maxQ = Math.Clamp(((int)roomDefinition.Difficulty * 2) + 1 + qualityBoost, 0, 5);

            // Jerarquía: Parámetro Manual > Preferencia de Sala
            lootRealm ??= roomDefinition.PreferredLootRealm;
            lootCategory ??= roomDefinition.PreferredLootCategory;

            foreach (var definition in ItemDefinition.All)
            {
                if (denyCategories != null && denyCategories.Contains(definition.Category))
                    continue;

                // Filtro de Calidad Máxima
                if (definition.Quality > maxQ)
                    continue;

                // Filtro de Reino
                if (lootRealm != null && definition.Realm != lootRealm)
                    continue;

                // Filtro de Categoría
                if (lootCategory != null && definition.Category != lootCategory)
                    continue;

                // Filtro de Unicidad (Items únicos no se repiten si ya los tiene)
                if (!definition.IsStackable && session.Inventory.Find(definition.Name) != null)
                    continue;

                candidates.Add(definition);
            }

            var table = new ChanceTable();

            // 2. LÓGICA DE "EMPTY DROP" (NADA)
            // Si hay qualityBoost positivo (ej. Cofre Dorado), la chance de vacío es 0.
            // Si no, depende de la dificultad de la sala.
            float emptyWeight = qualityBoost > 0 ? 0f : roomDefinition.Difficulty switch
            {
                Difficulty.Easy => 15.0f,   // Muy probable que no salga nada al principio
                Difficulty.Normal => 5.0f,  // Balanceado
                Difficulty.Hard => 1.5f,    // Raro que no salga nada en Hard
                _ => 10.0f
            };

            // Agregamos la opción "Nada" a la tabla
            table.Add("None", emptyWeight, 1, null);

            foreach (var c in candidates)
            {
                float weight = AdjustWeightByQuality(roomDefinition.Difficulty, c.Quality, c.SpawnWeight);

                // 3. BOOST DE PESO PARA ÍTEMS RAROS
                // Si es un drop con boost, los items de calidad >= 3 aparecen mucho más fácil.
                if (qualityBoost > 0 && c.Quality >= 3)
                {
                    weight *= 1.5f + qualityBoost;
                }

                table.Add(c.Name, weight, 1, c);
            }

            return table.GetValue()?.Context as ItemDefinition;
        }

        // GetForVending: Se mantiene igual, un Plan B seguro.
        public ItemDefinition GetForVending(RoomDefinition roomDefinition, Realm? lootRealm, ItemCategory? lootCategory)
        {
            // Intentamos obtener algo decente primero
            var item = Get(roomDefinition, lootRealm, lootCategory, qualityBoost: 1); // Boost ligero para tiendas
            if (item != null) return item;

            // Fallback a items básicos si todo falla
            var fallbackCandidates = ItemDefinition.All.Where(x => x.Quality <= 1).ToList();

            if (fallbackCandidates.Count == 0)
                throw new InvalidOperationException("No items available for fallback.");

            return fallbackCandidates[session.Random.Next(fallbackCandidates.Count)];
        }

        // GetPrice (Estático)
        public static int GetPrice(ItemDefinition item)
        {
            return item.Quality switch
            {
                0 or 1 => 5,
                2 or 3 => 10,
                4 or 5 => 15,
                _ => 5
            };
        }

        // RollCoins
        public int RollCoins(RoomDefinition roomDefinition, ThingDefinition thingDefinition)
        {
            // Chance base
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

            // Cantidad base
            int amount = thingDefinition.Difficulty switch
            {
                Difficulty.Easy => 1,
                Difficulty.Normal => 2,
                Difficulty.Hard => 3,
                _ => 1
            };

            // Modificadores de Sala
            if (roomDefinition.Difficulty == Difficulty.Hard && session.Random.NextDouble() < 0.4f)
            {
                amount += 1; // Bonus de riesgo
            }
            else if (roomDefinition.Difficulty == Difficulty.Easy && amount > 2)
            {
                amount = 2; // Cap inicial
            }

            return amount;
        }
    }
}