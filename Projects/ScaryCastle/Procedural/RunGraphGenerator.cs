using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    public static class RunGraphGenerator
    {
        #region Private members

        // Cuenta cuántas habitaciones existen alrededor de una coordenada
        private static int CountNeighbors((int x, int y) c, Dictionary<(int x, int y), RoomGraph> map)
        {
            int count = 0;
            if (map.ContainsKey((c.x, c.y + 1))) count++;
            if (map.ContainsKey((c.x, c.y - 1))) count++;
            if (map.ContainsKey((c.x - 1, c.y))) count++;
            if (map.ContainsKey((c.x + 1, c.y))) count++;
            return count;
        }

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

        // Conecta la habitación nueva con su padre OBLIGATORIAMENTE.
        // Conecta con otros vecinos solo si el azar lo permite (extraDoorChance).
        private static void ConnectToNeighborsSmart(RoomGraph current, RoomGraph parent, Dictionary<(int x, int y), RoomGraph> map, Random rng, float extraDoorChance)
        {
            for (int dir = 0; dir < 4; dir++)
            {
                (int nx, int ny) = GetCoords(current.X, current.Y, dir);

                // Usamos TryGetValue que es seguro y rápido para AOT
                if (map.TryGetValue((nx, ny), out RoomGraph? neighbor) && neighbor != null)
                {
                    bool isParent = neighbor == parent;

                    // Si es el padre, conectamos sí o sí.
                    // Si es un vecino accidental, tiramos el dado.
                    if (isParent || rng.NextDouble() < extraDoorChance)
                    {
                        Link(current, neighbor, dir);
                    }
                }
            }
        }

        // BFS estándar usando Queue y Dictionary (Totalmente AOT safe)
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

                // Array manual en lugar de lista para evitar overhead
                RoomGraph?[] neighbors = [current.Up, current.Down, current.Left, current.Right];

                for (int i = 0; i < neighbors.Length; i++)
                {
                    RoomGraph? n = neighbors[i];
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
            occupied.Add((0, -1), null!);

            // --- CONFIGURACIÓN PARA EVITAR BLOQUES ---
            // Probabilidad de rellenar un hueco si ya tiene vecinos (0.2 = 20%)
            float loopChance = 0.2f;

            // Probabilidad de abrir pared con un vecino que NO es el padre (0.4 = 40%)
            // Bajar esto hace que haya paredes entre habitaciones adyacentes
            float extraDoorChance = 0.4f;

            // Factor de "Serpiente": Qué porcentaje de las últimas habitaciones creadas 
            // son candidatas preferidas para ser padres. (0.5 = la mitad más nueva)
            float recentRoomBias = 0.5f;

            int attempts = 0;
            // Límite de seguridad
            int maxAttempts = roomCount * 100;

            while (rooms.Count < roomCount && attempts < maxAttempts)
            {
                attempts++;

                // 2. Selección del Padre (Sin LINQ)
                RoomGraph parent;

                // 75% de probabilidad de elegir una habitación "reciente" (hace que el mapa se estire)
                // 25% de probabilidad de elegir cualquiera (hace que surjan ramas nuevas)
                if (rng.NextDouble() < 0.75)
                {
                    // Calculamos el índice de inicio basado en el bias
                    int minIndex = (int)(rooms.Count * (1.0f - recentRoomBias));
                    // Elegimos entre minIndex y el final
                    int index = rng.Next(minIndex, rooms.Count);
                    parent = rooms[index];
                }
                else
                {
                    // Totalmente aleatorio
                    parent = rooms[rng.Next(rooms.Count)];
                }

                // OPTIMIZACIÓN VISUAL:
                // Si el padre ya tiene 4 vecinos ocupados (físicamente), no perdemos tiempo intentando crecer ahí.
                // Esto ayuda a que el algoritmo busque bordes libres más rápido.
                if (CountNeighbors((parent.X, parent.Y), occupied) == 4)
                    continue;

                int direction = rng.Next(6) switch
                {
                    0 => 0,
                    1 => 1,
                    2 or 3 => 2,
                    _ => 3
                };

                if (parent.RoomType == RoomType.Start && direction == 1)
                    continue;

                (int x, int y) target = GetCoords(parent.X, parent.Y, direction);

                if (occupied.ContainsKey(target)) continue;

                // 3. Decisión de Colocación
                int neighborsCount = CountNeighbors(target, occupied);
                bool allowPlacement;

                if (neighborsCount == 1)
                {
                    // Solo toca al padre: Crecimiento natural
                    allowPlacement = true;
                }
                else
                {
                    // Toca al padre y a otros (potencial Loop)

                    // REGLA ANTI-CUADRICULADO:
                    // Si tiene 3 o más vecinos, es un hueco muy cerrado. NO colocamos nada ahí.
                    // Esto fuerza a dejar espacios vacíos ("patios") dentro del mapa.
                    if (neighborsCount >= 3)
                    {
                        allowPlacement = false;
                    }
                    else
                    {
                        // Si tiene 2 vecinos, tiramos dado
                        allowPlacement = rng.NextDouble() < loopChance;
                    }
                }

                if (allowPlacement)
                {
                    var newRoom = new RoomGraph(rooms.Count, target.x, target.y, RoomType.Connector);
                    occupied.Add(target, newRoom);
                    rooms.Add(newRoom);

                    // Conexión selectiva
                    ConnectToNeighborsSmart(newRoom, parent, occupied, rng, extraDoorChance);
                }
            }

            // 4. Procesar Distancias
            var endRoom = ProcessMapData(start);

            if (endRoom != start)
                endRoom.RoomType = RoomType.End;

            return (rooms, endRoom.DistanceFromStart);
        }
    }
}