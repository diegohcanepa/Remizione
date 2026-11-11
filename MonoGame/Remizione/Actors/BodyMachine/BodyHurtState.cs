using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// BodyHurtState
    /// </summary>
    public sealed class BodyHurtState : BodyAnimatedState
    {
        private int cooldown;

        // Constructor
        public BodyHurtState()
            : base(AnimationNames.Hurt, true)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            cooldown = 300;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            cooldown -= gameTime.ElapsedGameTime.Milliseconds;
            if (cooldown <= 0)
                Machine.ChangeState<BodyStandState>();
        }
    }
}
