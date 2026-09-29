using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ILightSource
    /// </summary>
    public interface ILightSource
    {
        // BlendMode
        public LightBlendMode BlendMode { get; }

        // DrawLights
        void DrawLights(GameTime gameTime);

        // IsEmittingLight
        bool IsEmittingLight { get; }
    }
}
