using Adberration.Scripting;
using Microsoft.Xna.Framework;

namespace Remizione.InteractionCommands
{
    /// <summary>
    /// ScriptCommand
    /// </summary>
    public sealed class ScriptCommand : InteractionCommand
    {
        private Script? resolvedScript;

        // CanExecute
        protected override bool CanExecute(InteractionData data, GameThing target)
        {
            var heldItem = data.Session.InteractionContext.HeldItem;
            resolvedScript = null;

            if (heldItem == null)
            {
                if (target.Verb != Verb.Attack && target.Verb != Verb.Lift)
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

        // OnExecute
        protected override bool OnExecute(InteractionData data, GameThing target)
        {
            if (resolvedScript == null)
                return false;

            var player = data.Session.Player;
            player?.StopMoving();

            if (target.Verb == Verb.PickUp && !data.Session.InventoryEnabled)
            {
                data.Session.AwaitRoutine(RoutineNames.NoSack);
                return false;
            }

            if (Vector2.Distance(target.Position, data.TargetPosition) > 1)
            {
                data.Session.HUD?.Message.Show(MessageKind.OutOfReach);
                return false;
            }

            player?.FaceTo(target);
            data.Session.BeginOutcome(resolvedScript, target);

            if (data.Session.InteractionContext.HeldItem?.Definition.DeselectOnUse == true)
                data.Session.InteractionContext.HeldItem = null;

            return true;
        }
    }
}
