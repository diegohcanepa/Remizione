using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// HUDElement
    /// </summary>
    public abstract class HUDElement : GameObject
    {
        // Constructor
        protected HUDElement(GameSession session)
        {
            this.Session = session;
        }

        // Session
        public GameSession Session { get; }
    }
}
