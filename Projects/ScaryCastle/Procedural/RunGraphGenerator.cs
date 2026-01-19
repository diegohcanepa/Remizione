using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// RunGraphGenerator
    /// </summary>
    public static class RunGraphGenerator
    {
        #region Private members

        // CountNeighbors
        private static int CountNeighbors((int x, int y) c, Dictionary<(int x, int y), RoomGraph> map)
        {
            int count = 0;
            if (map.ContainsKey((c.x, c.y + 1))) count++;
            if (map.ContainsKey((c.x, c.y - 1))) count++;
            if (map.ContainsKey((c.x - 1, c.y))) count++;
            if (map.ContainsKey((c.x + 1, c.y))) count++;
            return count;
        }

        // GetCoords
        private static (int, int) GetCoords(int x, int y, int direction)
        {
            return direction switch
            {
                0 => (x, y + 1), // Up
                1 => (x, y - 1), // Down
                2 => (x - 1, y), // Left
                3 => (x + 1, y), // Right
                _ => (x, y)
            };
        }

        // Link
        // Conecta 'p' con 'c' usando la dirección 'direction' desde la perspectiva de 'p'
        private static void Link(RoomGraph p, RoomGraph c, int direction)
        {
            switch (direction)
            {
                case 0: p.Up = c; c.Down = p; break;
                case 1: p.Down = c; c.Up = p; break;
                case 2: p.Left = c; c.Right = p; break;
                case 3: p.Right = c; c.Left = p; break;
            }
        }

        // NUEVO: Conecta la habitación a TODOS sus vecinos existentes
        private static void ConnectToExistingNeighbors(RoomGraph room, Dictionary<(int x, int y), RoomGraph> map)
        {
            // Direcciones: 0:Up, 1:Down, 2:Left, 3:Right
            for (int dir = 0; dir < 4; dir++)
            {
                (int nx, int ny) = GetCoords(room.X, room.Y, dir);

                // Si hay una habitación válida en esa dirección...
                if (map.TryGetValue((nx, ny), out RoomGraph? neighbor) && neighbor != null)
                {
                    // ...las conectamos. 
                    // Link conecta 'room' hacia 'neighbor' en dirección 'dir'
                    Link(room, neighbor, dir);
                }
            }
        }

        // ProcessMapData (BFS para calcular distancias)
        private static RoomGraph ProcessMapData(RoomGraph start)
        {
            Queue<RoomGraph> queue = new();
            Dictionary<RoomGraph, int> visited = [];

            queue.Enqueue(start);
            visited.Add(start, 0);
            start.DistanceFromStart = 0;

            RoomGraph furthest = start;
            int maxDist = 0;

            while (queue.Count > 0)
            {
                RoomGraph current = queue.Dequeue();
                int d = visited[current];

                if (d > maxDist)
                {
                    maxDist = d;
                    furthest = current;
                }

                // Revisar conexiones existentes
                RoomGraph?[] neighbors = [current.Up, current.Down, current.Left, current.Right];
                foreach (var n in neighbors)
                {
                    if (n != null && !visited.ContainsKey(n))
                    {
                        n.DistanceFromStart = d + 1;
                        visited.Add(n, d + 1);
                        queue.Enqueue(n);
                    }
                }
            }
            return furthest;
        }

        #endregion

        // Generate
        public static (List<RoomGraph>, int) Generate(Random rng, int roomCount)
        {
            if (roomCount <= 0)
                return ([], 0);

            Dictionary<(int x, int y), RoomGraph> occupied = [];
            List<RoomGraph> rooms = [];

            // 1. Setup Start
            RoomGraph start = new(rooms.Count, 0, 0, RoomType.Start);
            occupied.Add((0, 0), start);
            rooms.Add(start);

            // Bloqueamos la coordenada de abajo
            occupied.Add((0, -1), null!);

            // CONFIGURACIÓN: Probabilidad de cerrar un loop (crear habitaciones que se tocan)
            // 0.5f = 50% de probabilidad de rellenar un hueco si hay más de 1 vecino
            float loopChance = 0.5f;

            // 2. Bucle de Generación
            int attempts = 0;
            // Aumentamos un poco el límite de intentos por si se encierra
            while (rooms.Count < roomCount && attempts < 5000)
            {
                attempts++;
                RoomGraph parent = rooms[rng.Next(rooms.Count)];

                int direction = rng.Next(6) switch
                {
                    0 => 0,
                    1 => 1,
                    2 or 3 => 2,
                    _ => 3
                };

                // Evitar conectar Start hacia abajo
                if (parent.RoomType == RoomType.Start && direction == 1)
                    continue;

                (int x, int y) target = GetCoords(parent.X, parent.Y, direction);

                // --- LÓGICA MODIFICADA PARA LOOPS ---

                // Si la casilla ya está ocupada, no hacemos nada
                if (occupied.ContainsKey(target))
                    continue;

                int neighborsCount = CountNeighbors(target, occupied);

                // Condición de colocación:
                // A) Solo tiene 1 vecino (el padre) -> Comportamiento clásico
                // B) Tiene >1 vecinos Y el azar lo permite -> Creamos un cruce/loop
                bool allowPlacement = neighborsCount == 1 || (neighborsCount > 1 && rng.NextDouble() < loopChance);

                if (allowPlacement)
                {
                    var newRoom = new RoomGraph(rooms.Count, target.x, target.y, RoomType.Connector);

                    // Importante: Agregar al diccionario ANTES de conectar
                    occupied.Add(target, newRoom);
                    rooms.Add(newRoom);

                    // Conectamos con el padre Y con cualquier otro vecino adyacente
                    ConnectToExistingNeighbors(newRoom, occupied);
                }
            }

            // 3. Procesar Distancias y marcar el exit
            // El BFS funciona perfectamente con loops y encontrará el camino más corto
            RoomGraph coinRoom = ProcessMapData(start);

            if (coinRoom != start)
                coinRoom.RoomType = RoomType.Exit;

            int maxDist = coinRoom.DistanceFromStart;
            return (rooms, maxDist);
        }
    }
}