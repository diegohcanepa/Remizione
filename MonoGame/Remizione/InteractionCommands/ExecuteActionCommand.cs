namespace Remizione.InteractionCommands
{
    /// <summary>
    /// ExecuteActionCommand
    /// </summary>
    public sealed class ExecuteActionCommand : InteractionCommand
    {
        private IAction? resolvedAction;

        // CanExecute
        public override bool CanExecute(InteractionData data, Actor player, GameThing target, Verb verb)
        {
            resolvedAction = null;

            if (!Utils.IsGoToVerb(verb))
            {
                if (player.Session.InteractionContext.HeldItem is { } heldItem)
                {
                    if (heldItem.Definition.ActionKind != ActionKind.Outcome)
                        resolvedAction = heldItem;
                }
            }

            return resolvedAction != null;
        }

        // Execute
        public override void Execute(InteractionData data, Actor player, GameThing target, Verb verb)
        {
            if (resolvedAction != null)
                player.ExecuteAction(resolvedAction, target);
        }
    }
}
