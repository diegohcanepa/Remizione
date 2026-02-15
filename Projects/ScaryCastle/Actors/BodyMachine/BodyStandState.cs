using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// BodyStandState
    /// </summary>
    public sealed class BodyStandState : BodyAnimatedState
    {
        private int idleCooldown;

        // Constructor
        public BodyStandState()
            : base(AnimationNames.Stand, true)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            idleCooldown = 15000;
            Owner.StopMoving();
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (idleCooldown >= 0)
                idleCooldown -= gameTime.ElapsedGameTime.Milliseconds;

            /*
            if (Owner.IsPlayer && idleCooldown < 0)
                Machine.ChangeState<ActorIdleState>();
            */
        }
    }
}
