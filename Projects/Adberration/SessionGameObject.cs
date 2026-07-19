using Engendro;

namespace Adberration
{
    /// <summary>
    /// SessionGameObject
    /// </summary>
    public abstract class SessionGameObject<T> : GameObject where T : Session
    {
        // Constructor
        protected SessionGameObject(T session)
        {
            this.Session = session;
        }

        // Session
        public T Session { get; }
    }
}
