namespace Remizione.InteractionCommands
{
    /// <summary>
    /// LiftCommand
    /// </summary>
    public sealed class LiftCommand : InteractionCommand
    {
        // CanExecute
        protected override bool CanExecute(InteractionData data, GameThing target)
        {
            return data.Session.InteractionContext.HeldItem == null && target.Verb == Verb.Lift;
        }

        // OnExecute
        protected override bool OnExecute(InteractionData data, GameThing target)
        {
            if (target is Prop { IsLiftable: true } prop)
            {
                data.Session.Player?.Lift(prop);
                return true;
            }

            return false;
        }
    }
}
