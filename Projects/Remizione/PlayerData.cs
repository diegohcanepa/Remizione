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

        // IncrementMaterialCount
        public int IncrementMaterialCount(Material material, int amount = 1)
        {
            return material switch
            {
                Material.Gut => GutCount += amount,
                Material.Stone => StoneCount += amount,
                Material.Wood => WoodCount += amount,
                _ => throw new System.NotImplementedException()
            };
        }

        // GetMaterialCount
        public int GetMaterialCount(Material material)
        {
            return material switch
            {
                Material.Gut => GutCount,
                Material.Stone => StoneCount,
                Material.Wood => WoodCount,
                _ => throw new System.NotImplementedException()
            };
        }

        // Grace
        public int Grace { get; set; }

        // GutCount
        public int GutCount { get; set; }

        // Inventory
        public ItemContainer Inventory { get; }

        // StoneCount
        public int StoneCount { get; set; }

        // WoodCount
        public int WoodCount { get; set; }
    }
}
