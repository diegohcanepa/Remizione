using Engendro;
using System;

namespace Remizione
{
    /// <summary>
    /// LootGenerator - Algoritmo de drop directo y exclusivo estilo Souls/Elden Ring.
    /// </summary>
    public sealed class LootGenerator
    {
        private readonly GameSession session;

        // Constructor
        public LootGenerator(GameSession session)
        {
            this.session = session;
        }

        #region Private methods

        // CheckDropSuccess
        private static bool CheckDropSuccess(ThingDefinition def, Random rng)
        {
            // Probabilidad base asignada o por dificultad de la entidad
            float baseChance = def.DropLootChanceBonus > 0
                ? def.DropLootChanceBonus
                : GetBaseChanceByDifficulty(def.Difficulty);

            // Stat de Suerte del Jugador (Discovery Multiplier estilo Souls)
            // Ejemplo: Luck = 0 -> Multiplicador 1.0 (Sin cambios)
            // Ejemplo: Luck = 50 -> Multiplicador 1.5 (+50% de probabilidad)
            float playerLuck = 0;// session.PlayerTraits?.GetTotalTraitValue(TraitType.Luck) ?? 0f;
            float finalChance = baseChance * (1f + (playerLuck / 100f));

            // Si el dado supera la probabilidad final, el enemigo no suelta nada
            return rng.NextDouble() <= Math.Clamp(finalChance, 0f, 1.0f);
        }

        // GetBaseChanceByDifficulty
        private static float GetBaseChanceByDifficulty(Difficulty difficulty)
        {
            return difficulty switch
            {
                Difficulty.Easy => 0.15f,
                Difficulty.Normal => 0.30f,
                Difficulty.Hard => 0.60f,
                _ => 0.10f
            };
        }

        /// <summary>
        /// PASO 2: Filtra los ítems válidos para la entidad y realiza la ruleta por SpawnWeight.
        /// </summary>
        private ItemDefinition? SelectItemFromPool(ThingDefinition entityDef, Random rng)
        {
            var table = new ChanceTable();

            for (int i = 0; i < GameData.Items.Count; i++)
            {
                var itemDef = GameData.Items[i];

                if (!CanSpawnInPool(itemDef, entityDef))
                    continue;

                // Agrega el ítem a la ruleta ponderada usando su SpawnWeight directo
                table.Add(itemDef.Name, itemDef.SpawnWeight, 1, itemDef);
            }

            // Nota: No se agrega 'ChanceTable.Nothing'. 
            // Si el flujo llegó aquí, se garantiza la selección de 1 ítem.
            return table.GetValue(rng)?.Context as ItemDefinition;
        }

        /// <summary>
        /// Valida si un ítem puede formar parte del pool de recompensa de la entidad.
        /// </summary>
        private bool CanSpawnInPool(ItemDefinition itemDef, ThingDefinition entityDef)
        {
            // La Gracia (Monedas) y las acciones del jugador no pasan por esta tabla de ítems
            if (itemDef.Name == nameof(Coin) || itemDef.Behavior == ItemBehavior.PlayerAction)
                return false;

            // Ítems sin peso configurado no spawnean
            if (itemDef.SpawnWeight <= 0)
                return false;

            // Filtrar por Dominio preferido del enemigo (si está definido)
            if (entityDef.PreferredLootRealm.HasValue && itemDef.Realm != entityDef.PreferredLootRealm)
                return false;

            // Filtrar por Categoría preferida del enemigo (si está definida)
            if (entityDef.PreferredLootCategory.HasValue && itemDef.Category != entityDef.PreferredLootCategory.Value)
                return false;

            // Evitar duplicados para ítems no apilables que el jugador ya posee en el inventario
            if (!itemDef.IsStackable && session.PlayerData.Inventory.Find(itemDef.Name) == null)
                return false;

            return true;
        }

        #endregion

        /// <summary>
        /// Determina el ítem dropeado por una entidad al morir.
        /// Retorna 1 ítem si la tirada es exitosa, o null si la entidad no suelta nada.
        /// </summary>
        public ItemDefinition? RollForLoot(GameThing thing)
        {
            var def = thing.Definition;
            if (def == null || def.DropMode == LootDropMode.None)
                return null;

            // 1. Drops fijos o personalizados (Jefes, Llaves o Key Items específicos)
            if (def.DropMode == LootDropMode.Custom && !string.IsNullOrEmpty(thing.CustomDropName))
                return GameData.Items.Find(thing.CustomDropName);

            var rng = session.CurrentRun?.VolatileRng ?? session.MasterRunRng;

            // 2. PASO 1: Tirada de probabilidad de drop con Stat de Suerte (Elden Ring Item Discovery)
            if (!CheckDropSuccess(def, rng))
                return null;

            // 3. PASO 2: Selección ponderada por SpawnWeight (Garantiza entregar 1 ítem del pool)
            return SelectItemFromPool(def, rng);
        }
    }
}