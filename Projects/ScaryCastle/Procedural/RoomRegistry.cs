using Engendro;
using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// RoomRegistry
    /// </summary>
    public sealed class RoomRegistry
    {
        private readonly Dictionary<RoomCategory, List<RoomDefinition>> buckets = [];

        #region Constructor

        // Constructor
        public RoomRegistry()
        {
            // 1. Inicialización dinámica de todas las categorías
            foreach (var category in Enum.GetValues<RoomCategory>())
            {
                this.buckets[category] = [];
            }

            // 2. Clasificación de definiciones cargadas
            var all = GameData.Rooms.All;
            for (int i = 0; i < all.Count; i++)
            {
                var def = all[i];

                if (this.buckets.TryGetValue(def.RoomCategory, out var bucket))
                {
                    bucket.Add(def);
                }
            }
        }

        #endregion

        #region Private members

        // CalculateWeight
        private static int CalculateWeight(Difficulty roomDiff, Difficulty runDiff)
        {
            return (runDiff, roomDiff) switch
            {
                // RUN EN EASY
                (Difficulty.Easy, Difficulty.Easy) => 100, // Preferencia total
                (Difficulty.Easy, Difficulty.Normal) => 10,  // Raro
                (Difficulty.Easy, Difficulty.Hard) => 1,   // Casi imposible pero posible

                // RUN EN MEDIUM
                (Difficulty.Normal, Difficulty.Easy) => 30,  // Respiro
                (Difficulty.Normal, Difficulty.Normal) => 100, // Estándar
                (Difficulty.Normal, Difficulty.Hard) => 20,  // Pico de dificultad

                // RUN EN HARD
                (Difficulty.Hard, Difficulty.Easy) => 10,  // Muy raro (respiro grande)
                (Difficulty.Hard, Difficulty.Normal) => 40,  // Calentamiento
                (Difficulty.Hard, Difficulty.Hard) => 100, // Estándar final

                _ => 0
            };
        }

        #endregion

        // GetValidDefinition
        public RoomDefinition? GetValidDefinition(RoomNode node, Difficulty runDiff, CounterBank counters, Random rng)
        {
            // Seleccionar el pool usando estrictamente la categoría asignada en la Fase 2 (Etiquetado)
            List<RoomDefinition> pool = this.buckets.GetValueOrDefault(node.Category) ?? [];

            if (pool.Count == 0)
                return null;

            // Preparar pesos (AOT Friendly: stackalloc previene activaciones del Garbage Collector)
            Span<int> weights = stackalloc int[pool.Count];
            int totalWeight = 0;

            for (int i = 0; i < pool.Count; i++)
            {
                var def = pool[i];

                // Filtros de exclusión (Hard Constraints)
                // node.Fits() ahora se encarga de la topología estricta (Puertas y ExactMatch)
                if (!node.Fits(def) || !def.PassesMaxPerRunConstraint(counters.GetCount(def.Name)))
                {
                    weights[i] = 0;
                    continue;
                }

                // Cálculo de peso por probabilidad (Soft Constraints)
                int w = CalculateWeight(def.Difficulty, runDiff);
                weights[i] = w;
                totalWeight += w;
            }

            if (totalWeight == 0)
                return null;

            // Selección por ruleta (Weighted Random)
            int roll = rng.Next(0, totalWeight);
            int cursor = 0;

            for (int i = 0; i < pool.Count; i++)
            {
                cursor += weights[i];
                if (roll < cursor)
                    return pool[i];
            }

            return null;
        }
    }
}