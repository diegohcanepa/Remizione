using Microsoft.Xna.Framework;

namespace Remizione
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
            if (Owner.HeldProp != null)
                return AnimationNames.MoveCarry;

            else if (Owner.FastMove && Owner.ContainsAnimation(AnimationNames.MoveFast))
                return AnimationNames.MoveFast;

            else
                return base.GetAnimationName();
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
