namespace Remizione.InteractionCommands
{
    /// <summary>
    /// ThrowCommand
    /// </summary>
    public sealed class ThrowCommand : InteractionCommand
    {
        // CanExecute
        public override bool CanExecute(InteractionData data, Actor player, GameThing target, Verb verb)
        {
            var heldItem = data.Session.InteractionContext.HeldItem;
            return heldItem == null && player.CarriedProp is not null && target.Verb == Verb.Attack;
        }

        // Execute
        public override void Execute(InteractionData data, Actor player, GameThing target, Verb verb)
        {
            player.StopMoving();
            player.ThrowCarriedProp(target);
        }
    }
}
