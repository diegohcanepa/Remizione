using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// ActorMoveState
    /// </summary>
    public sealed class ActorMoveState : ActorAnimatedState
    {
        // Constructor
        public ActorMoveState()
            : base(AnimationNames.Move, true)
        {
        }

        // GetAnimationName
        protected override string GetAnimationName()
        {
            if (Owner.IsPlayer && Owner.Session.AngryMode)
                return AnimationNames.MoveAngry;
            else
                return base.GetAnimationName();
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!Owner.IsMoving)
                Machine.ChangeState<ActorStandState>();
        }
    }
}
