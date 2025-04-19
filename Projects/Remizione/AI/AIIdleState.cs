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
        private AIStateSignal signal;

        // Constructor
        public AIIdleState(Actor owner, float idleDuration = 2000)
            : base(owner)
        {
            this.idleDuration = idleDuration;
        }

        // Enter
        public override void Enter()
        {
            timer = 0;
            signal = AIStateSignal.None;
            Owner.StopMoving();
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.Milliseconds;

            if (Owner.CanSeePlayer())
            {
                signal = AIStateSignal.SawPlayer;
                return;
            }

            if (timer >= idleDuration)
                signal = AIStateSignal.IdleTimeout;
        }

        // GetSignal
        public override AIStateSignal GetSignal() => signal;
    }
}
