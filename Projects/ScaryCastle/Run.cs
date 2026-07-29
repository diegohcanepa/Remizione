using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ScaryCastle
{
    /// <summary>
    /// Run
    /// </summary>
    public sealed class Run
    {
        #region Private fields

        private readonly Dictionary<Point, RoomNode> floorMap = [];
        private const int gridRadius = 4; // Radio 4 significa de -4 a 4 (Matriz de 9x9)
        private readonly RoomRegistry registry = new();
        private readonly GameSession session;
        private readonly int totalRooms;

        #endregion

        #region Constructor

        // Constructor
        public Run(GameSession session, int totalRooms)
        {
            CodeContract.ValidRange(totalRooms, 10, 20, nameof(totalRooms));

            this.session = session;
            this.totalRooms = totalRooms;
        }

        #endregion

        #region Private members

        // CleanUp
        private void CleanUp()
        {
            session.CleanUpRuntimeEntities();
            session.InteractionContext.HeldItem = null;

            // 1. Force an immediate collection of all generations (0, 1, and 2).
            // 'Forced' tells the GC to ignore its internal heuristics and run immediately.
            // 'true' makes the call blocking (execution halts until the GC finishes).
            GC.Collect(2, GCCollectionMode.Forced, true);

            // 2. Wait for objects with finalizers (destructors) to finish their cleanup logic.
            GC.WaitForPendingFinalizers();

            // 3. Collect again.
            // This is necessary because objects finalized in step 2 are now officially
            // marked as "garbage" and can finally be released from memory in this pass.
            GC.Collect(2, GCCollectionMode.Forced, true);
        }

        // ConnectNodes
        private static void ConnectNodes(RoomNode a, RoomNode b, Point directionFromAToB)
        {
            if (directionFromAToB == new Point(0, -1)) { a.Up = b; b.Down = a; }
            else if (directionFromAToB == new Point(0, 1)) { a.Down = b; b.Up = a; }
            else if (directionFromAToB == new Point(-1, 0)) { a.Left = b; b.Right = a; }
            else if (directionFromAToB == new Point(1, 0)) { a.Right = b; b.Left = a; }
        }

        // CountExistingNeighbors
        private int CountExistingNeighbors(Point p)
        {
            int count = 0;
            if (floorMap.ContainsKey(p + new Point(0, -1))) count++;
            if (floorMap.ContainsKey(p + new Point(0, 1))) count++;
            if (floorMap.ContainsKey(p + new Point(-1, 0))) count++;
            if (floorMap.ContainsKey(p + new Point(1, 0))) count++;
            return count;
        }

        // ExecutePhase1_Layout
        private void ExecutePhase1_Layout(Random rng)
        {
            int totalLayoutAttempts = 0;
            const int maxLayoutAttempts = 1000;

            // Ahora las 4 direcciones están completamente liberadas desde el inicio
            Point[] directions = [new(0, -1), new(0, 1), new(-1, 0), new(1, 0)];

            while (true)
            {
                floorMap.Clear();

                // 1. Clavamos el START en el centro lógico
                var startNode = new RoomNode(0, Point.Zero) { Category = RoomCategory.Start };
                floorMap[Point.Zero] = startNode;

                List<RoomNode> activeNodes = [startNode];

                int iterationsWithoutSuccess = 0;
                const int maxStagnantIterations = 500;

                // 2. Bucle de expansión (La Mancha)
                while (floorMap.Count < totalRooms && iterationsWithoutSuccess < maxStagnantIterations)
                {
                    iterationsWithoutSuccess++;

                    if (activeNodes.Count == 0)
                        break;

                    var currentNode = activeNodes[rng.Next(activeNodes.Count)];
                    Point dir = directions[rng.Next(directions.Length)];
                    Point newPos = currentNode.GridPosition + dir;

                    // --- LIMITACIÓN DE GRILLA PARAMETRIZADA (Bounding Box de 9x9) ---
                    if (Math.Abs(newPos.X) > gridRadius || Math.Abs(newPos.Y) > gridRadius)
                        continue;

                    // Chequeo de celda libre
                    if (floorMap.ContainsKey(newPos))
                        continue;

                    // --- REGLA DE ORO DE ADYACENCIA ---
                    // Si la posición propuesta tiene más de 1 vecino, generaría un bucle.
                    // Lo descartamos para forzar la expansión arbórea sin colisiones.
                    if (CountExistingNeighbors(newPos) > 1)
                        continue;

                    // El nodo superó los filtros, lo consolidamos
                    var newNode = new RoomNode(floorMap.Count, newPos);
                    floorMap[newPos] = newNode;
                    activeNodes.Add(newNode);

                    ConnectNodes(currentNode, newNode, dir);

                    // Éxito: reiniciamos el contador de estancamiento
                    iterationsWithoutSuccess = 0;
                }

                // 3. Validación de Topología
                if (floorMap.Count == totalRooms)
                {
                    StartNode = floorMap[Point.Zero];
                    break; // Salimos del while(true), la mancha está lista
                }

                // Si se estancó (ej: se arrinconó solo por RNG), hacemos rollback y reintentamos
                totalLayoutAttempts++;
                if (totalLayoutAttempts > maxLayoutAttempts)
                    throw new InvalidOperationException($"CRITICAL ERROR: Phase 1 failed.");
            }
        }

        // ExecutePhase2_Labeling
        private void ExecutePhase2_Labeling(Random rng)
        {
            // 1. El START ya está fijado, pero nos aseguramos por las dudas
            floorMap[Point.Zero].Category = RoomCategory.Start;

            // 2. Recolectamos los Dead-Ends naturales (callejones sin salida de la Fase 1)
            var deadEnds = new List<RoomNode>();
            foreach (var node in floorMap.Values)
            {
                if (node.ConnectionCount() == 1 && node.Category == RoomCategory.Standard)
                {
                    deadEnds.Add(node);
                }
            }

            // 3. Los ordenamos de Mayor a Menor distancia Manhattan con respecto al Start (0,0)
            deadEnds.Sort((a, b) =>
            {
                int distA = Math.Abs(a.GridPosition.X) + Math.Abs(a.GridPosition.Y);
                int distB = Math.Abs(b.GridPosition.X) + Math.Abs(b.GridPosition.Y);
                return distB.CompareTo(distA);
            });

            // --- INYECCIÓN DEL BOSS (Reemplazo obligatorio) ---
            if (deadEnds.Count > 0)
            {
                deadEnds[0].Category = RoomCategory.Boss;
                deadEnds.RemoveAt(0); // Lo sacamos para que no lo use otra sala
            }
            else
            {
                // Paracaídas de seguridad extremo: si la Fase 1 dio una masa sin dead-ends (rarísimo), 
                // buscamos la habitación más lejana de la grilla y la obligamos a ser el Boss.
                RoomNode? furthestNode = null;
                int maxDist = -1;
                foreach (var node in floorMap.Values)
                {
                    if (node.Category == RoomCategory.Standard)
                    {
                        int dist = Math.Abs(node.GridPosition.X) + Math.Abs(node.GridPosition.Y);
                        if (dist > maxDist)
                        {
                            maxDist = dist;
                            furthestNode = node;
                        }
                    }
                }

                furthestNode?.Category = RoomCategory.Boss;
            }

            // --- INYECCIÓN DE TREASURE Y STORE (Uso de Dead-Ends o Adosado) ---
            RoomCategory[] remainingMandatories = [RoomCategory.Treasure, RoomCategory.Store];
            Point[] directions = [new(0, -1), new(0, 1), new(-1, 0), new(1, 0)]; // Up, Down, Left, Right

            foreach (var category in remainingMandatories)
            {
                // Plan A: Si todavía nos queda un Dead-End natural libre, lo usamos (Reemplazo)
                if (deadEnds.Count > 0)
                {
                    deadEnds[0].Category = category;
                    deadEnds.RemoveAt(0);
                    continue;
                }

                // Plan B: El Brote (Adosado hacia afuera)
                bool placed = false;

                // Creamos una lista de salas comunes para intentar tirar el brote desde alguna de ellas
                var candidates = new List<RoomNode>();
                foreach (var node in floorMap.Values)
                {
                    if (node.Category == RoomCategory.Standard) candidates.Add(node);
                }

                // Mezclamos los candidatos al azar para que el brote no salga siempre del mismo lugar
                for (int i = candidates.Count - 1; i > 0; i--)
                {
                    int j = rng.Next(i + 1);
                    (candidates[i], candidates[j]) = (candidates[j], candidates[i]);
                }

                foreach (var baseNode in candidates)
                {
                    // Buscamos una dirección libre alrededor de esta sala común
                    // Mezclamos también las direcciones para aportar variedad visual
                    Point[] shuffledDirs = (Point[])directions.Clone();
                    for (int i = shuffledDirs.Length - 1; i > 0; i--)
                    {
                        int j = rng.Next(i + 1);
                        (shuffledDirs[i], shuffledDirs[j]) = (shuffledDirs[j], shuffledDirs[i]);
                    }

                    foreach (var dir in shuffledDirs)
                    {
                        Point candidatePos = baseNode.GridPosition + dir;

                        // Verificamos que no se pase del Bounding Box de la Fase 1
                        if (Math.Abs(candidatePos.X) > gridRadius || Math.Abs(candidatePos.Y) > gridRadius)
                            continue;

                        // Verificamos que la celda esté realmente vacía
                        if (floorMap.ContainsKey(candidatePos))
                            continue;

                        // Registramos y consolidamos el nuevo nodo adosado en la grilla
                        var newNode = new RoomNode(floorMap.Count, candidatePos) { Category = category };
                        floorMap[candidatePos] = newNode;

                        ConnectNodes(baseNode, newNode, dir);

                        placed = true;
                        break;
                    }

                    if (placed) break;
                }

                // Seguro total: Si por algún motivo de trabe espacial no se pudo adosar en la grilla 9x9,
                // tiramos la excepción controlada para activar el rollback global.
                if (!placed)
                    throw new InvalidOperationException($"Tagging ERROR: No free space could be found to attach the mandatory room {category}.");
            }
        }

        // ExecutePhase3_InjectSecrets
        private void ExecutePhase3_InjectSecrets(Random rng)
        {
        }

        // ExecutePhase4_AssignDefinitions
        private void ExecutePhase4_AssignDefinitions(Random rng)
        {
            int maxDistance = GetMaxFloorDistance();

            foreach (var node in floorMap.Values)
            {
                // 1. Calculamos la dificultad matemática según su posición en la grilla
                Difficulty localRoomDiff = GetProgressiveDifficulty(node.GridPosition, maxDistance);
                node.TopographicDifficulty = localRoomDiff;

                // 2. Intentamos buscar la definición del asset que calce con la topología
                var def = registry.GetValidDefinition(node, localRoomDiff, Spawns, rng);

                // --- SISTEMA DE FALLBACK SEGURO (Degradación Escalonada) ---

                // Caso A: Es una sala Especial y no encaja -> Se degrada a común respetando la dificultad
                if (def == null && node.Category == RoomCategory.Special)
                {
                    node.Category = RoomCategory.Standard;
                    def = registry.GetValidDefinition(node, localRoomDiff, Spawns, rng);
                }

                // Caso B: Es una sala Mandatoria (Boss, Treasure, Store) y no encuentra asset en su dificultad
                if (def == null)
                {
                    if (node.Category is RoomCategory.Treasure or RoomCategory.Store or RoomCategory.Boss)
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
                    // Si la degradación de dificultad falló por completo (ej: no tenés ese asset en el JSON),
                    // sacrificamos el rol especial convirtiéndolo en Standard para evitar que el juego crasheé.
                    if (def == null)
                    {
                        node.Category = RoomCategory.Standard;
                        def = registry.GetValidDefinition(node, localRoomDiff, Spawns, rng);
                    }
                }

                // Si después de todo esto sigue siendo null, es porque el JSON no tiene ni una sala básica Standard
                if (def == null)
                    throw new InvalidOperationException($"Critical Generation Error: No asset was found in the JSON for the role {node.Category} at position {node.GridPosition} (even after full demotion) that supports its physical connections.");

                // Consolidamos el asset y sumamos al banco de spawns
                node.Definition = def;
                Spawns.Increment(def.Name);
            }
        }

        // ExecutePhase5_TopologyAndLocks
        private void ExecutePhase5_TopologyAndLocks(Random rng)
        {
            int maxDistance = GetMaxFloorDistance();

            var visited = new HashSet<RoomNode>();
            var queue = new Queue<RoomNode>();
            var accessibleRooms = new List<RoomNode>();

            var startNode = floorMap[Point.Zero];
            queue.Enqueue(startNode);
            visited.Add(startNode);

            Point[] directions = [new(0, -1), new(0, 1), new(-1, 0), new(1, 0)];

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                accessibleRooms.Add(currentNode);

                // Control local: máximo 1 reja/palanca iniciada desde este nodo
                bool hasGateInCurrentNode = false;

                foreach (var dir in directions)
                {
                    Point neighborPos = currentNode.GridPosition + dir;

                    if (floorMap.TryGetValue(neighborPos, out RoomNode? targetNode) && !visited.Contains(targetNode))
                    {
                        Difficulty targetDiff = GetProgressiveDifficulty(targetNode.GridPosition, maxDistance);

                        if (targetNode.Category == RoomCategory.Standard)
                        {
                            DoorDirection currentToTarget = GetDoorDirectionFromPoint(dir);
                            DoorDirection targetToCurrent = GetDoorDirectionFromPoint(new Point(-dir.X, -dir.Y));

                            // REGLA VISUAL: Sin candados hacia abajo ni en el Start
                            if (currentToTarget == DoorDirection.Down ||
                                targetToCurrent == DoorDirection.Down ||
                                currentNode.Category == RoomCategory.Start)
                            {
                                visited.Add(targetNode);
                                queue.Enqueue(targetNode);
                                continue;
                            }

                            var bronzeKeyChance = targetDiff switch
                            {
                                Difficulty.Easy => 0.2f,
                                Difficulty.Normal => 0.2f,
                                Difficulty.Hard => 0.4f,
                                _ => 0.4f
                            };

                            // 1. Candado de Bronce
                            if (rng.NextDouble() <= bronzeKeyChance)
                            {
                                currentNode.LockedDoors[currentToTarget] = LockType.BronzeKey;
                                targetNode.LockedDoors[targetToCurrent] = LockType.BronzeKey;
                                var safeRoom = accessibleRooms[rng.Next(accessibleRooms.Count)];
                                safeRoom.BronzeKeys++;
                            }

                            // 2. Reja con Palanca (solo si el asset asignado en Fase 4 tiene placeholder y no pusimos reja aún)
                            else if (!hasGateInCurrentNode &&
                                     currentNode.Definition.Placeholders.GetPlaceholdersByTag(Tag.GateLever) is { } phList &&
                                     phList.Count > 0)
                            {
                                phList.Shuffle();

                                for (var i = 0; i < phList.Count; i++)
                                {
                                    if (currentNode.GetPlaceholderState(phList[i]) != PlaceholderState.Pending)
                                        continue;

                                    if (phList[i].FillChance.Roll(rng))
                                    {
                                        currentNode.LockedDoors[currentToTarget] = LockType.GateLever;
                                        targetNode.LockedDoors[targetToCurrent] = LockType.GateLever;
                                        hasGateInCurrentNode = true;

                                        // Asignamos la palanca al slot ganador
                                        currentNode.SetPlaceholderState(phList[i], PlaceholderState.GateLever);

                                        // Quemamos el resto de las alternativas de la sala
                                        for (var j = 0; j < phList.Count; j++)
                                        {
                                            if (j != i)
                                                currentNode.SetPlaceholderState(phList[j], PlaceholderState.Used);
                                        }

                                        break;
                                    }
                                }
                            }
                        }

                        visited.Add(targetNode);
                        queue.Enqueue(targetNode);
                    }
                }
            }
        }

        // ExecutePhase6_PrepareRooms
        private void ExecutePhase6_PrepareRooms(Random floorRng)
        {
            foreach (var node in floorMap.Values)
            {
                // El floorRng escupe un int único, determinista y seguro para esta sala exacta
                int roomSeed = floorRng.Next();

                // Se lo pasamos a la fábrica
                node.Room = ProceduralRoom.CreateInstance(session, node, roomSeed);
            }

            foreach (var node in floorMap.Values)
            {
                node.Room.Load();
            }
        }

        // GetDoorDirectionFromPoint
        private static DoorDirection GetDoorDirectionFromPoint(Point direction)
        {
            return direction switch
            {
                { X: 0, Y: -1 } => DoorDirection.Up,
                { X: 1, Y: 0 } => DoorDirection.Right,
                { X: 0, Y: 1 } => DoorDirection.Down,
                { X: -1, Y: 0 } => DoorDirection.Left,
                _ => throw new ArgumentException($"Invalid direction: {direction}")
            };
        }

        // GetManhattanDistance
        private static int GetManhattanDistance(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        // GetMaxFloorDistance
        private int GetMaxFloorDistance()
        {
            int max = 0;
            foreach (var node in floorMap.Values)
            {
                int dist = GetManhattanDistance(Point.Zero, node.GridPosition);
                if (dist > max) max = dist;
            }
            return max;
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

        #endregion

        // CurrentFloor
        public int CurrentFloor { get; private set; } = -1;

        // FloorMap
        public ReadOnlyDictionary<Point, RoomNode> FloorMap => new(floorMap);

        // NextFloor
        public void NextFloor(int floorIncrement = 1)
        {
            if (CurrentFloor >= 0)
                CleanUp();

            CurrentFloor += floorIncrement;
            if (CurrentFloor > 666)
                CurrentFloor = 666;

            // El seed de este piso lo dicta el RNG Maestro. 
            // Si recargas la run, el orden de pisos será exactamente igual.
            int currentFloorSeed = session.MasterRunRng.Next();
            var floorRng = new Random(currentFloorSeed);

            ExecutePhase1_Layout(floorRng);
            ExecutePhase2_Labeling(floorRng);
            ExecutePhase3_InjectSecrets(floorRng);
            ExecutePhase4_AssignDefinitions(floorRng);
            ExecutePhase5_TopologyAndLocks(floorRng);
            ExecutePhase6_PrepareRooms(floorRng);

            StartNode = floorMap[Point.Zero];

            int totalKeys = 0;
            foreach (var node in floorMap.Values)
            {
                totalKeys += node.BronzeKeys;
            }
            TotalBronzeKeys = totalKeys;
        }

        // Spawns
        public CounterBank Spawns { get; } = new();

        // StartNode
        public RoomNode? StartNode { get; private set; }

        // TotalBronzeKeys
        public int TotalBronzeKeys { get; private set; }
    }
}