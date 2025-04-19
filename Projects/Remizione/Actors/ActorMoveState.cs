namespace Remizione
{
    /// <summary>
    /// ActorMoveState
    /// </summary>
    public sealed class ActorMoveState : ActorState
    {
        // Constructor
        public ActorMoveState(Actor owner)
            : base(owner, ActorStateNames.Move, ActorStateSettings.LoopAnimation)
        {
        }
    }
}
