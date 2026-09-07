using Engendro;
using System;

namespace Remizione
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
            // TODO: Check
            /*
            if (!itemDef.IsStackable && run.PlayerInventory.Find(itemDef.Name) != null)
                return false;
            */

            return true;
        }

        // CheckDropSuccess
        private bool CheckDropSuccess(ThingDefinition def)
        {
            // Probabilidad base
            float chance = def.Difficulty switch
            {
                Difficulty.Easy => 0.20f,   // Subimos de 0.05 a 0.20
                Difficulty.Normal => 0.40f, // Subimos de 0.12 a 0.40
                Difficulty.Hard => 0.70f,   // Subimos de 0.25 a 0.70
                _ => 0.10f
            };

            // Bonos EXCLUSIVOS de la Catacumba
            if (session.CurrentRun != null)
                chance += session.CurrentRun.Progress * 0.15f;

            // Bonus propio del contenedor/enemigo (Aplica siempre)
            chance += def.DropLootChanceBonus;

            var rng = session.CurrentRun?.VolatileRng ?? session.MasterRunRng;

            return rng.NextDouble() <= Math.Clamp(chance, 0f, 0.95f);
        }

        // SelectLootItem
        private ItemDefinition? SelectLootItem(ThingDefinition entityDef, RoomNode? node)
        {
            // 1. Contexto de la Sala/Habitación
            var roomDef = node?.Definition;
            int qualityBoost = entityDef.QualityBoost;

            if (node?.Category == RoomCategory.Treasure)
                qualityBoost += 2;

            // 2. Manejo de Progreso (Catacumba vs. Superficie)
            bool isInRun = session.CurrentRun != null;
            float progress = session.CurrentRun is Run run ? run.Progress : 0f;

            // Si estás en las Catacumbas, el progreso incrementa la calidad posible.
            // En la Superficie, la calidad base depende únicamente de la dificultad de la sala o de la entidad.
            int roomDifficulty = roomDef != null ? (int)roomDef.Difficulty : (int)entityDef.Difficulty;
            int floorBonus = isInRun ? (int)(progress * 2f) : 0;

            int maxQ = Math.Clamp((roomDifficulty * 2) + 1 + qualityBoost + floorBonus, 0, 5);

            var table = new ChanceTable();

            // Si ya pasamos CheckDropSuccess, en la Superficie el 'emptyWeight' 
            // debería ser muy bajo (ej. 1.0f) o directamente 0 si la entidad tenía un QualityBoost.
            float emptyWeight = qualityBoost > 0
                ? 0f
                : (isInRun ? 10f * (1.0f - (progress * 0.6f)) : 1.0f); // Bajado a 1.0f en la Superficie

            if (emptyWeight > 0)
                table.Add(ChanceTable.Nothing, emptyWeight, 1, null);

            // 4. Dominios y Categorías preferidas
            Realm? lootRealm = entityDef.PreferredLootRealm ?? roomDef?.PreferredLootRealm;
            ItemCategory? lootCategory = entityDef.PreferredLootCategory ?? roomDef?.PreferredLootCategory;

            // 5. Filtrado y Ponderación de Ítems
            for (int i = 0; i < GameData.Items.Count; i++)
            {
                var itemDef = GameData.Items[i];

                if (!CanSpawn(itemDef, maxQ, lootRealm, lootCategory))
                    continue;

                float weight = itemDef.SpawnWeight;

                // Bonificadores de peso para ítems valiosos (calidad >= 3)
                if (isInRun)
                {
                    if (itemDef.Quality >= 3)
                        weight *= 1f + (progress * 1.5f);

                    if (qualityBoost > 0 && itemDef.Quality >= 3)
                        weight *= 1.5f + qualityBoost;
                }

                table.Add(itemDef.Name, weight, 1, itemDef);
            }

            // 6. Selección de RNG según contexto
            var rng = session.CurrentRun?.VolatileRng ?? session.MasterRunRng;
            return table.GetValue(rng)?.Context as ItemDefinition;
        }

        #endregion

        // RollForLoot
        public ItemDefinition? RollForLoot(GameThing thing)
        {
            // No room
            if (session.Room is not GameRoom room)
                return null;

            // No definition
            if (thing.Definition == null)
                return null;

            // 1. Check drop mode
            if (thing.Definition.DropMode is LootDropMode.None)
                return null;

            // 2. Check for custom drop
            if (thing.Definition.DropMode == LootDropMode.Custom && !string.IsNullOrEmpty(thing.CustomDropName))
                return GameData.Items.Find(thing.CustomDropName);

            // 3. Will drop?
            if (!CheckDropSuccess(thing.Definition))
                return null;

            // 4. Return loot
            return SelectLootItem(thing.Definition, (room as ProceduralRoom)?.RoomNode);
        }
    }
}