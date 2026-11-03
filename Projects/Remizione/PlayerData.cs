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

        // Bonfires
        public int Bonfires { get; set; }

        // Grace
        public int Grace { get; set; }

        // Inventory
        public ItemContainer Inventory { get; }

        // IsSafe
        public bool IsSafe { get; set; } = true;
    }
}
