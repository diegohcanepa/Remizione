namespace Remizione.InteractionCommands
{
    /// <summary>
    /// LiftCommand
    /// </summary>
    public sealed class LiftCommand : InteractionCommand
    {
        // CanExecute
        public override bool CanExecute(InteractionData data, Actor player, GameThing target)
        {
            return data.Session.InteractionContext.HeldItem == null && target.Verb == Verb.Lift;
        }

        // Execute
        public override void Execute(InteractionData data, Actor player, GameThing target)
        {
            if (target is Prop { IsLiftable: true } prop)
                player.Lift(prop);
        }
    }
}
