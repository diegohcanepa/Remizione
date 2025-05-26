using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// RainEmitterType
    /// </summary>
    public class RainEmitterType : IEmitterType
    {
        private readonly Camera camera;

        // Constructor
        public RainEmitterType(Camera camera)
        {
            this.camera = camera;
        }

        // GetParticleDirection
        public Vector2 GetParticleDirection() => Vector2.One;

        // GetParticlePosition
        public Vector2 GetParticlePosition(Vector2 emitterPosition) => new(Randomizer.Next(camera.VisibleBox.Left, camera.VisibleBox.Right), camera.VisibleBox.Top - 15);
    }
}
