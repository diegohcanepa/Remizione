namespace Remizione.InteractionCommands
{
    /// <summary>
    /// CombatCommand
    /// </summary>
    public sealed class CombatCommand : InteractionCommand
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
                    if (heldItem.Definition.ActionKind != ActionKind.ScriptOutcome)
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
