using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// ActorHurtState
    /// </summary>
    public sealed class ActorHurtState : ActorAnimatedState
    {
        private int cooldown;

        // Constructor
        public ActorHurtState()
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
                Machine.ChangeState<ActorStandState>();
        }
    }
}
