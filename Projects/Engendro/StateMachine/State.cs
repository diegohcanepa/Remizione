using Microsoft.Xna.Framework;

namespace Engendro
{
    /// <summary>
    /// State
    /// </summary>
    public abstract class State<TOwner>
    {
        // Machine
        protected StateMachine<TOwner> Machine { get; private set; } = null!;

        // Owner
        public TOwner Owner => Machine.Owner;

        // Enter
        public virtual void Enter()
        {
        }

        // Exit
        public virtual void Exit()
        {
        }

        // HandleInput
        public virtual HandleInputResult HandleInput(GameTime gameTime)
        {
            return HandleInputResult.Unhandled;
        }

        // Initialize
        public void Initialize(StateMachine<TOwner> machine)
        {
            Machine = machine;
        }

        // Update
        public virtual void Update(GameTime gameTime)
        {
        }
    }
}