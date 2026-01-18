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
        public ActorHurtState(Actor owner)
            : base(owner, ActorStateNames.Hurt, true)
        {
        }

        // CheckTransitions
        public override string? CheckTransitions()
        {
            if (cooldown <= 0)
                return ActorStateNames.Stand;
            else
                return base.CheckTransitions();
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
        }
    }
}
