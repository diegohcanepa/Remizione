using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// ILightSource
    /// </summary>
    public interface ILightSource
    {
        // DrawLights
        void DrawLights(GameTime gameTime);

        // IsEmittingLight
        bool IsEmittingLight { get; }
    }
}
