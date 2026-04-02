using Engendro;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// Run
    /// </summary>
    public sealed class Run
    {
        private int corridorIndex = -1;
        private readonly int maxCorridors;
        private readonly RoomRegistry registry = new();
        private readonly int seed;

        // Constructor
        public Run(int seed, int maxCorridors)
        {
            this.seed = seed;
            this.maxCorridors = maxCorridors;
        }

        #region Private members

        // AssignAsset
        private void AssignAsset(RoomNode node, Difficulty diff, Random rng)
        {
            var def = registry.GetValidDefinition(node, diff, this.Spawns, rng);
            if (def == null)
                throw new InvalidOperationException($"ERROR: No assets found for {node.RoomType}/{node.SideRoomCategory} in {diff}");

            node.Definition = def;
            
            this.Spawns.Increment(def.Name);
        }

        // CleanUpCurrentCorridor
        private void CleanUpCurrentCorridor()
        {
            if (CurrentCorridor == null)
                return;

            foreach (var r in CurrentCorridor.GetAllNodes())
            {
                r.RideRoom?.Children.Clear();
            }

            CurrentCorridor = null;
        }

        // GenerateTrident
        private static void GenerateTrident(RoomNode corridor, ref int nodeCounter, Random rng)
        {
            // 1. El Nexo (Siempre arriba del corredor)
            var nexo = new RoomNode(nodeCounter++, corridor.X, 1, RoomType.SideRoom, SideRoomCategory.Standard);
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
            this.AssignAsset(corridor, diff, rng);

            // 2. Si hay Nexo, asignarlo y buscar la Hoja en sus 3 lados
            if (corridor.Up != null)
            {
                var nexo = corridor.Up;
                
                this.AssignAsset(nexo, diff, rng);

                // Solo uno de estos será distinto de null según el azar de GenerateTrident
                if (nexo.Left != null)
                    this.AssignAsset(nexo.Left, diff, rng);
                
                if (nexo.Up != null)
                    this.AssignAsset(nexo.Up, diff, rng);
                
                if (nexo.Right != null)
                    this.AssignAsset(nexo.Right, diff, rng);
            }
        }

        #endregion

        // CurrentCorridor
        public RoomNode? CurrentCorridor { get; private set; }

        // Intensity
        public float Intensity
        {
            get
            {
                if (maxCorridors <= 1)
                    return 0;

                float progress = (float)corridorIndex / maxCorridors;

                return (float)Math.Pow(Math.Clamp(progress, 0f, 1f), 1.2f);
            }
        }

        // NextCorridor
        public bool NextCorridor(GameSession session)
        {
            CleanUpCurrentCorridor();

            this.corridorIndex++;

            // Fin de la Run
            if (this.corridorIndex >= this.maxCorridors)
            {
                this.CurrentCorridor = null;
                return false;
            }

            // 1. Esqueleto (Nodos)
            // Usamos corridorIndex para la semilla, garantizando determinismo
            var rng = new Random(this.seed + this.corridorIndex);
            int localIndex = 0;

            // El RoomNode sigue necesitando X para la lógica de dificultad/posicionamiento
            var corridor = new RoomNode(localIndex++, this.corridorIndex, 0, RoomType.Corridor, SideRoomCategory.None);

            // 2. Tridente (Nexo + Hojas) - Lógica interna 50/50
            if (rng.NextDouble() < 0.5)
                GenerateTrident(corridor, ref localIndex, rng);

            // 3. Población (Asignación de assets según dificultad)
            Difficulty diff = GetDifficultyTier(this.corridorIndex, this.maxCorridors);
            this.Populate(corridor, diff, rng);

            // 4. Actualizar estado
            this.CurrentCorridor = corridor;

            // Build rooms
            foreach (var r in corridor.GetAllNodes())
            {
                r.RideRoom = RideRoom.CreateInstance(session, r);
            }

            foreach (var r in corridor.GetAllNodes())
            {
                r.RideRoom.Load();
            }

            return true;
        }

        // Spawns
        public CounterBank Spawns { get; } = new();
    }
}