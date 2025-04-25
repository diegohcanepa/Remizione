using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// AIState
    /// </summary>
    public abstract class AIState
    {
        // Constructor
        protected AIState(Actor owner)
        {
            this.Owner = owner;
        }

        // Enter
        public virtual void Enter()
        {
            Signal = AIStateSignal.None;
        }

        // Exit
        public virtual void Exit()
        {
        }

        // Owner
        protected Actor Owner { get; }

        // Signal
        public AIStateSignal Signal { get; protected set; }

        // Update
        public virtual void Update(GameTime gameTime)
        {
        }
    }
}
