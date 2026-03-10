using Engendro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ScaryCastle
{
    /// <summary>
    /// RunManager
    /// </summary>
    public static class RunManager
    {
        #region Private fields

        // roomGraphs
        // Internal collection of generated room nodes.
        private static readonly List<RoomGraph> roomGraphs = [];

        #endregion

        #region Private members

        // ApplyDefinitions
        // Logic for assigning room definitions to nodes based on type and constraints.
        private static bool ApplyDefinitions(List<RoomDefinition> definitions, int maxDistance, Random rng, bool strict)
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
                    if (d.IsStartingRoom && startNode.Fits(d))
                    {
                        startDef = d;
                        break;
                    }
                }

                if (startDef == null && strict)
                {
                    return false;
                }

                if (startDef != null)
                {
                    Assign(startNode, startDef, availableNodes);
                }
            }

            // 2. EXIT ROOM
            RoomGraph? exitNode = null;
            foreach (var n in availableNodes)
            {
                if (n.RoomType == RoomType.Exit)
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
                    if (d.IsExit && exitNode.Fits(d))
                    {
                        exitDef = d;
                        break;
                    }
                }

                if (exitDef == null && strict)
                {
                    return false;
                }

                if (exitDef != null)
                {
                    Assign(exitNode, exitDef, availableNodes);
                }
            }

            // 3. MANDATORY ROOMS
            List<RoomDefinition> mandatory = [];
            foreach (var d in definitions)
            {
                if (d.IsMandatory && !d.IsStartingRoom && !d.IsExit)
                {
                    mandatory.Add(d);
                }
            }

            foreach (var def in mandatory)
            {
                List<RoomGraph> validNodes = [];
                foreach (var n in availableNodes)
                {
                    if (n.Fits(def) && (!def.RequiresDeadEnd || n.ConnectionCount == 1))
                    {
                        validNodes.Add(n);
                    }
                }

                if (validNodes.Count > 0)
                {
                    Assign(validNodes[rng.Next(validNodes.Count)], def, availableNodes);
                }
                else if (strict)
                {
                    return false;
                }
            }

            // 4. FILLER ROOMS (FLUFF)
            List<RoomDefinition> fluff = [];
            foreach (var d in definitions)
            {
                if (!d.IsMandatory && !d.IsStartingRoom && !d.IsExit)
                {
                    fluff.Add(d);
                }
            }

            List<RoomGraph> nodesToFill = [.. availableNodes];
            foreach (var node in nodesToFill)
            {
                Difficulty targetDiff = GetDifficulty(node.DistanceFromStart, threshold);
                List<RoomDefinition> candidates = [];

                foreach (var d in fluff)
                {
                    if (d.Difficulty == targetDiff && node.Fits(d) && d.PassesMaxPerRunConstraint())
                    {
                        candidates.Add(d);
                    }
                }

                if (candidates.Count == 0)
                {
                    foreach (var d in fluff)
                    {
                        if (node.Fits(d) && d.PassesMaxPerRunConstraint())
                        {
                            candidates.Add(d);
                        }
                    }
                }

                if (candidates.Count > 0)
                {
                    Assign(node, candidates[rng.Next(candidates.Count)], availableNodes);
                }
                else if (strict)
                {
                    return false;
                }
            }

            return true;
        }

        // Assign
        // Links a definition to a node and removes it from the pool.
        private static void Assign(RoomGraph node, RoomDefinition def, List<RoomGraph> pool)
        {
            node.Definition = def;
            SpawnCounter.Increment(def.Name);
            pool.Remove(node);
        }

        // ClearInternal
        // Resets the internal state and the spawn counter.
        private static void ClearInternal()
        {
            roomGraphs.Clear();
            SpawnCounter.Reset();
        }

        // GenerateValidTopology
        // Loops until a mathematically valid map skeleton is generated.
        private static (List<RoomGraph>, int) GenerateValidTopology(Random rng, int count)
        {
            int safetyNet = 0;
            while (safetyNet < 1000)
            {
                var res = RunGraphGenerator.Generate(rng, count);
                if (res.Item2 != -1)
                {
                    return res;
                }
                safetyNet++;
            }
            throw new InvalidOperationException("Critical Error: Unable to generate topology with vertical Exit.");
        }

        // GetAvailableDefinitions
        // Filters all definitions based on session constraints and pool intersection.
        private static List<RoomDefinition> GetAvailableDefinitions(GameSession session, Tags pools)
        {
            List<RoomDefinition> result = [];
            foreach (var definition in RoomDefinition.Definitions.All)
            {
                if (!definition.PassesRunConstraints(session))
                {
                    continue;
                }

                if (pools.Count > 0)
                {
                    if (!Utils.Intersects(pools, definition.Pools))
                    {
                        continue;
                    }
                }

                result.Add(definition);
            }
            return result;
        }

        // GetDifficulty
        // Determines difficulty based on normalized distance from start.
        private static Difficulty GetDifficulty(int dist, float threshold)
        {
            if (dist >= threshold * 2)
            {
                return Difficulty.Hard;
            }

            if (dist >= threshold)
            {
                return Difficulty.Normal;
            }

            return Difficulty.Easy;
        }

        // GetRunLength
        // Calculates room count using a curve based on the current episode/chapter.
        private static int GetRunLength(int chapter)
        {
            const int MIN_ROOMS = 12;
            const int MAX_ROOMS = 30;
            const int MAX_EPISODES = 50;
            const float CURVE = 1.2f;

            float f = Math.Clamp((float)(chapter - 1) / (MAX_EPISODES - 1), 0f, 1f);
            float curvedProgress = (float)Math.Pow(f, CURVE);

            return (int)Math.Round(MIN_ROOMS + ((MAX_ROOMS - MIN_ROOMS) * curvedProgress));
        }

        #endregion

        // Clear
        // Cleans up the current run including the instantiated RideRooms.
        public static void Clear()
        {
            foreach (var r in roomGraphs)
            {
                if (r.RideRoom != null)
                {
                    r.RideRoom.Children.Clear();
                }
            }

            ClearInternal();
            HasContent = false;
        }

        // Generate
        // Main entry point for the procedural generation process.
        public static void Generate(GameSession session, Tags pools)
        {
            HasContent = false;
            List<RoomDefinition> defs = GetAvailableDefinitions(session, pools);
            int count = GetRunLength(session.Chapter);
            bool success = false;

            // Try 15 strict assignment attempts with guaranteed skeletons
            for (int i = 0; i < 15; i++)
            {
                ClearInternal();
                var res = GenerateValidTopology(session.Random, count);
                roomGraphs.AddRange(res.Item1);

                if (ApplyDefinitions(defs, res.Item2, session.Random, true))
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
                ApplyDefinitions(defs, res.Item2, session.Random, false);
            }

            // Instance and Load rooms
            foreach (var r in roomGraphs)
            {
                r.RideRoom = RideRoom.CreateInstance(session, r);
            }

            foreach (var r in roomGraphs)
            {
                r.RideRoom.Load();
            }

            HasContent = true;
        }

        // HasContent
        public static bool HasContent { get; private set; }

        // Rooms
        public static ReadOnlyCollection<RoomGraph> Rooms => roomGraphs.AsReadOnly();

        // SpawnCounter
        public static MultiCounter SpawnCounter { get; } = new();
    }
}