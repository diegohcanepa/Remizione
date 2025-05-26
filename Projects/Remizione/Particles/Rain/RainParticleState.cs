using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// RainParticleState
    /// </summary>
    public sealed class RainParticleState : ParticleState
    {
        private readonly Weather weather;

        // Constructor
        public RainParticleState(Weather weather)
            : base()
        {
            this.weather = weather;

            for (int i = 0; i < Atlases.Environment.RainParticles.Count; i++)
            {
                AddImage(Atlases.Environment.RainParticles[i]);
            }
        }

        // Acceleration
        public override Vector2 Acceleration => Randomizer.Next(new Vector2(30), new Vector2(60));

        // Gravity
        public override Vector2 Gravity => Vector2.Zero;

        // MinLifespan
        public override int MinLifespan => 900;

        // MaxLifespan
        public override int MaxLifespan => 1400;

        // Opacity
        public override float Opacity => .3f * weather.Intensity;

        // Scale
        public override Vector2 Scale { get; } = new Vector2(Randomizer.Next(.2f, .35f));

        // Velocity
        public override Vector2 Speed => new(150, 300);
    }
}
