using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ISafeZone
    /// </summary>
    public interface ISafeZone
    {
        Vector2 Center { get; }
        bool IsEnabled { get; }
        float Radius { get; }
    }
}
