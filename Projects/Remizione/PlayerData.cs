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

        // Grace
        public int Grace { get; set; }

        // Inventory
        public ItemContainer Inventory { get; }
    }
}
