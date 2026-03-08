using Engendro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ScaryCastle
{
    public static class RunManager
    {
        private static readonly List<RoomGraph> roomGraphs = [];

        #region Private members

        // Garantiza matemáticamente que el mapa tiene una salida válida
        private static (List<RoomGraph>, int) GenerateValidTopology(Random rng, int count)
        {
            int safetyNet = 0;
            while (safetyNet < 1000)
            {
                var res = RunGraphGenerator.Generate(rng, count);
                if (res.Item2 != -1) return res;
                safetyNet++;
            }
            throw new Exception("Error crítico: Imposible generar topología con Exit vertical.");
        }

        private static List<RoomDefinition> GetAvailableDefinitions(GameSession session, Tags pools)
        {
            var result = new List<RoomDefinition>();
            foreach (var definition in RoomDefinition.Definitions.All)
            {
                if (!definition.PassesRunConstraints(session)) continue;
                if (pools.Count > 0)
                {
                    if (!Utils.Intersects(pools, definition.Pools)) continue;
                }
                result.Add(definition);
            }
            return result;
        }

        private static bool ApplyDefinitions(List<RoomDefinition> definitions, int maxDistance, Random rng, bool strict)
        {
            float threshold = maxDistance / 3f;
            var availableNodes = new List<RoomGraph>(roomGraphs);

            // 1. INICIO
            RoomGraph? startNode = null;
            foreach (var n in availableNodes) if (n.DistanceFromStart == 0) { startNode = n; break; }
            if (startNode != null)
            {
                RoomDefinition? startDef = null;
                foreach (var d in definitions) if (d.IsStartingRoom && startNode.Fits(d)) { startDef = d; break; }
                if (startDef == null && strict) return false;
                if (startDef != null) Assign(startNode, startDef, availableNodes);
            }

            // 2. SALIDA (Exit)
            RoomGraph? exitNode = null;
            foreach (var n in availableNodes) if (n.RoomType == RoomType.Exit) { exitNode = n; break; }
            if (exitNode != null)
            {
                RoomDefinition? exitDef = null;
                foreach (var d in definitions) if (d.IsExit && exitNode.Fits(d)) { exitDef = d; break; }
                if (exitDef == null && strict) return false;
                if (exitDef != null) Assign(exitNode, exitDef, availableNodes);
            }

            // 3. OBLIGATORIAS
            var mandatory = new List<RoomDefinition>();
            foreach (var d in definitions) if (d.IsMandatory && !d.IsStartingRoom && !d.IsExit) mandatory.Add(d);
            foreach (var def in mandatory)
            {
                var validNodes = new List<RoomGraph>();
                foreach (var n in availableNodes)
                {
                    if (n.Fits(def) && (!def.RequiresDeadEnd || n.ConnectionCount == 1)) validNodes.Add(n);
                }
                if (validNodes.Count > 0) Assign(validNodes[rng.Next(validNodes.Count)], def, availableNodes);
                else if (strict) return false;
            }

            // 4. RELLENO
            var fluff = new List<RoomDefinition>();
            foreach (var d in definitions) if (!d.IsMandatory && !d.IsStartingRoom && !d.IsExit) fluff.Add(d);
            var nodesToFill = new List<RoomGraph>(availableNodes);
            foreach (var node in nodesToFill)
            {
                var targetDiff = GetDifficulty(node.DistanceFromStart, threshold);
                var candidates = new List<RoomDefinition>();
                foreach (var d in fluff) if (d.Difficulty == targetDiff && node.Fits(d) && d.PassesMaxPerRunConstraint()) candidates.Add(d);
                if (candidates.Count == 0) foreach (var d in fluff) if (node.Fits(d) && d.PassesMaxPerRunConstraint()) candidates.Add(d);
                if (candidates.Count > 0) Assign(node, candidates[rng.Next(candidates.Count)], availableNodes);
                else if (strict) return false;
            }
            return true;
        }

        private static void Assign(RoomGraph node, RoomDefinition def, List<RoomGraph> pool)
        {
            node.Definition = def;
            SpawnCounter.Increment(def.Name);
            pool.Remove(node);
        }

        private static Difficulty GetDifficulty(int dist, float threshold)
        {
            if (dist >= threshold * 2) return Difficulty.Hard;
            if (dist >= threshold) return Difficulty.Normal;
            return Difficulty.Easy;
        }

        private static void ClearInternal() { roomGraphs.Clear(); SpawnCounter.Reset(); }
        #endregion

        public static void Generate(GameSession session, Tags pools, int floorIndex)
        {
            HasContent = false;
            var defs = GetAvailableDefinitions(session, pools);
            int count = 10 + (floorIndex * 2);

            bool success = false;
            // 15 intentos puros de asignación (con topología ya garantizada)
            for (int i = 0; i < 15; i++)
            {
                ClearInternal();
                var res = GenerateValidTopology(session.Random, count);
                roomGraphs.AddRange(res.Item1);
                if (ApplyDefinitions(defs, res.Item2, session.Random, true)) { success = true; break; }
            }

            if (!success)
            {
                ClearInternal();
                var res = GenerateValidTopology(session.Random, count);
                roomGraphs.AddRange(res.Item1);
                ApplyDefinitions(defs, res.Item2, session.Random, false);
            }

            foreach (var r in roomGraphs) r.RideRoom = RideRoom.CreateInstance(session, r);
            foreach (var r in roomGraphs) r.RideRoom.Load();
            HasContent = true;
        }

        public static void Clear() { foreach (var r in roomGraphs) r.RideRoom?.Children.Clear(); ClearInternal(); HasContent = false; }
        public static bool HasContent { get; private set; }
        public static ReadOnlyCollection<RoomGraph> Rooms => roomGraphs.AsReadOnly();
        public static MultiCounter SpawnCounter { get; } = new();
    }
}