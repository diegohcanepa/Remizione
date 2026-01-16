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
        private static readonly List<RoomGraph> rooms = [];

        #region Private members

        // ApplyDefinitions
        private static void ApplyDefinitions(List<RoomDefinition> definitions, int maxDistance)
        {
            float threshold = maxDistance / 3f;

            var candidates = new List<RoomDefinition>();

            // Rooms
            foreach (var room in rooms)
            {
                // Determinamos la fase según la distancia del room
                var targetDiff = Difficulty.Easy;
                if (room.DistanceFromStart >= threshold * 2)
                    targetDiff = Difficulty.Hard;
                else if (room.DistanceFromStart >= threshold)
                    targetDiff = Difficulty.Normal;

                candidates.Clear();
                foreach (var definition in definitions)
                {
                    // Match room type?
                    if (definition.RoomType != room.RoomType)
                        continue;

                    // Match difficulty
                    if (definition.Difficulty != targetDiff)
                        continue;

                    if (definition.RequiresDeadEnd && room.GetConnectionCount() > 1)
                        continue;

                    if (!definition.PassesMaxPerRunConstraint())
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
                    throw new InvalidOperationException("Failed to apply room definition. No match found.");
            }
        }

        // GetAvailableDefinitions
        private static List<RoomDefinition> GetAvailableDefinitions(GameSession session, Tags pools)
        {
            var result = new List<RoomDefinition>();

            foreach (var definition in RoomDefinition.All)
            {
                // Run constraints
                if (!definition.PassesFloorConstraints(session))
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

        #endregion

        // Clear
        public static void Clear()
        {
            foreach (var room in rooms)
            {
                room.RideRoom?.Children.Clear();
            }

            rooms.Clear();
            SpawnCounter.Reset();
            HasContent = false;
        }

        // Generate
        public static void Generate(GameSession session, Tags pools, int roomCount)
        {
            HasContent = true;

            rooms.Clear();
            var result = RunGraphGenerator.Generate(session.Random, roomCount);
            rooms.AddRange(result.Item1);

            // Get available room definitions
            var definitions = GetAvailableDefinitions(session, pools);

            // Assign definitions
            ApplyDefinitions(definitions, result.Item2);

            // Create ride rooms
            foreach (var room in rooms)
            {
                room.RideRoom = RideRoom.CreateInstance(session, room);
            }

            // Load rooms
            foreach (var room in rooms)
            {
                if (room.Definition == null)
                    throw new InvalidOperationException($"Room [{room}] has no definition.");

                if (room.RideRoom == null)
                    throw new InvalidOperationException($"Room [{room}] has no procedural room.");

                room.RideRoom?.Load();
            }
        }

        // HasContent
        public static bool HasContent { get; private set; }

        // Rooms
        public static ReadOnlyCollection<RoomGraph> Rooms { get; } = rooms.AsReadOnly();

        // SpawnCounter
        public static MultiCounter SpawnCounter { get; } = new();
    }
}
