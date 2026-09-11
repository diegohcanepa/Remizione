namespace Remizione.InteractionCommands
{
    /// <summary>
    /// CombatCommand
    /// </summary>
    public sealed class CombatCommand : InteractionCommand
    {
        private CombatIntent? resolvedIntent;

        // CanExecute
        protected override bool CanExecute(InteractionData data, GameThing target)
        {
            var heldItem = data.Session.InteractionContext.HeldItem;
            var player = data.Session.Player;

            resolvedIntent = null;

            if (heldItem == null)
            {
                if (target.Verb == Verb.Attack)
                    resolvedIntent = player?.CombatBehavior?.Intents[0];
            }
            else
            {
                if (!target.IsGoToVerb && heldItem.Definition.ActionKind != ActionKind.Script)
                    resolvedIntent = player?.CombatBehavior?.Intents.Find(heldItem.Name);
            }

            return resolvedIntent != null;
        }

        // OnExecute
        protected override bool OnExecute(InteractionData data, GameThing target)
        {
            if (resolvedIntent == null)
                return false;

            data.Session.Player?.ExecuteAction(resolvedIntent, target);
            return true;
        }
    }
}
