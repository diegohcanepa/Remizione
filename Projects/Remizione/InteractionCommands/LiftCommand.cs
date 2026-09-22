namespace Remizione.InteractionCommands
{
    /// <summary>
    /// LiftCommand
    /// </summary>
    public sealed class LiftCommand : InteractionCommand
    {
        // CanExecute
        public override bool CanExecute(InteractionData data, Actor player, GameThing target, Verb verb)
        {
            return data.Session.InteractionContext.HeldItem == null && verb == Verb.Lift;
        }

        // Execute
        public override void Execute(InteractionData data, Actor player, GameThing target, Verb verb)
        {
            if (target is Prop prop && verb == Verb.Lift)
                player.Lift(prop);
        }
    }
}
