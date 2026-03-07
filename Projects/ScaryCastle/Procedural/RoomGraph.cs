namespace ScaryCastle
{
    /// <summary>
    /// RoomGraph
    /// </summary>
    public sealed class RoomGraph
    {
        // Constructor
        public RoomGraph(int index, int x, int y, RoomType roomType)
        {
            this.Index = index;
            this.X = x;
            this.Y = y;
            this.RoomType = roomType;
        }

        #region Private members

        // ConnectionCount
        private void UpdateConnectionCount()
        {
            var count = 0;
            if (Up != null) count++;
            if (Down != null) count++;
            if (Left != null) count++;
            if (Right != null) count++;
            ConnectionCount = count;
        }

        #endregion

        // ConnectionCount
        public int ConnectionCount { get; private set; }

        // Definition
        public RoomDefinition Definition { get; set; } = null!;

        // DistanceFromStart
        public int DistanceFromStart { get; set; }

        // Down
        public RoomGraph? Down
        {
            get;
            set
            {
                field = value;
                UpdateConnectionCount();
            }
        }

        // Fits
        public bool Fits(RoomDefinition def)
        {
            // 1. ¿Qué conexiones reales tiene este nodo en el laberinto?
            bool needsUp = Up != null;
            bool needsDown = Down != null;
            bool needsLeft = Left != null;
            bool needsRight = Right != null;

            // 2. Validación de Encaje Estricto (El caso del Patio/Balcón)
            if (def.ExactMatch)
            {
                // La topología del nodo debe ser EXACTAMENTE igual a la de las puertas.
                // Si el nodo pide Norte y Sur, el asset debe tener Norte y Sur, y NINGUNA OTRA.
                return needsUp == def.HasUpDoor && needsDown == def.HasDownDoor &&
                       needsLeft == def.HasLeftDoor && needsRight == def.HasRightDoor;
            }

            // 3. Validación Flexible (El caso estándar - "Over-provisioning")
            // El asset tiene permiso para que le "sobren" puertas (que luego se taparán con un muro).
            // Pero NO le pueden faltar puertas donde el grafo exige una conexión.
            if (needsUp && !def.HasUpDoor) return false;
            if (needsDown && !def.HasDownDoor) return false;
            if (needsLeft && !def.HasLeftDoor) return false;
            if (needsRight && !def.HasRightDoor) return false;

            // Si pasó todas las validaciones flexibles, el asset cabe perfectamente.
            return true;
        }

        // HeartCount
        public int HeartCount { get; set; }

        // Index
        public int Index { get; }

        // Left
        public RoomGraph? Left
        {
            get;
            set
            {
                field = value;
                UpdateConnectionCount();
            }
        }

        // Realm
        public Realm Realm { get; set; }

        // RideRoom
        public RideRoom RideRoom { get; set; } = null!;

        // Right
        public RoomGraph? Right
        {
            get;
            set
            {
                field = value;
                UpdateConnectionCount();
            }
        }

        // RoomType
        public RoomType RoomType { get; set; }

        // ToString
        public override string ToString()
        {
            return $"[Room_{RoomType}_{Index} ({X},{Y})]";
        }

        // Up
        public RoomGraph? Up
        {
            get;
            set
            {
                field = value;
                UpdateConnectionCount();
            }
        }

        // Visited
        public bool Visited { get; set; }

        // X
        public int X { get; }

        // Y
        public int Y { get; }
    }
}
