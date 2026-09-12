namespace Remizione.InteractionCommands
{
    /// <summary>
    /// ItemCommand
    /// </summary>
    public sealed class ItemCommand : InteractionCommand
    {
        // CanExecute
        public override bool CanExecute(InteractionData data, Actor player, GameThing target)
        {
            return data.Session.InteractionContext.HeldItem != null;
        }

        // Execute
        public override void Execute(InteractionData data, Actor player, GameThing target)
        {
            if (data.Session.InteractionContext.HeldItem is Item heldItem)
                player.ExecuteAction(heldItem, target);
        }
    }
}
