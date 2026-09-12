namespace Remizione.InteractionCommands
{
    /// <summary>
    /// ThrowCommand
    /// </summary>
    public sealed class ThrowCommand : InteractionCommand
    {
        // CanExecute
        public override bool CanExecute(InteractionData data, Actor player, GameThing target)
        {
            var heldItem = data.Session.InteractionContext.HeldItem;
            return heldItem == null && player.ActiveThrowable is not null && target is { IsGoToVerb: false, Verb: Verb.Attack };
        }

        // Execute
        public override void Execute(InteractionData data, Actor player, GameThing target)
        {
            player.StopMoving();
            player.ThrowActiveTrowable(target);
        }
    }
}
