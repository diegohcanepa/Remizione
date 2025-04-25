using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// AIIdleState
    /// </summary>
    public sealed class AIIdleState : AIState
    {
        private readonly float idleDuration;
        private int timer;

        // Constructor
        public AIIdleState(Actor owner, float idleDuration = 2000)
            : base(owner)
        {
            this.idleDuration = idleDuration;
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            timer = 0;
            Owner.StopMoving();
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.Milliseconds;

            if (Owner.CanSeeTarget())
            {
                Signal = AIStateSignal.SawTarget;
                return;
            }

            if (timer >= idleDuration)
                Signal = AIStateSignal.IdleTimeout;
        }
    }
}
