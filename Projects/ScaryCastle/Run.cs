using Engendro;
using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// Run
    /// </summary>
    public sealed class Run
    {
        private readonly RoomRegistry registry = new();
        private readonly int seed;

        // Constructor
        public Run(int seed, int maxCorridors)
        {
            CodeContract.GreaterThanZero(maxCorridors, nameof(maxCorridors));

            this.seed = seed;
            this.MaxCorridors = maxCorridors;
        }

        #region Private members

        // AssignDefinition
        private void AssignDefinition(RoomNode node, Difficulty diff, Random rng)
        {
            if (registry.GetValidDefinition(node, diff, this.Spawns, rng) is not RoomDefinition def)
            {
                throw new InvalidOperationException($"ERROR: No assets found for {node.RoomType}/{node.SideRoomCategory} in {diff}");
            }
            else
            {
                node.Definition = def;
                this.Spawns.Increment(def.Name);
            }
        }

        // CleanUpCurrentCorridor
        private void CleanUpCurrentCorridor()
        {
            if (CurrentCorridor != null)
            {
                foreach (var r in CurrentCorridor.GetAllNodes())
                {
                    r.RideRoom?.Children.Clear();
                }

                CurrentCorridor = null;
            }
        }

        // GenerateTrident
        private static void GenerateTrident(RoomNode corridor, ref int nodeCounter, Random rng)
        {
            // 1. El Nexo (Siempre arriba del corredor)
            var nexo = new RoomNode(nodeCounter++, corridor.X, 1, RoomType.SideRoom, SideRoomCategory.Hub);
            corridor.Up = nexo;
            nexo.Down = corridor;

            // 2. Chance de que aparezca la Hoja (Premio)
            if (rng.NextDouble() < 0.3)
            {
                var category = rng.NextDouble() < 0.5 ? SideRoomCategory.Treasure : SideRoomCategory.Save;

                // 3. Sorteo de dirección (0: Left, 1: Up, 2: Right)
                int dir = rng.Next(0, 3);

                // Creamos la hoja ajustando X e Y según la dirección
                RoomNode leaf;
                switch (dir)
                {
                    case 0: // Izquierda
                        leaf = new RoomNode(nodeCounter++, nexo.X - 1, nexo.Y, RoomType.SideRoom, category);
                        nexo.Left = leaf;
                        leaf.Right = nexo;
                        break;
                    case 1: // Arriba
                        leaf = new RoomNode(nodeCounter++, nexo.X, nexo.Y + 1, RoomType.SideRoom, category);
                        nexo.Up = leaf;
                        leaf.Down = nexo;
                        break;
                    default: // Derecha
                        leaf = new RoomNode(nodeCounter++, nexo.X + 1, nexo.Y, RoomType.SideRoom, category);
                        nexo.Right = leaf;
                        leaf.Left = nexo;
                        break;
                }
            }
        }

        // GetDifficultyTier
        private static Difficulty GetDifficultyTier(int x, int max)
        {
            // Calculamos el progreso normalizado (0.0 a 1.0)
            float progress = (float)(x + 1) / max;

            if (progress <= .33f)
                return Difficulty.Easy;   // Primer tercio

            if (progress <= .66f)
                return Difficulty.Normal; // Segundo tercio

            return Difficulty.Hard;
        }

        // Populate
        private void Populate(RoomNode corridor, Difficulty diff, Random rng)
        {
            // 1. Asignar al Corredor
            this.AssignDefinition(corridor, diff, rng);

            // 2. Si hay Nexo, asignarlo y buscar la Hoja en sus 3 lados
            if (corridor.Up != null)
            {
                var nexo = corridor.Up;

                this.AssignDefinition(nexo, diff, rng);

                // Solo uno de estos será distinto de null según el azar de GenerateTrident
                if (nexo.Left != null)
                    this.AssignDefinition(nexo.Left, diff, rng);

                if (nexo.Up != null)
                    this.AssignDefinition(nexo.Up, diff, rng);

                if (nexo.Right != null)
                    this.AssignDefinition(nexo.Right, diff, rng);
            }
        }

        // SetupDarkness
        private void SetupDarkness(Random rng)
        {
            if (CurrentCorridor == null)
                return;

            int darkRoll = rng.Next(1, 101);

            // A mayor Intensity, más chances de apagón general (va de 5% al inicio a 25% al final)
            var stageDarknessChance = float.Lerp(5, 25, this.Intensity);

            // Chance de side rooms oscuros pero pasillo con luz (va de 15% al inicio a 35% al final)
            var isolatedDarknessChance = float.Lerp(15, 35, this.Intensity);

            if (darkRoll <= stageDarknessChance)
            {
                foreach (var roomNode in CurrentCorridor.GetAllNodes())
                {
                    roomNode.RideRoom?.TurnOffLights();
                }
            }
            else if (darkRoll <= stageDarknessChance + isolatedDarknessChance)
            {
                foreach (var roomNode in CurrentCorridor.GetAllNodes())
                {
                    if (roomNode.RoomType != RoomType.Corridor)
                        roomNode.RideRoom?.TurnOffLights();
                }
            }
        }

        // SpawnGoo
        private void SpawnGoo(Random rng)
        {
            if (CurrentCorridor == null)
                return;

            var roomList = new List<RideRoom>();
            foreach (var node in CurrentCorridor.GetAllNodes())
            {
                if (node.RideRoom != null)
                    roomList.Add(node.RideRoom);
            }

            if (roomList.Count == 0)
                return;

            // Calculamos una chance que decrece con la intensidad. 
            // Al principio (Intensity 0): 85% de chance.
            // Al final (Intensity 1): 20% de chance (Un milagro absoluto).
            float spawnChance = float.Lerp(.85f, .20f, this.Intensity);

            if (rng.NextDouble() <= spawnChance)
            {
                int luckyRoomIndex = rng.Next(0, roomList.Count);
                var room = roomList[luckyRoomIndex];

                if (room.WalkArea != null)
                {
                    if (room.CreateThingClone<Goo>(nameof(Goo)) is Goo goo)
                    {
                        goo.Position = room.WalkArea.RandomWalkablePoint(10);
                        room.Children.Add(goo);
                    }
                }
            }
        }

        #endregion

        // CorridorIndex
        public int CorridorIndex { get; private set; } = -1;

        // CurrentCorridor
        public RoomNode? CurrentCorridor { get; private set; }

        // Intensity
        public float Intensity
        {
            get
            {
                if (MaxCorridors <= 1)
                    return 0;

                float progress = (float)CorridorIndex / MaxCorridors;

                return (float)Math.Pow(Math.Clamp(progress, 0f, 1f), 1.2f);
            }
        }

        // LoadNextCorridor
        public bool LoadNextCorridor(GameSession session)
        {
            CleanUpCurrentCorridor();

            this.CorridorIndex++;

            // Fin de la Run
            if (this.CorridorIndex >= this.MaxCorridors)
            {
                this.CurrentCorridor = null;
                return false;
            }

            // 1. Esqueleto (Nodos)
            // Usamos corridorIndex para la semilla, garantizando determinismo
            var rng = new Random(this.seed + this.CorridorIndex);
            int localIndex = 0;

            // El RoomNode sigue necesitando X para la lógica de dificultad/posicionamiento
            var corridor = new RoomNode(localIndex++, this.CorridorIndex, 0, RoomType.Corridor, SideRoomCategory.None);

            // 2. Tridente (Nexo + Hojas)
            GenerateTrident(corridor, ref localIndex, rng);

            // 3. Población (Asignación de assets según dificultad)
            Difficulty diff = GetDifficultyTier(this.CorridorIndex, this.MaxCorridors);
            this.Populate(corridor, diff, rng);

            // 4. Actualizar estado
            this.CurrentCorridor = corridor;

            // Build rooms
            foreach (var roomNode in corridor.GetAllNodes())
            {
                roomNode.RideRoom = RideRoom.CreateInstance(session, roomNode);
            }

            foreach (var roomNode in corridor.GetAllNodes())
            {
                roomNode.RideRoom.Load();
            }

            SpawnGoo(rng);

            SetupDarkness(rng);

            return true;
        }

        // MaxCorridors
        public int MaxCorridors { get; }

        // Progress
        public Ratio Progress => CorridorIndex < 0 ? 0 : (float)CorridorIndex / MaxCorridors;

        // Spawns
        public CounterBank Spawns { get; } = new();
    }
}