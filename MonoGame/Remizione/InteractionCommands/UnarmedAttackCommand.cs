namespace Remizione.InteractionCommands
{
    /// <summary>
    /// UnarmedAttackCommand
    /// </summary>
    public sealed class UnarmedAttackCommand : InteractionCommand
    {
        // CanExecute
        public override bool CanExecute(InteractionData data, Actor player, GameThing target, Verb verb)
        {
            if (player == target || !target.CanBeHit())
                return false;

            return player.Session.InteractionContext.UnarmedAttackMode;
        }

        // Execute
        public override void Execute(InteractionData data, Actor player, GameThing target, Verb verb)
        {
            player.ExecuteAction(PunchAction.Instance, target);
        }
    }
}