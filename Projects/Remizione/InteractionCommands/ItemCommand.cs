namespace Remizione.InteractionCommands
{
    /// <summary>
    /// ItemCommand
    /// </summary>
    public sealed class ItemCommand : InteractionCommand
    {
        // CanExecute
        protected override bool CanExecute(InteractionData data, GameThing target)
        {
            return data.Session.InteractionContext.HeldItem != null;
        }

        // OnExecute
        protected override bool OnExecute(InteractionData data, GameThing target)
        {
            var heldItem = data.Session.InteractionContext.HeldItem;
            if (heldItem == null)
                return false;

            data.Session.Player?.ExecuteAction(heldItem, target);
            return true;
        }
    }
}
