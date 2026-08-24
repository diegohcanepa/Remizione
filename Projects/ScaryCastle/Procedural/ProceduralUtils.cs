using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ScaryCastle.Procedural
{
    /// <summary>
    /// ProceduralUtils
    /// </summary>
    internal static class ProceduralUtils
    {
        // AdjustWeight
        internal static float AdjustWeight(Difficulty roomDiff, Difficulty difficulty, float baseWeight, ActorRank? rank)
        {
            int distance = (int)roomDiff - (int)difficulty;

            // 1. Modificador por choque de dificultades (Topografía vs Definición)
            float roomMultiplier = distance switch
            {
                2 => 0.05f,   // Sala Hard, Enemigo Easy -> Desalentamos masillas en el clímax
                1 => 0.25f,   // Sala Normal, Enemigo Easy
                0 => 1.0f,    // Calce ideal
                -1 => 0.10f,  // Out of Depth leve: Sala Easy, Enemigo Normal -> 10% de chances base
                -2 => 0.02f,  // Out of Depth severo: Sala Easy, Enemigo Hard -> 2% de chances base
                _ => 1.0f
            };

            // 2. Modificador por jerarquía de combate (El filtro salvaje)
            float rankMultiplier = 1.0f;
            if (rank.HasValue)
            {
                rankMultiplier = rank.Value switch
                {
                    // El Boss real tiene peso plano porque ya está blindado por su filtro de sala dedicado
                    ActorRank.Boss => 1.0f,

                    // Si es un MiniBoss y está queriendo irrumpir en una zona que no es Hard, 
                    // le pegamos un hachazo drástico a su peso para que sea una rareza absoluta.
                    ActorRank.MiniBoss => roomDiff switch
                    {
                        Difficulty.Easy => 0.10f,   // Hachazo del 90%. Combinado con el -2 de arriba, da un 0.002% real. Épico si sale.
                        Difficulty.Normal => 0.30f, // Hachazo del 70%. Aparece a mitad de camino de forma muy esporádica.
                        Difficulty.Hard => 1.0f,   // Peso completo: es su hábitat natural.
                        _ => 1.0f
                    },

                    _ => 1.0f
                };
            }

            return baseWeight * roomMultiplier * rankMultiplier;
        }

        // CalculateEnemyBudget
        internal static int CalculateEnemyBudget(RoomNode roomNode, Random rng)
        {
            // 1. Definimos el presupuesto estrictamente por la zona geográfica
            var (min, max) = roomNode.TopographicDifficulty switch
            {
                Difficulty.Easy => (1, 1),   // Muy tranquilo
                Difficulty.Normal => (2, 2), // Reto estándar
                Difficulty.Hard => (2, 3),   // Presión alta
                _ => (0, 0)
            };

            // 2. Variación aleatoria (-1, 0, +1) para inyectar imprevisibilidad
            int finalBudget = rng.Next(min - 1, max + 2);

            // 3. Clamp final para garantizar un límite mínimo y máximo absoluto en el cuarto
            return Math.Clamp(finalBudget, 1, 4); // Nunca 0, nunca más de 4 patrullas/patotas base
        }

        // GetSpawnPoints
        internal static List<Vector2> GetSpawnPoints(Polygon polygon, int count, int cellSize, Random rng)
        {
            var cells = new List<Vector2>();
            var area = polygon.BoundingRectangle;

            for (int y = area.Top; y < area.Bottom; y += cellSize)
            {
                for (int x = area.Left; x < area.Right; x += cellSize)
                {
                    float cx = x + (cellSize * 0.5f);
                    float cy = y + (cellSize * 0.5f);

                    float offsetRange = cellSize * 0.25f;
                    cx += (float)((rng.NextDouble() * offsetRange * 2) - offsetRange);
                    cy += (float)((rng.NextDouble() * offsetRange * 2) - offsetRange);

                    var candidate = new Vector2(cx, cy);

                    if (polygon.Contains(candidate))
                    {
                        cells.Add(candidate);
                    }
                }
            }

            for (int i = cells.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (cells[i], cells[j]) = (cells[j], cells[i]);
            }

            if (cells.Count > count)
                cells.RemoveRange(count, cells.Count - count);

            return cells;
        }
    }
}
