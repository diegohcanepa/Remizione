namespace Remizione.InteractionCommands
{
    /// <summary>
    /// InteractionCommand
    /// </summary>
    public abstract class InteractionCommand
    {
        // CanExecute
        public abstract bool CanExecute(InteractionData data, Actor player, GameThing target, Verb verb);

        // Execute
        public abstract void Execute(InteractionData data, Actor player, GameThing target, Verb verb);
    }
}