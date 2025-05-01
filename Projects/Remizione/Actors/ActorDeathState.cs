namespace Remizione
{
    /// <summary>
    /// ActorDeathState
    /// </summary>
    public sealed class ActorDeathState : ActorAnimatedState
    {
        // Constructor
        public ActorDeathState(Actor owner)
            : base(owner, ActorStateNames.Death, false)
        {
        }
    }
}
