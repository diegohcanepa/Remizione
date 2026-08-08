using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// BodyMoveState
    /// </summary>
    public sealed class BodyMoveState : BodyAnimatedState
    {
        private Vector2 lastPosition;

        // Constructor
        public BodyMoveState()
            : base(AnimationNames.Move, true)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            lastPosition = Owner.Position;
            Owner.PixelsMoved = 0;
        }

        // GetAnimationName
        protected override string GetAnimationName()
        {
            if (Owner.ActiveThrowable == null)
                return base.GetAnimationName();
            else
                return AnimationNames.MoveCarry;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            var currentPosition = Owner.Position;

            base.Update(gameTime);

            if (currentPosition != lastPosition)
            {
                Owner.PixelsMoved += Vector2.Distance(lastPosition, currentPosition);
                lastPosition = currentPosition;
            }

            if (!Owner.IsMoving)
                Machine.ChangeState<BodyStandState>();
        }
    }
}
