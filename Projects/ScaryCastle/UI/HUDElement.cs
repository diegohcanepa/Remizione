using Adberration;
using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// HUDElement
    /// </summary>
    public abstract class GameObject<T> : GameObject where T : Session
    {
        // Constructor
        protected GameObject(T session)
        {
            this.Session = session;
        }

        // Session
        public T Session { get; }
    }
}
