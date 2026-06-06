using Engendro;
using Microsoft.Xna.Framework;
using System;

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

                if (!itemDef.IsStackable && session.PlayerInventory.Find(itemDef.Name) != null)
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
        private int RollCoinAmount(ThingDefinition thingDef, float chanceBonus)
        {
            // 1. Bloqueo rápido: Si el bonus es negativo o el modo de drop lo prohíbe, 0 monedas.
            // (Asumimos que el chequeo de DropMode se hace en TryDropCoins antes de llamar aquí)
            if (chanceBonus < 0)
                return 0;

            // 2. Base por dificultad (Valores planos de probabilidad)
            float chance = thingDef.Difficulty switch
            {
                Difficulty.Easy => 0.3f,   // 30%
                Difficulty.Normal => 0.5f, // 50%
                Difficulty.Hard => 0.9f,   // 90%
                _ => 0.1f
            };

            // 3. Suma de modificadores
            // Suerte: Cada punto de Luck suma un +10% de probabilidad de encontrar monedas
            if (session.CurrentRun != null)
            {
                chance += session.PlayerStats.Luck.Value * 0.1f;
            }

            // Bonus de la instancia (Si quieres un +30% de chances, pasas 0.3f)
            chance += chanceBonus;

            // 4. El Roll (Con un cap de 98% para dejar siempre un margen mínimo de error, 
            // a menos que el diseño pida 100% garantizado)
            float finalChance = MathHelper.Clamp(chance, 0.0f, 0.98f);

            if (session.Random.NextDouble() > finalChance)
                return 0;

            // 5. Cantidad de monedas (Lógica de cantidad según dificultad)
            // Se mantiene la precisión de Cero LINQ y switch expressions de C# 13
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

        // RollForCoin
        public int RollForCoin(GameThing thing)
        {
            if ((thing as IThingDefinition)?.Definition is not { } def)
                return 0;

            // 1. FILTRO DE INSTANCIA: Si el bicho está seteado para no dar nada o solo dar bolsa, abortamos.
            if (def.DropMode is LootDropMode.None or LootDropMode.SackOnly or LootDropMode.Custom)
                return 0;

            if (thing is not IThingDefinition t || t.Definition == null || session.Room is not ProceduralRoom)
                return 0;

            // 2. Calculamos la cantidad pasando el multiplicador de la instancia
            return RollCoinAmount(t.Definition, def.DropCoinChanceBonus);
        }

        // RollForLoot
        public ItemDefinition? RollForLoot(GameThing thing, bool guaranteeDrop = false)
        {
            if ((thing as IThingDefinition)?.Definition is not { } def)
                return null;

            // 1. Validaciones de estado (Bánatelo rápido)
            if (def.DropMode is LootDropMode.None or LootDropMode.CoinsOnly)
                return null;

            if (thing is not IThingDefinition t || t.Definition == null || session.Room is not ProceduralRoom room)
                return null;

            // 2. Lógica de Garantía (Usamos un valor centinela como 1.0 o una flag)
            // Si el bonus es 1.0 (100%) o más, es drop garantizado.
            if (def.DropSackChanceBonus >= 1.0f)
                guaranteeDrop = true;

            if (!guaranteeDrop)
            {
                // 3. Base por dificultad (Valores planos)
                float chance = t.Definition.Difficulty switch
                {
                    Difficulty.Easy => 0.05f,   // 5%
                    Difficulty.Normal => 0.12f, // 12%
                    Difficulty.Hard => 0.25f,   // 25%
                    _ => 0.02f
                };

                // 4. Suma de modificadores (Simple y sólido)
                // Suerte: Cada punto de Luck es un +5% plano
                if (session.CurrentRun != null)
                {
                    chance += session.PlayerStats.Luck.Value * 0.05f;
                }

                // Bonus de la instancia (Aquí es donde sumas tu 0.30f si quieres un +30%)
                // IMPORTANTE: Cambia mentalmente 'Multiplier' por 'Bonus'
                chance += def.DropSackChanceBonus;

                // 5. El "Roll" con Cap
                // Nunca dejamos que sea 100% a menos que sea guaranteeDrop explícito
                float finalChance = MathHelper.Clamp(chance, 0.0f, 0.95f);

                if (session.Random.NextDouble() > finalChance)
                    return null;
            }

            // 6. Selección de Item
            if (def.DropMode == LootDropMode.Custom && !string.IsNullOrEmpty(thing.CustomDropName))
                return ItemDefinition.Definitions.Find(thing.CustomDropName);

            return GetLoot(room.RoomNode, t.Definition.PreferredLootRealm, t.Definition.PreferredLootCategory, null, t.Definition.QualityBoost, guaranteeDrop);
        }
    }
}