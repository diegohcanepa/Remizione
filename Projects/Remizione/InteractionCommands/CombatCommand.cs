namespace Remizione.InteractionCommands
{
    /// <summary>
    /// CombatCommand
    /// </summary>
    public sealed class CombatCommand : InteractionCommand
    {
        private CombatIntent? resolvedIntent;

        // CanExecute
        public override bool CanExecute(InteractionData data, Actor player, GameThing target)
        {
            var heldItem = data.Session.InteractionContext.HeldItem;

            resolvedIntent = null;

            if (heldItem == null)
            {
                if (target.Verb == Verb.Attack)
                    resolvedIntent = player.CombatBehavior?.Intents[0];
            }
            else
            {
                if (!target.IsGoToVerb && heldItem.Definition.ActionKind != ActionKind.Script)
                    resolvedIntent = player.CombatBehavior?.Intents.Find(heldItem.Name);
            }

            return resolvedIntent != null;
        }

        // Execute
        public override void Execute(InteractionData data, Actor player, GameThing target)
        {
            if (resolvedIntent != null)
               player.ExecuteAction(resolvedIntent, target);
        }
    }
}
