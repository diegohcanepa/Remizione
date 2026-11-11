namespace Remizione.InteractionCommands
{
    /// <summary>
    /// HoldCommand
    /// </summary>
    public sealed class HoldCommand : InteractionCommand
    {
        // CanExecute
        public override bool CanExecute(InteractionData data, Actor player, GameThing target, Verb verb)
        {
            return player.Session.InteractionContext.HeldItem == null && verb == Verb.Lift;
        }

        // Execute
        public override void Execute(InteractionData data, Actor player, GameThing target, Verb verb)
        {
            if (target is Prop prop && verb == Verb.Lift)
                player.Hold(prop);
        }
    }
}
