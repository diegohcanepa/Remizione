using Microsoft.Xna.Framework;

namespace Remizione
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
            Owner.Blinker.Start(50);
            cooldown = 600;
        }

        // Exit
        public override void Exit()
        {
            Owner.Blinker.Stop();
            base.Exit();
        }

        // CheckTransitions
        public override string? CheckTransitions()
        {
            if (Owner.AnimationPlayer.Animation != null && !Owner.AnimationPlayer.IsPlaying)
                return ActorStateNames.Stand;

            else if (cooldown <= 0)
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
