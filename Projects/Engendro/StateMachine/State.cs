using Engendro.Input;
using Microsoft.Xna.Framework;

namespace Engendro
{
    /// <summary>
    /// State
    /// </summary>
    public abstract class State<T> : IInputHandler
    {
        // Constructor
        protected State(T owner, string name)
        {
            CodeContract.NotEmpty(name, nameof(name));
            this.Owner = owner;
            this.Name = name;
        }

        #region Protected members

        // OnHandleInput
        protected virtual HandleInputResult OnHandleInput(GameTime gameTime)
        {
            return HandleInputResult.Unhandled;
        }

        #endregion

        // CheckTransitions
        public virtual string? CheckTransitions()
        {
            return null;
        }

        // Enter
        public virtual void Enter()
        {
        }

        // Exit
        public virtual void Exit()
        {
        }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime) => HandleInputResult.Unhandled;

        // Name
        public string Name { get; }

        // Owner
        public T Owner { get; }

        // Update
        public virtual void Update(GameTime gameTime)
        {
        }
    }
}
