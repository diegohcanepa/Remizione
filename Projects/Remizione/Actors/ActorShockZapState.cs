using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ActorShockZapState
    /// </summary>
    public sealed class ActorShockZapState : ActorAnimatedState
    {
        private int cooldown;

        // Constructor
        public ActorShockZapState(Actor owner)
            : base(owner, ActorStateNames.ShockZap, true)
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
            Owner.PlaySound(SoundNames.ShockZap);
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            cooldown -= gameTime.ElapsedGameTime.Milliseconds;
        }
    }
}
