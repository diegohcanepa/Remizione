using Adberration.Scripting;

namespace Remizione.InteractionCommands
{
    /// <summary>
    /// ScriptCommand
    /// </summary>
    public sealed class ScriptCommand : InteractionCommand
    {
        private Script? resolvedScript;

        // CanExecute
        public override bool CanExecute(InteractionData data, Actor player, GameThing target)
        {
            var heldItem = data.Session.InteractionContext.HeldItem;
            resolvedScript = null;

            if (heldItem == null)
            {
                if (target.Verb is not Verb.Attack and not Verb.Lift)
                    resolvedScript = target.OutcomeScript;
            }
            else
            {
                if (target.IsGoToVerb)
                {
                    resolvedScript = target.OutcomeScript;
                }
                else if (heldItem.Definition.ActionKind == ActionKind.Script)
                {
                    resolvedScript = target.Session.ScriptLibrary.FindOutcomeOverload(target.DeclaredName, heldItem.Name);
                }
            }

            return resolvedScript != null;
        }

        // Execute
        public override void Execute(InteractionData data, Actor player, GameThing target)
        {
            if (resolvedScript == null)
                return;

            player.StopMoving();

            if (target.Verb == Verb.PickUp && !data.Session.InventoryEnabled)
            {
                data.Session.AwaitRoutine(RoutineNames.NoSack);
                return;
            }

            /*
            if (Vector2.Distance(target.Position, data.TargetPosition) > 1)
            {
                data.Session.HUD?.Message.Show(MessageKind.OutOfReach);
                return false;
            }
            */

            player.FaceTo(target);
            data.Session.BeginOutcome(resolvedScript, target);

            if (data.Session.InteractionContext.HeldItem?.Definition.DeselectOnUse == true)
                data.Session.InteractionContext.HeldItem = null;
        }
    }
}
