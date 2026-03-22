using Engendro;
using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// FloorBuilder
    /// </summary>
    public sealed class FloorBuilder
    {
        private bool built;
        private readonly List<RoomGraph> roomGraphs = [];

        #region Private Static members

        // GetAvailableRoomDefinitions
        private static List<RoomDefinition> GetAvailableRoomDefinitions(GameSession session, MultiCounter floorSpawns, MultiCounter globalSpawns, Tags pools)
        {
            List<RoomDefinition> result = [];

            foreach (var definition in RoomDefinition.Definitions.All)
            {
                // 1. ¿Está disponible?
                if (!definition.PassesRunConstraints(session.RunCount))
                    continue;

                // 2. ¿Salió muchas veces en esta partida?
                if (!definition.PassesMaxPerRunConstraint(floorSpawns, globalSpawns))
                    continue;

                // 3. ¿Pertenece al bioma/pool actual?
                // EXCEPCIÓN: Start y Exit ignoran el filtro de pools. 
                // Esto asegura que siempre haya una entrada y salida, incluso si 
                // olvidaste tagearlas para un bioma específico.
                if (definition.RoomType != RoomType.Start && definition.RoomType != RoomType.LeftExit && definition.RoomType != RoomType.RightExit)
                    if (pools.Count > 0 && !Utils.Intersects(pools, definition.Pools))
                        continue;

                result.Add(definition);
            }

            return result;
        }

        // GetFloorLength
        // Calcula la cantidad de salas basándose en el bioma (chapter) y el progreso interno (floorIndex).
        private static int GetFloorLength(int chapter, int floorIndex)
        {
            // 1. Definimos la base según el capítulo
            const int MIN_ROOMS_START = 7;
            const int MAX_ROOMS_BASE = 15; // Base máxima para el capítulo 50
            const int MAX_CHAPTERS = 50;
            const float CURVE = 1.2f;

            float f = Math.Clamp((float)(chapter - 1) / (MAX_CHAPTERS - 1), 0f, 1f);
            float curvedProgress = (float)Math.Pow(f, CURVE);

            // Tamaño inicial para el Piso 1 de este capítulo
            int baseSize = (int)Math.Round(MIN_ROOMS_START + ((MAX_ROOMS_BASE - MIN_ROOMS_START) * curvedProgress));

            // 2. Sumamos el incremento por piso dentro de la run actual
            // Cada piso suma 1 sala adicional (puedes ajustar este factor)
            int floorBonus = (floorIndex - 1) * 1;

            // 3. Resultado final con un tope absoluto para no romper el generador
            return Math.Min(baseSize + floorBonus, 20);
        }

        #endregion

        #region Private members

        // ApplyDefinitions
        // Logic for assigning room definitions to nodes based on type and constraints.
        private bool ApplyDefinitions(List<RoomDefinition> definitions, int maxDistance, Random random, bool strict, MultiCounter globalSpawns)
        {
            float threshold = maxDistance / 3f;
            List<RoomGraph> availableNodes = [.. roomGraphs];

            // 1. STARTING ROOM
            RoomGraph? startNode = null;
            foreach (var n in availableNodes)
            {
                if (n.DistanceFromStart == 0)
                {
                    startNode = n;
                    break;
                }
            }

            if (startNode != null)
            {
                RoomDefinition? startDef = null;
                foreach (var d in definitions)
                {
                    if (d.RoomType == RoomType.Start && startNode.Fits(d))
                    {
                        startDef = d;
                        break;
                    }
                }

                if (startDef == null && strict)
                    return false;

                if (startDef != null)
                    Assign(startNode, startDef, availableNodes);
            }

            // 2. EXIT ROOM
            RoomGraph? exitNode = null;
            foreach (var n in availableNodes)
            {
                if (n.RoomType == RoomType.LeftExit || n.RoomType == RoomType.RightExit)
                {
                    exitNode = n;
                    break;
                }
            }

            if (exitNode != null)
            {
                RoomDefinition? exitDef = null;
                foreach (var d in definitions)
                {
                    // Validación exacta: El JSON debe ser LeftExit si el nodo es LeftExit, y viceversa.
                    if (d.RoomType == exitNode.RoomType && exitNode.Fits(d))
                    {
                        exitDef = d;
                        break;
                    }
                }

                if (exitDef == null && strict)
                    return false;

                if (exitDef != null)
                    Assign(exitNode, exitDef, availableNodes);
            }

            // 3. MANDATORY ROOMS
            List<RoomDefinition> mandatory = [];
            foreach (var d in definitions)
            {
                if (d.IsMandatory && d.RoomType != RoomType.Start && d.RoomType != RoomType.LeftExit && d.RoomType != RoomType.RightExit)
                    mandatory.Add(d);
            }

            foreach (var def in mandatory)
            {
                List<RoomGraph> validNodes = [];
                foreach (var n in availableNodes)
                {
                    if (n.Fits(def) && (!def.RequiresDeadEnd || n.ConnectionCount == 1))
                        validNodes.Add(n);
                }

                if (validNodes.Count > 0)
                    Assign(validNodes[random.Next(validNodes.Count)], def, availableNodes);
                else if (strict)
                    return false;
            }

            // 4. FILLER ROOMS (FLUFF)
            List<RoomDefinition> fluff = [];
            foreach (var d in definitions)
            {
                if (!d.IsMandatory && d.RoomType != RoomType.Start && d.RoomType != RoomType.LeftExit && d.RoomType != RoomType.RightExit)
                    fluff.Add(d);
            }

            List<RoomGraph> nodesToFill = [.. availableNodes];
            foreach (var node in nodesToFill)
            {
                Difficulty targetDiff = GetDifficulty(node.DistanceFromStart, threshold);
                List<RoomDefinition> candidates = [];

                foreach (var d in fluff)
                {
                    if (d.Difficulty == targetDiff && node.Fits(d) && d.PassesMaxPerRunConstraint(Spawns, globalSpawns))
                        candidates.Add(d);
                }

                if (candidates.Count == 0)
                {
                    foreach (var d in fluff)
                    {
                        if (node.Fits(d) && d.PassesMaxPerRunConstraint(Spawns, globalSpawns))
                            candidates.Add(d);
                    }
                }

                if (candidates.Count > 0)
                    Assign(node, candidates[random.Next(candidates.Count)], availableNodes);
                else if (strict)
                    return false;
            }

            return true;
        }

        // Assign
        // Links a definition to a node and removes it from the pool.
        private void Assign(RoomGraph node, RoomDefinition def, List<RoomGraph> pool)
        {
            node.Definition = def;
            Spawns.Increment(def.Name);
            pool.Remove(node);
        }

        // ClearInternal
        // Resets the internal state and the spawn counter.
        private void ClearInternal()
        {
            roomGraphs.Clear();
            Spawns.Reset();
        }

        // GenerateValidTopology
        // Loops until a mathematically valid map skeleton is generated.
        private (List<RoomGraph>, int) GenerateValidTopology(Random rng, int count)
        {
            int safetyNet = 0;
            while (safetyNet < 1000)
            {
                var res = RunGraphGenerator.Generate(rng, count);
                if (res.Item2 != -1)
                    return res;

                safetyNet++;
            }
            throw new InvalidOperationException("Critical Error: Unable to generate topology with directional Exit.");
        }

        // GetDifficulty
        // Determines difficulty based on normalized distance from start.
        private Difficulty GetDifficulty(int dist, float threshold)
        {
            if (dist >= threshold * 2)
                return Difficulty.Hard;

            if (dist >= threshold)
                return Difficulty.Normal;

            return Difficulty.Easy;
        }

        #endregion

        // Build
        public Floor Build(GameSession session, int floorIndex, Tags pools, MultiCounter globalSpawns)
        {
            if (built)
                throw new InvalidOperationException("Floor is already built.");

            built = true;

            // 1. Collect available room definitions for the floor
            List<RoomDefinition> defs = GetAvailableRoomDefinitions(session, Spawns, globalSpawns, pools);

            // 2. Calculate run size
            int count = GetFloorLength(session.Chapter, floorIndex);

            // 3. Try 15 strict assignment attempts with guaranteed skeletons
            bool success = false;
            for (int i = 0; i < 15; i++)
            {
                ClearInternal();
                var res = GenerateValidTopology(session.Random, count);
                roomGraphs.AddRange(res.Item1);

                if (ApplyDefinitions(defs, res.Item2, session.Random, true, globalSpawns))
                {
                    success = true;
                    break;
                }
            }

            // Fallback for tricky topologies
            if (!success)
            {
                ClearInternal();
                var res = GenerateValidTopology(session.Random, count);
                roomGraphs.AddRange(res.Item1);
                ApplyDefinitions(defs, res.Item2, session.Random, false, globalSpawns);
            }

            return new Floor(roomGraphs, floorIndex);
        }

        // Spawns
        public MultiCounter Spawns { get; } = new();
    }
}