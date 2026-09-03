using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// DustEmitterType
    /// </summary>
    public class DustEmitterType : IEmitterType
    {
        private readonly Camera camera;

        // Constructor
        public DustEmitterType(Camera camera)
        {
            this.camera = camera;
        }

        // Direction
        public Vector2 Direction { get; private set; }

        // GetParticleDirection
        public Vector2 GetParticleDirection()
        {
            return new(Random.Shared.Next(-1, 2), Random.Shared.Next(-1, 2));
        }

        // GetParticlePosition
        public Vector2 GetParticlePosition(Vector2 emitterPosition)
        {
            return camera.VisibleBox.GetRandomPoint();
        }
    }
}
