namespace Remizione
{
    /// <summary>
    /// ActorDeathState
    /// </summary>
    public sealed class ActorDeathState : ActorState
    {
        // Constructor
        public ActorDeathState(Actor owner)
            : base(owner, ActorStateNames.Death, ActorStateSettings.None)
        {
        }
    }
}
