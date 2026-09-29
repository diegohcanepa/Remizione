using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ILightSource
    /// </summary>
    public interface ILightSource
    {
        // DrawLights
        void DrawLights(GameTime gameTime, LightBlendMode blendMode);

        // IsEmittingLight
        bool IsEmittingLight { get; }
    }
}
