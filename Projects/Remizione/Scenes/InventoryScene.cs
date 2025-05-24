namespace Remizione
{
    /// <summary>
    /// InventoryScene
    /// </summary>
    public sealed class InventoryScene : ItemContainerScene
    {
        // Constructor
        public InventoryScene(RemizioneGame game)
            : base(game, ItemContainerCategory.Inventory, true, true)
        {
        }
    }
}
