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

        // GetAnimationName
        protected override string GetAnimationName()
        {
            if (Owner.IsPlayer && Owner.Session.AngryMode)
                return AnimationNames.MoveAngry;
            else
                return AnimationNames.Move;
        }
    }
}
