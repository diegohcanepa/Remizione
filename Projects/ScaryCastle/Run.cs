using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// Run
    /// </summary>
    public sealed class Run
    {
        #region Private fields

        private readonly int maxRooms;
        private readonly RoomRegistry registry = new();
        private readonly int seed;

        #endregion

        #region Constructor

        // Constructor
        public Run(int seed, int maxRooms)
        {
            CodeContract.GreaterThanZero(maxRooms, nameof(maxRooms));

            this.seed = seed;
            this.maxRooms = maxRooms;
        }

        #endregion

        #region Private members

        // ConnectNodes
        private static void ConnectNodes(RoomNode a, RoomNode b, Point directionFromAToB)
        {
            if (directionFromAToB == new Point(0, -1)) { a.Up = b; b.Down = a; }
            else if (directionFromAToB == new Point(0, 1)) { a.Down = b; b.Up = a; }
            else if (directionFromAToB == new Point(-1, 0)) { a.Left = b; b.Right = a; }
            else if (directionFromAToB == new Point(1, 0)) { a.Right = b; b.Left = a; }
        }

        // DisconnectNodes - Helper vital para el Plan A Rígido
        private static void DisconnectNodes(RoomNode a, RoomNode b, Point directionFromAToB)
        {
            if (directionFromAToB == new Point(0, -1)) { a.Up = null; b.Down = null; }
            else if (directionFromAToB == new Point(0, 1)) { a.Down = null; b.Up = null; }
            else if (directionFromAToB == new Point(-1, 0)) { a.Left = null; b.Right = null; }
            else if (directionFromAToB == new Point(1, 0)) { a.Right = null; b.Left = null; }
        }

        // CountExistingNeighbors
        private int CountExistingNeighbors(Point p)
        {
            int count = 0;

            if (FloorMap.ContainsKey(p + new Point(0, -1))) count++;
            if (FloorMap.ContainsKey(p + new Point(0, 1))) count++;
            if (FloorMap.ContainsKey(p + new Point(-1, 0))) count++;
            if (FloorMap.ContainsKey(p + new Point(1, 0))) count++;

            return count;
        }

        // ExecutePhase1_Layout
        private void ExecutePhase1_Layout(Random rng)
        {
            var startNode = new RoomNode(FloorMap.Count, Point.Zero);
            FloorMap[Point.Zero] = startNode;

            List<RoomNode> activeNodes = [startNode];
            Point[] directions = [new(0, -1), new(0, 1), new(-1, 0), new(1, 0)]; // Up, Down, Left, Right

            while (FloorMap.Count < this.maxRooms)
            {
                var currentNode = activeNodes[rng.Next(activeNodes.Count)];
                Point dir = directions[rng.Next(directions.Length)];

                // --- REGLA STRICTA DE SEGURIDAD PARA START ROOM ---
                // Si el nodo actual es el Start (0,0) y el dado eligió ir hacia Abajo (0,1), cancelamos el intento.
                if (currentNode.GridPosition == Point.Zero && dir == new Point(0, 1))
                    continue;
                // --------------------------------------------------

                Point newPos = currentNode.GridPosition + dir;

                if (FloorMap.ContainsKey(newPos))
                    continue;

                // Además, protegemos el casillero (0,1) para que ninguna OTRA habitación crezca ahí desde los lados
                if (newPos == new Point(0, 1))
                    continue;

                if (CountExistingNeighbors(newPos) >= 3)
                    continue;

                var newNode = new RoomNode(FloorMap.Count, newPos);
                FloorMap[newPos] = newNode;
                activeNodes.Add(newNode);

                ConnectNodes(currentNode, newNode, dir);
            }
        }

        // ExecutePhase2_Labeling
        private void ExecutePhase2_Labeling(Random rng)
        {
            FloorMap[Point.Zero].Category = RoomCategory.Start;

            var deadEnds = new List<RoomNode>();
            foreach (var node in FloorMap.Values)
            {
                if (node.ConnectionCount() == 1 && node.Category == RoomCategory.Standard)
                {
                    deadEnds.Add(node);
                }
            }

            deadEnds.Sort((a, b) =>
            {
                int distA = GetManhattanDistance(Point.Zero, a.GridPosition);
                int distB = GetManhattanDistance(Point.Zero, b.GridPosition);
                return distB.CompareTo(distA);
            });

            if (deadEnds.Count > 0)
            {
                deadEnds[0].Category = RoomCategory.Boss;
                deadEnds.RemoveAt(0);
            }

            if (deadEnds.Count > 0)
            {
                deadEnds[0].Category = RoomCategory.Treasure;
                deadEnds.RemoveAt(0);
            }

            if (deadEnds.Count > 0)
            {
                deadEnds[0].Category = RoomCategory.Save;
                deadEnds.RemoveAt(0);
            }

            if (rng.NextDouble() <= 0.15)
            {
                var standardRooms = new List<RoomNode>();
                foreach (var node in FloorMap.Values)
                {
                    if (node.Category == RoomCategory.Standard)
                        standardRooms.Add(node);
                }

                if (standardRooms.Count > 0)
                {
                    var specialCandidate = standardRooms[rng.Next(standardRooms.Count)];
                    specialCandidate.Category = RoomCategory.Special;
                }
            }
        }

        // ExecutePhase3_AssignAssets
        private void ExecutePhase3_AssignAssets(Random rng)
        {
            int maxDistance = GetMaxFloorDistance();

            foreach (var node in FloorMap.Values)
            {
                Difficulty localRoomDiff = GetProgressiveDifficulty(node.GridPosition, maxDistance);
                node.TopographicDifficulty = localRoomDiff;

                var def = registry.GetValidDefinition(node, localRoomDiff, Spawns, rng);

                // --- SISTEMA DE FALLBACK SEGURO ---

                // Caso A: Es una sala Especial y no encaja -> Se degrada a común respetando la dificultad
                if (def == null && node.Category == RoomCategory.Special)
                {
                    node.Category = RoomCategory.Standard;
                    def = registry.GetValidDefinition(node, localRoomDiff, Spawns, rng);
                }

                // Caso B: Es una sala Mandatoria y no encuentra asset en esta dificultad -> Degradación escalonada
                if (def == null)
                {
                    if (node.Category is RoomCategory.Treasure or RoomCategory.Save or RoomCategory.Boss)
                    {
                        // Si era Hard, probamos en Normal
                        if (localRoomDiff == Difficulty.Hard)
                        {
                            def = registry.GetValidDefinition(node, Difficulty.Normal, Spawns, rng);
                        }

                        // Si sigue siendo null (o si originalmente era Normal), probamos en Easy
                        def ??= registry.GetValidDefinition(node, Difficulty.Easy, Spawns, rng);
                    }

                    // --- PLAN B RÍGIDO: SALVAGUARDA DE DISEÑO ---
                    // Si la degradación total falló (no tenés ningún asset de ese rol en el JSON),
                    // sacrificamos el rol mandatorio convirtiéndolo en Standard para evitar el crash.
                    if (def == null)
                    {
                        node.Category = RoomCategory.Standard;
                        def = registry.GetValidDefinition(node, localRoomDiff, Spawns, rng);
                    }
                }

                // ----------------------------------

                if (def == null)
                    throw new InvalidOperationException($"ERROR Crítico de Generación: No se encontró ningún asset en el JSON para el rol {node.Category} en la posición {node.GridPosition} (incluso tras intentar degradación de dificultad) que admita sus conexiones físicas.");

                node.Definition = def;
                Spawns.Increment(def.Name);
            }
        }

        // ExecutePhase4_InjectSecrets
        private void ExecutePhase4_InjectSecrets(Random rng)
        {
            int targetSecrets = rng.Next(0, 4);
            int secretsGenerated = 0;

            int maxDistance = GetMaxFloorDistance();

            var existingNodes = new List<RoomNode>(FloorMap.Count);
            foreach (var node in FloorMap.Values)
            {
                existingNodes.Add(node);
            }

            for (int i = existingNodes.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                var temp = existingNodes[i];
                existingNodes[i] = existingNodes[j];
                existingNodes[j] = temp;
            }

            for (int i = 0; i < existingNodes.Count; i++)
            {
                if (secretsGenerated >= targetSecrets)
                    break;

                var node = existingNodes[i];

                // Intento Izquierda
                Point leftPos = node.GridPosition + new Point(-1, 0);
                if (node.Left == null && node.Definition.DoorLeft != null && !FloorMap.ContainsKey(leftPos))
                {
                    Difficulty secretDiff = GetProgressiveDifficulty(leftPos, maxDistance);
                    if (InjectSecretNode(leftPos, node, new Point(1, 0), secretDiff, rng))
                    {
                        secretsGenerated++;
                    }
                    continue;
                }

                // Intento Derecha
                Point rightPos = node.GridPosition + new Point(1, 0);
                if (node.Right == null && node.Definition.DoorRight != null && !FloorMap.ContainsKey(rightPos))
                {
                    Difficulty secretDiff = GetProgressiveDifficulty(rightPos, maxDistance);
                    if (InjectSecretNode(rightPos, node, new Point(-1, 0), secretDiff, rng))
                    {
                        secretsGenerated++;
                    }
                }
            }
        }

        // GetManhattanDistance
        private static int GetManhattanDistance(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        // GetProgressiveDifficulty
        private static Difficulty GetProgressiveDifficulty(Point position, int maxDistance)
        {
            if (maxDistance == 0)
                return Difficulty.Easy;

            int currentDistance = GetManhattanDistance(Point.Zero, position);
            float ratio = (float)currentDistance / maxDistance;

            if (ratio < 0.33f)
                return Difficulty.Easy;

            if (ratio < 0.66f)
                return Difficulty.Normal;

            return Difficulty.Hard;
        }

        // GetMaxFloorDistance
        private int GetMaxFloorDistance()
        {
            int max = 0;
            foreach (var node in FloorMap.Values)
            {
                int dist = GetManhattanDistance(Point.Zero, node.GridPosition);
                if (dist > max) max = dist;
            }
            return max;
        }

        // InjectSecretNode - Ahora devuelve bool y maneja el descarte seguro del Plan A
        private bool InjectSecretNode(Point secretPos, RoomNode originNode, Point dirFromSecretToOrigin, Difficulty runDiff, Random rng)
        {
            var secretNode = new RoomNode(FloorMap.Count, secretPos)
            {
                Category = RoomCategory.Secret,
                TopographicDifficulty = runDiff
            };

            // Conectamos temporalmente para que Fits() evalúe la topología de forma correcta
            ConnectNodes(secretNode, originNode, dirFromSecretToOrigin);

            var def = registry.GetValidDefinition(secretNode, runDiff, this.Spawns, rng);
            if (def == null)
            {
                // Rollback total: Desconectamos los punteros y no agregamos nada al diccionario
                DisconnectNodes(secretNode, originNode, dirFromSecretToOrigin);
                return false;
            }

            // Si pasa el filtro, consolidamos el nodo físicamente en el mapa
            secretNode.Definition = def;
            FloorMap[secretPos] = secretNode;
            this.Spawns.Increment(def.Name);

            return true;
        }

        #endregion

        // FloorMap
        public Dictionary<Point, RoomNode> FloorMap { get; } = [];

        // Generate
        public void Generate(GameSession session)
        {
            var rng = new Random(seed);

            this.ExecutePhase1_Layout(rng);
            this.ExecutePhase2_Labeling(rng);
            this.ExecutePhase3_AssignAssets(rng);
            this.ExecutePhase4_InjectSecrets(rng);

            foreach (var node in FloorMap.Values)
            {
                node.RideRoom = RideRoom.CreateInstance(session, node);
            }

            foreach (var node in FloorMap.Values)
            {
                node.RideRoom.Load();
            }

            StartNode = FloorMap[Point.Zero];
        }

        // Spawns
        public CounterBank Spawns { get; } = new();

        // StartNode
        public RoomNode? StartNode { get; private set; }
    }
}