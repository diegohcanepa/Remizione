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
            : base(owner, ActorStateNames.Hurt, false)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            cooldown = 600;
        }

        // CheckTransitions
        public override string? CheckTransitions()
        {
            if (cooldown <= 0)
                return ActorStateNames.Stand;

            else if (Owner.AnimationPlayer.Animation != null && !Owner.AnimationPlayer.IsPlaying)
                return ActorStateNames.Stand;

            else
                return base.CheckTransitions();
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (cooldown >= 0)
                cooldown -= gameTime.ElapsedGameTime.Milliseconds;
        }
    }
}
