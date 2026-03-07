using Engendro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ScaryCastle.Procedural
{
    /// <summary>
    /// RunManager
    /// </summary>
    public static class RunManager
    {
        private static readonly List<RoomGraph> roomGraphs = [];

        #region Private members

        // ApplyDefinitions
        private static void ApplyDefinitions(List<RoomDefinition> definitions, int maxDistance)
        {
            float threshold = maxDistance / 3f;
            var candidates = new List<RoomDefinition>();

            // Rooms
            foreach (var room in roomGraphs)
            {
                // Detectamos si es el punto de partida real
                bool isStartPoint = room.DistanceFromStart == 0;

                // Determinamos la fase según la distancia del room
                var targetDiff = Difficulty.Easy;
                if (room.DistanceFromStart >= threshold * 2)
                    targetDiff = Difficulty.Hard;
                else if (room.DistanceFromStart >= threshold)
                    targetDiff = Difficulty.Normal;

                candidates.Clear();
                foreach (var definition in definitions)
                {
                    // 1. FILTRO DE INICIO: Regla crítica
                    // Si es el inicio, solo queremos starters. Si NO es el inicio, NO queremos starters en medio del dungeon.
                    if (definition.IsStartingRoom != isStartPoint)
                        continue;

                    // 2. DIFICULTAD (Básico)
                    if (definition.Difficulty != targetDiff)
                        continue;

                    // 3. REGLA DE INSTANCIAS (Tu lógica de MaxPerRun)
                    if (!definition.PassesMaxPerRunConstraint())
                        continue;

                    // 4. REGLA DE DISEÑO LÓGICO (Callejones sin salida)
                    // Impide que un asset diseñado para ser final de camino se use como conector.
                    if (definition.RequiresDeadEnd && room.GetConnectionCount() > 1)
                        continue;

                    candidates.Add(definition);
                }

                // FALLBACK: Si no hay definitions específicas para esa fase, buscamos una inferior
                if (candidates.Count == 0 && targetDiff > Difficulty.Easy)
                {
                    foreach (var definition in definitions)
                    {
                        if (definition.Difficulty < targetDiff)
                        {
                            if (definition.RequiresDeadEnd && room.GetConnectionCount() > 1)
                                continue;

                            if (definition.PassesMaxPerRunConstraint())
                                candidates.Add(definition);
                        }
                    }
                }

                // Pick
                var chanceTable = new ChanceTable();
                foreach (var candidate in candidates)
                {
                    chanceTable.Add(candidate.Name, candidate.SpawnWeight, 1, candidate);
                }

                if (chanceTable.GetValue()?.Context is RoomDefinition chosenDefinition)
                {
                    SpawnCounter.Increment(chosenDefinition.Name);
                    room.Definition = chosenDefinition;
                }
                else
                {
                    throw new InvalidOperationException("Failed to apply room definition. No match found.");
                }
            }
        }

        // GetAvailableDefinitions
        private static List<RoomDefinition> GetAvailableDefinitions(GameSession session, Tags pools)
        {
            var result = new List<RoomDefinition>();

            foreach (var definition in RoomDefinition.Definitions.All)
            {
                // Run constraints
                if (!definition.PassesRunConstraints(session))
                    continue;

                // Pools
                if (pools.Count > 0)
                {
                    if (!Utils.Intersects(pools, definition.Pools))
                        continue;
                }

                // Passed all checks
                result.Add(definition);
            }

            return result;
        }

        // GetRoomCount
        private static int GetRoomCount(int runCount)
        {
            const int MAX_RUNS = 666;
            const int MIN_ROOMS = 5;
            const int MAX_ROOMS = 60;
            const float CURVE = 1.5f; // Controla qué tan rápido crece el mapa

            if (runCount > MAX_RUNS)
                runCount = MAX_RUNS;

            // f entre 0.0 y 1.0
            float f = (float)(runCount - 1) / (MAX_RUNS - 1);

            // Aplicar la potencia para crecimiento tardío
            float curvedProgress = (float)Math.Pow(f, CURVE);

            // Interpolación lineal
            int count = (int)Math.Round(MIN_ROOMS + ((MAX_ROOMS - MIN_ROOMS) * curvedProgress));

            return count;
        }

        #endregion

        // Clear
        public static void Clear()
        {
            foreach (var room in roomGraphs)
            {
                room.RideRoom.Children.Clear();
            }

            roomGraphs.Clear();
            SpawnCounter.Reset();
            HasContent = false;
        }

        // Generate
        public static void Generate(GameSession session, Tags pools, int floorIndex)
        {
            HasContent = true;

            roomGraphs.Clear();
            var result = RunGraphGenerator.Generate(session.Random, GetRoomCount(floorIndex));
            roomGraphs.AddRange(result.Item1);

            // Get available room definitions
            var definitions = GetAvailableDefinitions(session, pools);

            // Assign definitions
            ApplyDefinitions(definitions, result.Item2);

            // Create ride rooms
            foreach (var roomGraph in roomGraphs)
            {
                roomGraph.RideRoom = RideRoom.CreateInstance(session, roomGraph);
            }

            // Load rooms
            foreach (var room in roomGraphs)
            {
                room.RideRoom.Load();
            }
        }

        // HasContent
        public static bool HasContent { get; private set; }

        // Rooms
        public static ReadOnlyCollection<RoomGraph> Rooms { get; } = roomGraphs.AsReadOnly();

        // SpawnCounter
        public static MultiCounter SpawnCounter { get; } = new();
    }
}
