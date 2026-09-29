using Adberration.Scripting;

namespace Remizione.InteractionCommands
{
    /// <summary>
    /// ScriptOutcomeCommand
    /// </summary>
    public sealed class ScriptOutcomeCommand : InteractionCommand
    {
        private Script? resolvedScript;

        // CanExecute
        public override bool CanExecute(InteractionData data, Actor player, GameThing target, Verb verb)
        {
            var heldItem = player.Session.InteractionContext.HeldItem;
            resolvedScript = null;

            if (heldItem == null)
            {
                if (target.Verb is not Verb.Attack and not Verb.Lift)
                    resolvedScript = target.OutcomeScript;
            }
            else
            {
                if (Utils.IsGoToVerb(target.Verb))
                {
                    resolvedScript = target.OutcomeScript;
                }
                else if (heldItem.Definition.ActionKind == ActionKind.ScriptOutcome)
                {
                    resolvedScript = target.Session.ScriptLibrary.FindOutcomeOverload(target.DeclaredName, heldItem.Name);
                }
            }

            return resolvedScript != null;
        }

        // Execute
        public override void Execute(InteractionData data, Actor player, GameThing target, Verb verb)
        {
            if (resolvedScript == null)
                return;

            player.StopMoving();

            if (target.Verb == Verb.PickUp && !player.Session.InventoryEnabled)
            {
                MouseCursor.CustomImage = null;
                player.Session.InteractionContext.HeldItem = null;
                player.Session.AwaitRoutine(RoutineNames.NoSack);
                return;
            }

            /*
            if (Vector2.Distance(target.Position, data.TargetPosition) > 1)
            {
                data.Session.HUD?.Message.Show(MessageKind.OutOfReach);
                return false;
            }
            */

            if (player.Session.InteractionContext.HeldItem?.Definition.DeselectOnUse == true)
                player.Session.InteractionContext.HeldItem = null;

            if (target.ApproachBehavior != ApproachBehavior.Over)
                player.FaceTo(target);

            player.Session.BeginOutcome(resolvedScript, target);
        }
    }
}
