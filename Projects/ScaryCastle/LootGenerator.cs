using Engendro;
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

        // CanSpawn
        private bool CanSpawn(ItemDefinition itemDef, int maxQ, Realm? realm, ItemCategory? category)
        {
            // Coin or player action
            if (itemDef.Name == nameof(Coin) || itemDef.Behavior == ItemBehavior.PlayerAction)
                return false;

            // Can spawn or match quality?
            if (itemDef.SpawnWeight <= 0 || itemDef.Quality > maxQ)
                return false;

            // Match realm
            if (realm.HasValue && itemDef.Realm != realm)
                return false;

            // Match category
            if (category.HasValue && itemDef.Category != category.Value)
                return false;

            // Avoid dropping duplicates for non-stackable items
            if (!itemDef.IsStackable && run.PlayerInventory.Find(itemDef.Name) != null)
                return false;

            return true;
        }

        // CheckDropSuccess
        private bool CheckDropSuccess(ThingDefinition def)
        {
            // Calculate base chances based on thing difficulty
            float chance = def.Difficulty switch
            {
                Difficulty.Easy => 0.05f,
                Difficulty.Normal => 0.12f,
                Difficulty.Hard => 0.25f,
                _ => 0.02f
            };

            // Up to 10% of more chances depending on the current floor
            chance += run.FloorProgress * 0.1f;

            // Player's luck
            chance += run.Traits.GetTotalTraitValue(TraitType.Luck);

            // Extra chances
            chance += def.DropSackChanceBonus;

            // There is alwqays a 5% chance og getting nothing
            return run.VolatileRng.NextDouble() <= float.Clamp(chance, 0, .95f);
        }

        // RollCoinAmount
        private int RollCoinAmount(ThingDefinition thingDef)
        {
            // Probabilidad base por dificultad
            float chance = thingDef.Difficulty switch
            {
                Difficulty.Easy => 0.1f,
                Difficulty.Normal => 0.3f,
                Difficulty.Hard => 0.5f,
                _ => 0.1f
            };

            // Modificadores de Suerte y Bonus
            chance += run.Traits.GetTotalTraitValue(TraitType.Luck);
            chance += thingDef.DropCoinChanceBonus;

            // Cap de seguridad (max 95%)
            float finalChance = float.Clamp(chance, 0, .95f);

            // Tirada de dados
            if (run.VolatileRng.NextDouble() > finalChance)
                return 0;

            // Cantidad entregada si la tirada tuvo éxito
            return thingDef.Difficulty switch
            {
                Difficulty.Easy => 1,
                Difficulty.Normal => 1,
                Difficulty.Hard => run.VolatileRng.Next(1, 3),
                _ => 1
            };
        }

        // SelectLootItem
        private ItemDefinition? SelectLootItem(RoomNode node, ThingDefinition entityDef)
        {
            var roomDef = node.Definition;
            int qualityBoost = entityDef.QualityBoost;

            if (node.Category == RoomCategory.Treasure)
                qualityBoost += 2;

            // Si estás en el Piso 1, maxQ se mantiene bajo (0-2) evitando épicos/legendarios desbalanceados.
            // En los pisos finales, el techo se eleva a 4-5.
            float progress = run.FloorProgress;
            int floorBonus = (int)(progress * 2f);
            int maxQ = Math.Clamp(((int)roomDef.Difficulty * 2) + 1 + qualityBoost + floorBonus, 0, 5);

            var table = new ChanceTable();

            // Reducción del "Empty Drop" en pisos profundos
            float emptyWeight = qualityBoost > 0 ? 0 : 10f * (1.0f - (progress * 0.6f));
            if (emptyWeight > 0)
                table.Add(ChanceTable.Nothing, emptyWeight, 1, null);

            Realm? lootRealm = entityDef.PreferredLootRealm ?? roomDef.PreferredLootRealm;
            ItemCategory? lootCategory = entityDef.PreferredLootCategory ?? roomDef.PreferredLootCategory;

            for (int i = 0; i < GameData.Items.Count; i++)
            {
                var itemDef = GameData.Items[i];

                if (!CanSpawn(itemDef, maxQ, lootRealm, lootCategory))
                    continue;

                float weight = itemDef.SpawnWeight;

                // Escala de peso para ítems valiosos según progreso y bonus de sala
                if (itemDef.Quality >= 3)
                    weight *= 1f + (progress * 1.5f);

                if (qualityBoost > 0 && itemDef.Quality >= 3)
                    weight *= 1.5f + qualityBoost;

                table.Add(itemDef.Name, weight, 1, itemDef);
            }

            return table.GetValue(run.VolatileRng)?.Context as ItemDefinition;
        }

        #endregion

        // RollForCoin
        public int RollForCoin(GameThing thing)
        {
            if (thing.Definition == null)
                return 0;

            // 1. Filtro rápido de DropMode
            if (thing.Definition.DropMode is LootDropMode.None or LootDropMode.SackOnly or LootDropMode.Custom)
                return 0;

            // 2. Procesa la tirada y cantidad de monedas en el método dedicado
            return RollCoinAmount(thing.Definition);
        }

        // RollForLoot
        public ItemDefinition? RollForLoot(GameThing thing)
        {
            if (run.Session.Room is not ProceduralRoom room)
                return null;

            if (thing.Definition == null)
                return null;

            // 1. Check drop mode
            if (thing.Definition.DropMode is LootDropMode.None or LootDropMode.CoinsOnly)
                return null;

            // 2. Check for custom drop
            if (thing.Definition.DropMode == LootDropMode.Custom && !string.IsNullOrEmpty(thing.CustomDropName))
                return GameData.Items.Find(thing.CustomDropName);

            // 3. Will drop?
            if (!CheckDropSuccess(thing.Definition))
                return null;

            // 4. Return loot
            return SelectLootItem(room.RoomNode, thing.Definition);
        }
    }
}