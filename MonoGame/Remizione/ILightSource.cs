using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ILightSource
    /// </summary>
    public interface ILightSource
    {
        // BoundingBox
        RectangleF BoundingBox { get; }

        // DrawLights
        void DrawLights(GameTime gameTime, LightBlendMode blendMode);

        // IsEmittingLight
        bool IsEmittingLight { get; }
    }
}
