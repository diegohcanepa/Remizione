using Engendro;

namespace Remizione
{
    /// <summary>
    /// SessionOverlayScene
    /// </summary>
    public abstract class SessionOverlayScene(GameSession session) : Scene
    {
        // Session
        protected GameSession Session { get; } = session;
    }
}
