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

        // CheckDropSuccess
        private static bool CheckDropSuccess(ThingDefinition def, Random rng)
        {
            float baseChance = def.BaseDropChance;
            if (baseChance <= 0)
                return false;

            // Stat de Suerte del Jugador (Discovery Multiplier)
            float playerLuck = 0; // TODO: session.PlayerTraits?.GetTotalTraitValue(TraitType.Luck) ?? 0f;
            float finalChance = baseChance * (1f + (playerLuck / 100f));

            // Tirada del dado contra la probabilidad final
            return rng.NextDouble() <= Math.Clamp(finalChance, 0f, 1f);
        }

        #endregion


        // RollForLoot
        public ItemDefinition? RollForLoot(ThingDefinition def)
        {
            var rng = session.CurrentRun?.VolatileRng ?? session.MasterRunRng;

            // 1. PASO 1: Suelta algo?
            if (!CheckDropSuccess(def, rng))
                return null;

            // 2. PASO 2: Selección Ponderada desde la tabla exclusiva de la entidad
            if (def.LootPool.GetItem(rng)?.Context is not ItemDefinition itemDefinition)
                return null;

            // 3. Validación final: Evitar dar duplicados si es un ítem único (no apilable)
            if (!itemDefinition.IsStackable && session.PlayerData.Inventory.Find(itemDefinition.Name) != null)
                return null;

            return itemDefinition;
        }
    }
}