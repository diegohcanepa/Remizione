using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// Generador de Loot orientado a Nodos (AOT-Friendly, Cero LINQ)
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
        private static float AdjustWeightByQuality(Difficulty difficulty, int itemQuality, float baseWeight)
        {
            float finalWeight = baseWeight;
            int roomVal = (int)difficulty;

            if (roomVal == 2 && itemQuality <= 1)
                finalWeight *= 0.1f;
            else if (roomVal == 2 && itemQuality >= 4)
                finalWeight *= 3.0f;

            return finalWeight;
        }

        // GetLoot
        private ItemDefinition? GetLoot(RoomNode node, Realm? lootRealm, ItemCategory? lootCategory, ItemCategory[]? denyCategories = null, int qualityBoost = 0, bool guaranteeDrop = false)
        {
            RoomDefinition def = node.Definition;

            // Integración de diseño: Los tesoros mejoran el qualityBoost
            if (node.RoomType == RoomType.SideRoom && node.SideRoomCategory == SideRoomCategory.Treasure)
                qualityBoost += 2;

            int maxQ = Math.Clamp(((int)def.Difficulty * 2) + 1 + qualityBoost, 0, 5);

            lootRealm ??= def.PreferredLootRealm;
            lootCategory ??= def.PreferredLootCategory;

            var table = new ChanceTable();

            // Lógica de "Empty Drop"
            float emptyWeight = (qualityBoost > 0 || guaranteeDrop) ? 0f : def.Difficulty switch
            {
                Difficulty.Easy => 15f,
                Difficulty.Normal => 5f,
                Difficulty.Hard => 1.5f,
                _ => 10.0f
            };

            if (emptyWeight > 0)
                table.Add("None", emptyWeight, 1, null);

            // Filtro Principal
            for (int i = 0; i < ItemDefinition.Definitions.All.Count; i++)
            {
                var itemDef = ItemDefinition.Definitions.All[i];

                if (itemDef.Name == nameof(Coin))
                    continue;

                if (IsDenied(itemDef.Category, denyCategories))
                    continue;

                if (itemDef.Quality > maxQ)
                    continue;

                if (lootRealm.HasValue && itemDef.Realm != lootRealm.Value)
                    continue;

                if (lootCategory.HasValue && itemDef.Category != lootCategory.Value)
                    continue;

                if (!itemDef.IsStackable && session.Inventory.Find(itemDef.Name) != null)
                    continue;

                float weight = AdjustWeightByQuality(def.Difficulty, itemDef.Quality, itemDef.SpawnWeight);
                if (qualityBoost > 0 && itemDef.Quality >= 3)
                    weight *= 1.5f + qualityBoost;

                table.Add(itemDef.Name, weight, 1, itemDef);
            }

            ItemDefinition? result = table.GetValue(session.Random)?.Context as ItemDefinition;

            // Fallback: Si se garantizaba un drop pero fallaron los filtros estrictos
            if (result == null && guaranteeDrop)
            {
                for (int i = 0; i < ItemDefinition.Definitions.All.Count; i++)
                {
                    var fallbackDef = ItemDefinition.Definitions.All[i];
                    if (fallbackDef.Quality <= 1)
                        table.Add(fallbackDef.Name, fallbackDef.SpawnWeight, 1, fallbackDef);
                }
                result = table.GetValue(session.Random)?.Context as ItemDefinition;
            }

            return result;
        }

        // IsDenied
        private static bool IsDenied(ItemCategory cat, ItemCategory[]? denies)
        {
            if (denies == null) return false;
            for (int i = 0; i < denies.Length; i++)
            {
                if (denies[i] == cat) return true;
            }
            return false;
        }

        // RollCoinAmount
        private int RollCoinAmount(RoomDefinition roomDef, ThingDefinition thingDef, float chanceMultiplier)
        {
            // Chance base según qué tan duro es el enemigo
            float baseChance = thingDef.Difficulty switch
            {
                Difficulty.Easy => 0.3f,   // 30%
                Difficulty.Normal => 0.5f, // 50%
                Difficulty.Hard => 0.9f,   // 90% (casi siempre tiran)
                _ => 0.1f
            };

            // Sumamos la suerte del jugador y multiplicamos por el factor del NPC/Enemigo
            float finalChance = (baseChance + session.Inventory.GetLuckFactor()) * chanceMultiplier;

            if (session.Random.NextDouble() > finalChance)
                return 0;

            // Cantidad de monedas (Isaac-style, montos chicos)
            return thingDef.Difficulty switch
            {
                Difficulty.Easy => 1,
                Difficulty.Normal => session.Random.Next(1, 3), // 1 a 2 monedas
                Difficulty.Hard => session.Random.Next(2, 5),   // 2 a 4 monedas
                _ => 1
            };
        }

        #endregion

        // GetPrice
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

        // RollForLoot
        public ItemDefinition? RollForLoot(GameThing thing, bool guaranteeDrop = false)
        {
            if (thing.DropMode != LootDropMode.Standard && thing.DropChanceMultiplier == 1)
                guaranteeDrop = true;

            // 1. FILTRO DE INSTANCIA: Si es None o solo Monedas, no hay chance de Saco
            if (thing.DropMode is LootDropMode.None or LootDropMode.CoinsOnly)
                return null;

            if (thing is not IThingDefinition t || t.Definition == null || session.Room is not ProceduralRoom room)
                return null;

            // 2. CÁLCULO DE CHANCE (Solo si no es garantizado)
            if (!guaranteeDrop)
            {
                float lootChance = t.Definition.Difficulty switch
                {
                    Difficulty.Easy => 0.05f,
                    Difficulty.Normal => 0.12f,
                    Difficulty.Hard => 0.25f,
                    _ => 0.02f
                };

                // Aplicamos el multiplicador de la instancia
                float finalChance = (lootChance + session.Inventory.GetLuckFactor()) * thing.DropChanceMultiplier;

                if (session.Random.NextDouble() > finalChance)
                    return null;
            }

            // 3. SELECCIÓN DEL ÍTEM
            // Si el modo es Custom, buscamos el ítem específico por nombre
            if (thing.DropMode == LootDropMode.Custom && !string.IsNullOrEmpty(thing.CustomDropName))
                return ItemDefinition.Definitions.Find(thing.CustomDropName);

            // De lo contrario, usamos el generador procedural por room node
            return GetLoot(room.RoomNode, t.Definition.PreferredLootRealm, t.Definition.PreferredLootCategory, null, t.Definition.QualityBoost, guaranteeDrop);
        }

        // TryDropCoins
        public void TryDropCoins(GameThing thing)
        {
            // 1. FILTRO DE INSTANCIA: Si el bicho está seteado para no dar nada o solo dar items, abortamos.
            if (thing.DropMode is LootDropMode.None or LootDropMode.SackOnly or LootDropMode.Custom)
                return;

            if (thing is not IThingDefinition t || t.Definition == null || session.Room is not ProceduralRoom room)
                return;

            // 2. Calculamos la cantidad pasando el multiplicador de la instancia
            int amount = RollCoinAmount(room.RoomNode.Definition, t.Definition, thing.DropChanceMultiplier);

            // 3. Instanciación física
            for (int i = 0; i < amount; i++)
            {
                if (room.CreateThingClone("Coin") is Coin coin)
                {
                    coin.Position = thing.Position;
                    // Offset aleatorio para que no caigan apiladas exactamente en el mismo píxel
                    coin.Position += new Vector2(
                        session.Random.Next(-6, 7),
                        session.Random.Next(-6, 7)
                    );
                    room.Children.Add(coin);
                }
            }
        }

        // TryDropLoot
        public bool TryDropLoot(GameThing thing)
        {
            ItemDefinition? itemDefinition = RollForLoot(thing);

            if (itemDefinition != null && session.Room is ProceduralRoom room)
            {
                if (room.CreateThingClone(nameof(Sack)) is Sack sack)
                {
                    sack.Loot = itemDefinition;
                    sack.Position = thing.Position;
                    room.Children.Add(sack);
                    return true;
                }
            }

            return false;
        }
    }
}