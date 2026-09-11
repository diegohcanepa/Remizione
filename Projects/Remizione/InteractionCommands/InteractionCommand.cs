namespace Remizione.InteractionCommands
{
    /// <summary>
    /// InteractionCommand
    /// </summary>
    public abstract class InteractionCommand
    {
        // CanExecute
        protected abstract bool CanExecute(InteractionData data, GameThing target);

        // OnExecute
        protected abstract bool OnExecute(InteractionData data, GameThing target);

        // Execute
        public bool Execute(InteractionData data, GameThing target)
        {
            return CanExecute(data, target) && OnExecute(data, target);
        }
    }
}