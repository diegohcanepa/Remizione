using Microsoft.Xna.Framework;

namespace Engendro.Input
{
    /// <summary>
    /// InputDevice
    /// </summary>
    public abstract class InputDevice(PlayerInputManager player)
    {
        #region Protected members

        // CanUpdate
        protected virtual bool CanUpdate => true;

        // OnUpdate
        protected virtual void OnUpdate(GameTime gameTime)
        {
        }

        #endregion

        #region Internal members

        // Update
        internal void Update(GameTime gameTime)
        {
            if (CanUpdate)
                OnUpdate(gameTime);
        }

        #endregion

        // HasInput
        public abstract bool HasInput();

        // Player
        public PlayerInputManager Player { get; } = player;

        // Reset
        public virtual void Reset()
        {
        }
    }
}
