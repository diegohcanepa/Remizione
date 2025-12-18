namespace ScaryCastle
{
    /// <summary>
    /// ActorMoveState
    /// </summary>
    public sealed class ActorMoveState : ActorAnimatedState
    {
        // Constructor
        public ActorMoveState(Actor owner)
            : base(owner, ActorStateNames.Move, true)
        {
        }
    }
}
