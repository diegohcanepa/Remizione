namespace Remizione.InteractionCommands
{
    /// <summary>
    /// ThrowHeldPropCommand
    /// </summary>
    public sealed class ThrowHeldPropCommand : InteractionCommand
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
            player.ThrowHeldProp(target);
        }
    }
}
