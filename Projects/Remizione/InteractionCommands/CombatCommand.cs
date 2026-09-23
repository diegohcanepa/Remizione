namespace Remizione.InteractionCommands
{
    /// <summary>
    /// CombatCommand
    /// </summary>
    public sealed class CombatCommand : InteractionCommand
    {
        private CombatIntent? resolvedIntent;

        // CanExecute
        public override bool CanExecute(InteractionData data, Actor player, GameThing target, Verb verb)
        {
            var heldItem = player.Session.InteractionContext.HeldItem;

            resolvedIntent = null;

            if (heldItem == null)
            {
                if (verb == Verb.Attack)
                    resolvedIntent = player.CombatBehavior?.Intents[0];
            }
            else
            {
                if (!Utils.IsGoToVerb(verb) && heldItem.Definition.ActionKind != ActionKind.Script)
                    resolvedIntent = player.CombatBehavior?.Intents.Find(heldItem.Name);
            }

            return resolvedIntent != null;
        }

        // Execute
        public override void Execute(InteractionData data, Actor player, GameThing target, Verb verb)
        {
            if (resolvedIntent != null)
                player.ExecuteAction(resolvedIntent, target);
        }
    }
}
