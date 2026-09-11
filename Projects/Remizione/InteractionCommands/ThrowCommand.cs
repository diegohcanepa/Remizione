namespace Remizione.InteractionCommands
{
    /// <summary>
    /// ThrowCommand
    /// </summary>
    public sealed class ThrowCommand : InteractionCommand
    {
        // CanExecute
        protected override bool CanExecute(InteractionData data, GameThing target)
        {
            var heldItem = data.Session.InteractionContext.HeldItem;
            var player = data.Session.Player;

            return heldItem == null && player?.ActiveThrowable is Prop && target is { IsGoToVerb: false, Verb: Verb.Attack };
        }

        // OnExecute
        protected override bool OnExecute(InteractionData data, GameThing target)
        {
            data.Session.Player?.StopMoving();
            data.Session.Player?.ThrowActiveTrowable(target);
            return true;
        }
    }
}
