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

        // Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!Owner.IsMoving)
                Machine.ChangeState<BodyStandState>();
        }
    }
}
