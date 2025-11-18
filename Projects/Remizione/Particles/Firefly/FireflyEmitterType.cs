using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// FireflyEmitterType
    /// </summary>
    public class FireflyEmitterType : IEmitterType
    {
        private readonly Camera camera;

        // Constructor
        public FireflyEmitterType(Camera camera)
        {
            this.camera = camera;
        }

        // Direction
        public Vector2 Direction { get; private set; }

        // GetParticleDirection
        public Vector2 GetParticleDirection() => new(Random.Shared.Next(-1, 2), Random.Shared.Next(-1, 2));

        // GetParticlePosition
        public Vector2 GetParticlePosition(Vector2 emitterPosition) => camera.VisibleBox.GetRandomPoint();
    }
}
