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
        }

        // Exit
        public virtual void Exit()
        {
        }

        // GetSignal
        public virtual AIStateSignal GetSignal() => AIStateSignal.None;

        // Owner
        protected Actor Owner { get; }

        // Update
        public virtual void Update(GameTime gameTime)
        {
        }
    }
}
