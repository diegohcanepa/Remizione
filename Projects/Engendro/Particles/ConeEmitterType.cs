using Microsoft.Xna.Framework;
using System;

namespace Engendro
{
    /// <summary>
    /// ConeEmitterType
    /// </summary>
    public class ConeEmitterType : IEmitterType
    {
        // Constructor
        public ConeEmitterType(Vector2 direction, float spread)
        {
            Direction = direction;
            Spread = spread;
        }

        // Direction
        public Vector2 Direction { get; private set; }

        // GetParticleDirection
        public Vector2 GetParticleDirection()
        {
            if (Direction == Vector2.Zero)
            {
                return Direction;
            }

            var angle = (float)Math.Atan2(Direction.Y, Direction.X);

            var newAngle = RandomHelper.Next(System.Random.Shared, angle - (Spread / 2.0f), angle + (Spread / 2.0f));

            Vector2 particleDirection = new((float)Math.Cos(newAngle), (float)Math.Sin(newAngle));

            particleDirection.Normalize();

            return particleDirection;
        }

        // GetParticlePosition
        public Vector2 GetParticlePosition(Vector2 emitterPosition)
        {
            return new(emitterPosition.X, emitterPosition.Y);
        }

        // Spread
        public float Spread { get; private set; }
    }
}
