using Adberration;
using Engendro;
using Engendro.Input;

namespace Remizione.UI
{
    /// <summary>
    /// HUDComponent
    /// </summary>
    public abstract class HUDComponent : SessionGameObject<GameSession>
    {
        // Constructor
        protected HUDComponent(GameSession session)
            : base(session)
        {
        }

        // BoundingBox
        public RectangleF BoundingBox { get; protected set; }

        // IsMouseOver
        public bool IsMouseOver()
        {
            return BoundingBox.Contains(InputManager.DefaultPlayer.Mouse.VirtualPosition);
        }
    }
}
