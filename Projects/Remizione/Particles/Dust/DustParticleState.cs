using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// DustParticleState
    /// </summary>
    public sealed class DustParticleState : ParticleState
    {
        // Constructor
        public DustParticleState()
            : base()
        {
            AddImages(Atlases.Environment.DustParticles);
        }

        // Acceleration
        public override Vector2 Acceleration => Vector2.One;

        // Color
        public override Color Color => Color.Black;

        // Gravity
        public override Vector2 Gravity => Vector2.Zero;

        // MinLifespan
        public override int MinLifespan => 1000;

        // MaxLifespan
        public override int MaxLifespan => 4000;

        // Opacity
        public override float Opacity => .6f;

        // OpacityDeviation
        public override float OpacityDeviation => .2f;

        // RotationSpeed
        public override float RotationSpeed => 2;

        // Scale
        public override Vector2 Scale => new(RandomHelper.Next(Random.Shared, .4f, .9f));

        // Speed
        public override Vector2 Speed => new(Random.Shared.Next(-8, 9), Random.Shared.Next(-1, 2));
    }
}