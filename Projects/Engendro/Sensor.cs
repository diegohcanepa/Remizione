using Microsoft.Xna.Framework;

namespace Engendro
{
    /// <summary>
    /// Sensor
    /// </summary>
    public abstract class Sensor<T>(T owner)
    {
        #region Protected members

        // OnUpdate
        protected virtual void OnUpdate(GameTime gameTime)
        {
        }

        #endregion

        // IsTriggered
        public virtual bool IsTriggered() => false;

        // Owner
        public T Owner { get; } = owner;

        // Update
        public void Update(GameTime gameTime) => OnUpdate(gameTime);
    }
}
