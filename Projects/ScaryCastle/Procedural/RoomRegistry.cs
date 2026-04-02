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
        private readonly List<RoomDefinition> corridors = [];
        private readonly Dictionary<SideRoomCategory, List<RoomDefinition>> sideRooms = [];

        #region Constructor

        // Constructor
        public RoomRegistry()
        {
            // 1. Inicialización dinámica de categorías
            foreach (var category in Enum.GetValues<SideRoomCategory>())
            {
                if (category != SideRoomCategory.None)
                    this.sideRooms[category] = [];
            }

            // 2. Clasificación de definiciones cargadas
            var all = RoomDefinition.Definitions.All;
            for (int i = 0; i < all.Count; i++)
            {
                var def = all[i];

                if (def.RoomType == RoomType.Corridor)
                {
                    this.corridors.Add(def);
                }
                else if (def.RoomType == RoomType.SideRoom)
                {
                    if (this.sideRooms.TryGetValue(def.SideRoomCategory, out var bucket))
                    {
                        bucket.Add(def);
                    }
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
            // Seleccionar el pool de búsqueda
            List<RoomDefinition> pool = node.RoomType == RoomType.Corridor
                ? this.corridors
                : this.sideRooms.GetValueOrDefault(node.SideRoomCategory) ?? [];

            if (pool.Count == 0)
                return null;

            // Preparar pesos (AOT Friendly: stackalloc para evitar allocations)
            Span<int> weights = stackalloc int[pool.Count];
            int totalWeight = 0;

            for (int i = 0; i < pool.Count; i++)
            {
                var def = pool[i];

                // Filtros de exclusión (Hard Constraints)
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