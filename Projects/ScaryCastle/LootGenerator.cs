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
        private readonly Run run;

        // Constructor
        public LootGenerator(Run run)
        {
            this.run = run;
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
        private ItemDefinition? GetLoot(RoomNode node, Realm? lootRealm, ItemCategory? lootCategory, ItemCategory[]? denyCategories = null, int qualityBoost = 0)
        {
            const string NoneValue = "None";

            RoomDefinition def = node.Definition;

            // Integración de diseño: Los tesoros mejoran el qualityBoost
            if (node.Category == RoomCategory.Treasure)
                qualityBoost += 2;

            int maxQ = Math.Clamp(((int)def.Difficulty * 2) + 1 + qualityBoost, 0, 5);

            lootRealm ??= def.PreferredLootRealm;
            lootCategory ??= def.PreferredLootCategory;

            var table = new ChanceTable();

            // Lógica de "Empty Drop"
            float emptyWeight = qualityBoost > 0 ? 0 : def.Difficulty switch
            {
                Difficulty.Easy => 15f,
                Difficulty.Normal => 5f,
                Difficulty.Hard => 1.5f,
                _ => 10.0f
            };

            if (emptyWeight > 0)
                table.Add(NoneValue, emptyWeight, 1, null);

            // Filtro Principal
            for (int i = 0; i < ItemDefinition.Data.All.Count; i++)
            {
                var itemDef = ItemDefinition.Data.All[i];

                if (itemDef.Name == nameof(Coin) || itemDef.Behavior == ItemBehavior.PlayerAction)
                    continue;

                if (itemDef.SpawnWeight <= 0)
                    continue;

                if (IsDenied(itemDef.Category, denyCategories))
                    continue;

                if (lootRealm.HasValue && itemDef.Realm != lootRealm.Value)
                    continue;

                if (lootCategory.HasValue && itemDef.Category != lootCategory.Value)
                    continue;

                if (!itemDef.IsStackable && run.PlayerInventory.Find(itemDef.Name) != null)
                    continue;

                if (itemDef.Quality > maxQ)
                    continue;

                float weight = AdjustWeightByQuality(def.Difficulty, itemDef.Quality, itemDef.SpawnWeight);
                if (qualityBoost > 0 && itemDef.Quality >= 3)
                    weight *= 1.5f + qualityBoost;

                table.Add(itemDef.Name, weight, 1, itemDef);
            }

            return table.GetValue(run.VolatileRng)?.Context as ItemDefinition;
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
        private int RollCoinAmount(ThingDefinition thingDef)
        {
            // 1. Bloqueo rápido: Si el bonus es negativo o el modo de drop lo prohíbe, 0 monedas.
            // (Asumimos que el chequeo de DropMode se hace en TryDropCoins antes de llamar aquí)
            if (thingDef.DropCoinChanceBonus < 0)
                return 0;

            // 2. Base por dificultad (Valores planos de probabilidad)
            float chance = thingDef.Difficulty switch
            {
                Difficulty.Easy => .1f,   // 10%
                Difficulty.Normal => .3f, // 30%
                Difficulty.Hard => .5f,   // 50%
                _ => .1f
            };

            // 3. Suma de modificadores
            // Suerte: Cada punto de Luck suma un +10% de probabilidad de encontrar monedas
            chance += run.Traits.GetTotalTraitValue(TraitType.Luck);

            // Bonus de la instancia (Si quieres un +30% de chances, pasas 0.3f)
            chance += thingDef.DropCoinChanceBonus;

            // 4. El Roll (Con un cap de 98% para dejar siempre un margen mínimo de error, 
            // a menos que el diseño pida 100% garantizado)
            float finalChance = MathHelper.Clamp(chance, 0, .98f);

            if (run.VolatileRng.NextDouble() > finalChance)
                return 0;

            // 5. Cantidad de monedas (Lógica de cantidad según dificultad)
            return thingDef.Difficulty switch
            {
                Difficulty.Easy => 1,
                Difficulty.Normal => 1,
                Difficulty.Hard => run.VolatileRng.Next(1, 3),
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

            // 2. Calculamos la cantidad pasando el multiplicador de la instancia
            return RollCoinAmount(def);
        }

        // RollForLoot
        public ItemDefinition? RollForLoot(GameThing thing)
        {
            if (run.Session.Room is not ProceduralRoom room)
                return null;

            if ((thing as IThingDefinition)?.Definition is not { } def)
                return null;

            if (def.DropMode is LootDropMode.None or LootDropMode.CoinsOnly)
                return null;

            // Custom drop
            if (def.DropMode == LootDropMode.Custom && !string.IsNullOrEmpty(thing.CustomDropName))
                return ItemDefinition.Data.Find(thing.CustomDropName);

            float chance = def.Difficulty switch
            {
                Difficulty.Easy => 0.05f,
                Difficulty.Normal => 0.12f,
                Difficulty.Hard => 0.25f,
                _ => 0.02f
            };

            chance += run.Traits.GetTotalTraitValue(TraitType.Luck);
            chance += def.DropSackChanceBonus;

            float finalChance = MathHelper.Clamp(chance, 0f, 0.95f);

            if (run.VolatileRng.NextDouble() > finalChance)
                return null;

            return GetLoot(room.RoomNode, def.PreferredLootRealm, def.PreferredLootCategory, null, def.QualityBoost);
        }
    }
}