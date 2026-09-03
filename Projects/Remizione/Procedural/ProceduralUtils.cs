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

        // GetCandidateDefinitions
        internal static List<TDefinition> GetCandidateDefinitions<TDefinition>(Run run, RoomNode roomNode, IList<TDefinition> definitions)
            where TDefinition : ThingDefinition
        {
            var outList = new List<TDefinition>();

            foreach (var definition in definitions)
            {
                if (definition.SpawnWeight == 0)
                    continue;

                if (definition.MinFloor > run.FloorIndex)
                    continue;

                // Si exige sala específica y no coincide, afuera.
                if (definition.TargetRoomCategory.HasValue && definition.TargetRoomCategory != roomNode.Category)
                    continue;

                // 2. Si es de ámbito StandardOnly y estamos en una sala restringida, afuera.
                if (definition.SpawnScope == SpawnScope.StandardOnly && roomNode.Definition.IsRestricted)
                    continue;

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

        // IsValidActorForRoom
        internal static bool IsValidActorForRoom(Difficulty roomDiff, Difficulty actorDiff)
        {
            return roomDiff switch
            {
                Difficulty.Easy => actorDiff == Difficulty.Easy,
                Difficulty.Normal => actorDiff is Difficulty.Normal or Difficulty.Easy,
                Difficulty.Hard => true,
                _ => false
            };
        }
    }
}