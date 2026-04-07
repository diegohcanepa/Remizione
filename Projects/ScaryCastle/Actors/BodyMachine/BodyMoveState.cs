using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// BodyMoveState
    /// </summary>
    public sealed class BodyMoveState : BodyAnimatedState
    {
        // Constructor
        public BodyMoveState()
            : base(AnimationNames.Move, true)
        {
        }

        // GetAnimationName
        protected override string GetAnimationName()
        {
            if (Owner.CarriedProp == null)
                return base.GetAnimationName();
            else
                return AnimationNames.MoveCarry;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!Owner.IsMoving)
                Machine.ChangeState<BodyStandState>();
        }
    }
}
