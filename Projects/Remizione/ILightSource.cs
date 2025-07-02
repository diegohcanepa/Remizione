using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Remizione
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
