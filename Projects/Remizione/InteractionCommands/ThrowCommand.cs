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
            if (player.Session.InteractionContext.HeldItem != null)
                return false;

            return player.CarriedProp != null && verb == Verb.Attack;
        }

        // Execute
        public override void Execute(InteractionData data, Actor player, GameThing target, Verb verb)
        {
            player.ThrowCarriedProp(target);
        }
    }
}
