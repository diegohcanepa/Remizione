namespace Remizione.InteractionCommands
{
    /// <summary>
    /// ItemCommand
    /// </summary>
    public sealed class ItemCommand : InteractionCommand
    {
        // CanExecute
        public override bool CanExecute(InteractionData data, Actor player, GameThing target, Verb verb)
        {
            return data.Session.InteractionContext.HeldItem != null;
        }

        // Execute
        public override void Execute(InteractionData data, Actor player, GameThing target, Verb verb)
        {
            if (data.Session.InteractionContext.HeldItem is Item heldItem)
                player.ExecuteAction(heldItem, target);
        }
    }
}
