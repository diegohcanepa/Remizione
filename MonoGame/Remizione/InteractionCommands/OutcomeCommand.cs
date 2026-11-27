using Adberration.Scripting;

namespace Remizione.InteractionCommands
{
    /// <summary>
    /// OutcomeCommand
    /// </summary>
    public sealed class OutcomeCommand : InteractionCommand
    {
        private Script? outcomeScript;

        // CanExecute
        public override bool CanExecute(InteractionData data, Actor player, GameThing target, Verb verb)
        {
            if (player.Session.InteractionContext.UnarmedAttackMode)
                return false;

            var heldItem = player.Session.InteractionContext.HeldItem;
            outcomeScript = null;

            if (heldItem == null)
            {
                if (target.Verb is not Verb.Attack and not Verb.Lift)
                    outcomeScript = target.OutcomeScript;
            }
            else
            {
                if (Utils.IsGoToVerb(target.Verb))
                {
                    outcomeScript = target.OutcomeScript;
                }
                else if (heldItem.Definition.ActionKind == ActionKind.Outcome)
                {
                    outcomeScript = target.Session.ScriptLibrary.FindOutcomeOverload(target.DeclaredName, heldItem.Name);
                }
            }

            return outcomeScript != null || (target == player && heldItem?.Definition.AllowSelfAction == true);
        }

        // Execute
        public override void Execute(InteractionData data, Actor player, GameThing target, Verb verb)
        {
            player.StopMoving();

            if (!CanExecute(data, player, target, verb))
                return;

            if (player.HeldProp != null)
                return;

            if (target.Verb == Verb.PickUp && !player.Session.InventoryEnabled)
            {
                MouseCursor.CustomImage = null;
                player.Session.InteractionContext.HeldItem = null;
                player.Session.AwaitRoutine(RoutineNames.NoSack);
                return;
            }

            if (target == player && player.Session.InteractionContext.HeldItem is { Definition.AllowSelfAction: true } helItem)
            {
                player.ExecuteAction(helItem, target);
            }
            else if (outcomeScript != null)
            {
                if (target.ApproachBehavior != ApproachBehavior.Over)
                    player.FaceTo(target);

                player.Session.BeginOutcome(outcomeScript, target);
            }

            player.Session.InteractionContext.HeldItem = null;
        }
    }
}
