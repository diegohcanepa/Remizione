namespace Remizione
{
    /// <summary>
    /// InventoryScene
    /// </summary>
    public sealed class OldInventoryScene : ItemContainerScene
    {
        // Constructor
        public OldInventoryScene(RemizioneGame game)
            : base(game, ItemContainerCategory.Inventory, true, true)
        {
        }
    }
}
