namespace Remizione.InteractionCommands
{
    /// <summary>
    /// DropCarriedPropCommand
    /// </summary>
    public sealed class DropCarriedPropCommand : InteractionCommand
    {
        // CanExecute
        public override bool CanExecute(InteractionData data, Actor player, GameThing target, Verb verb)
        {
            return player.CarriedProp != null;
        }

        // Execute
        public override void Execute(InteractionData data, Actor player, GameThing target, Verb verb)
        {
            player.DropCarriedProp();
        }
    }
}
