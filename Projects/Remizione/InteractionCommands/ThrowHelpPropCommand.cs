namespace Remizione.InteractionCommands
{
    /// <summary>
    /// ThrowHelpPropCommand
    /// </summary>
    public sealed class ThrowHelpPropCommand : InteractionCommand
    {
        // CanExecute
        public override bool CanExecute(InteractionData data, Actor player, GameThing target, Verb verb)
        {
            if (player == target)
                return false;

            if (player.Session.InteractionContext.HeldItem != null)
                return false;

            return player.HeldProp != null && verb == Verb.Attack;
        }

        // Execute
        public override void Execute(InteractionData data, Actor player, GameThing target, Verb verb)
        {
            if (player.Session.Room?.WalkArea?.InLineOfSight(player.Position, target.Position, RaycastContext.LineOfSight, out _) == true)
            {
                player.ThrowHeldProp(target);
            }
            else
            {
                player.Session.HUD.Message.Show(MessageKind.OutOfSight);
                MouseCursor.Shake();
            }
        }
    }
}
