using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ActorContactDamageState
    /// </summary>
    public sealed class ActorContactDamageState : ActorAnimatedState
    {
        private int cooldown;

        // Constructor
        public ActorContactDamageState(Actor owner)
            : base(owner, ActorStateNames.ContactDamage, true)
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
