using Adberration;
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
        // AdjustActorWeight
        internal static float AdjustActorWeight(Difficulty roomDiff, Difficulty difficulty, float baseWeight)
        {
            switch (roomDiff)
            {
                case Difficulty.Easy:
                    // Sala Easy: SOLO enemigos Easy.
                    if (difficulty == Difficulty.Easy) return baseWeight * 1.00f;
                    return 0.00f;

                case Difficulty.Normal:
                    // Sala Normal: Enemigos Normales (100%) y Masillas Easy (35%).
                    if (difficulty == Difficulty.Normal) return baseWeight * 1.00f;
                    if (difficulty == Difficulty.Easy) return baseWeight * 0.35f;
                    return 0.00f;

                case Difficulty.Hard:
                    // Sala Hard: Enemigos Hard (100%) y Normales de soporte (50%).
                    if (difficulty == Difficulty.Hard) return baseWeight * 1.00f;
                    if (difficulty == Difficulty.Normal) return baseWeight * 0.50f;
                    return 0.00f;

                default:
                    return 0.00f;
            }
        }

        // AdjustPropWeight
        internal static float AdjustPropWeight(Difficulty roomDiff, Difficulty difficulty, float baseWeight)
        {
            // Para los Props, la dificultad de la sala actúa como un TOPE.
            // Una Antorcha (Easy) puede estar en cualquier sala. 
            // Un Cofre (Hard) NO puede estar en una sala Easy.
            if (difficulty > roomDiff)
                return 0.00f; // El prop es demasiado difícil para esta sala

            // Si el prop es de dificultad menor o igual a la sala, sale con su peso normal.
            return baseWeight;
        }

        // CalculateEnemyBudget
        internal static int CalculateEnemyBudget(RoomNode roomNode, float runProgress, Random rng)
        {
            // Presupuesto base austero pero suficiente para la aventura
            int baseBudget = roomNode.TopographicDifficulty switch
            {
                Difficulty.Easy => rng.Next(2, 4),   // 2 a 3 pts (Ideal para 1-3 masillas o un pack)
                Difficulty.Normal => rng.Next(4, 7),   // 4 a 6 pts
                Difficulty.Hard => rng.Next(7, 11),  // 7 a 10 pts
                _ => 2
            };

            // Escalado de volumen por avance de piso (+0 a +3 puntos maximo hacia el final)
            int extraBudget = (int)Math.Round(runProgress * 3.0f);

            return baseBudget + extraBudget;
        }

        // GetCandidateDefinitions
        internal static List<TDefinition> GetCandidateDefinitions<TDefinition>(Run run, RoomNode roomNode, IList<TDefinition> definitions)
            where TDefinition : ThingDefinition
        {
            var outList = new List<TDefinition>();

            foreach (var definition in definitions)
            {
                if (definition.IsUnique)
                    continue;

                if (definition.MinFloor > run.FloorIndex)
                    continue;

                if (definition is ActorDefinition actorDefinition)
                {
                    // Si es un Boss real, SOLO puede aparecer en la habitación etiquetada como Boss
                    if (actorDefinition.Rank == ActorRank.Boss && roomNode.Category != RoomCategory.End)
                        continue;

                    // Y viceversa: en la sala del Boss no queremos que spawneen murciélagos comunes como plato principal
                    if (roomNode.Category == RoomCategory.End && actorDefinition.Rank != ActorRank.Boss)
                        continue;
                }

                if (definition.RoomTheme.HasValue && definition.RoomTheme != roomNode.Definition.Theme)
                    continue;

                if (run.Session.GetProceduralThing(definition.Name) == null)
                    continue;

                if (!definition.PassesRunConstraints(run.Session.RunIndex))
                    continue;

                if (!definition.PassesMaxPerRunConstraint(run.Spawns))
                    continue;

                if (!TagScope.Test(roomNode.Definition.Scope, roomNode.Definition.Pools, definition.Tags))
                    continue;

                outList.Add(definition);
            }

            return outList;
        }

        // GetSpawnPoints (Geometria inalterada)
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