namespace Remizione
{
    /// <summary>
    /// PlayerData
    /// </summary>
    public sealed class PlayerData
    {
        // Constructor
        public PlayerData(GameSession session)
        {
            Inventory = new(session);
        }

        // GetMaterialCount
        public int GetMaterialCount(Material material)
        {
            return material switch
            {
                Material.Gut => Gut,
                Material.Stone => Stone,
                Material.Wood => Wood,
                _ => throw new System.NotImplementedException()
            };
        }

        // Grace
        public int Grace { get; set; }

        // Gut
        public int Gut { get; set; }

        // Inventory
        public ItemContainer Inventory { get; }

        // Stone
        public int Stone { get; set; }

        // Wood
        public int Wood { get; set; }
    }
}
