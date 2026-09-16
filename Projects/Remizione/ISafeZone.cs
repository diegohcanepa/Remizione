using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Remizione
{
    /// <summary>
    /// ISafeZone
    /// </summary>
    public interface ISafeZone
    {
        Vector2 Center { get; }
        float Radius { get; }
    }
}
