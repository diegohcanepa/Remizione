using Microsoft.Xna.Framework;
using System;

namespace Engendro
{
    /// <summary>
    /// CircleEmitterType
    /// </summary>
    public class CircleEmitterType(float radius) : IEmitterType
    {
        // GetParticleDirection
        public Vector2 GetParticleDirection() => Vector2.Zero;

        // GetParticlePosition
        public Vector2 GetParticlePosition(Vector2 emitterPosition)
        {
            var newAngle = RandomHelper.Next(System.Random.Shared, 0, 2 * MathHelper.Pi);
            Vector2 positionVector = new((float)Math.Cos(newAngle), (float)Math.Sin(newAngle));

            positionVector.Normalize();

            var distance = RandomHelper.Next(System.Random.Shared, 0, Radius);
            var position = positionVector * distance;

            var x = emitterPosition.X + position.X;
            var y = emitterPosition.Y + position.Y;

            return new Vector2(x, y);
        }

        // Radius
        public float Radius { get; private set; } = radius;
    }
}
