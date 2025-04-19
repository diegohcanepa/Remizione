using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// DustParticleState
    /// </summary>
    public sealed class DustParticleState : ParticleState
    {
        private readonly GameSession session;

        // Constructor
        public DustParticleState(GameSession session)
            : base()
        {
            this.session = session;

            AddImages(Atlases.Environment.DustParticles);
        }

        // Acceleration
        public override Vector2 Acceleration => Vector2.One;

        // Color
        public override Color Color
        {
            get
            {
                if (session.Room != null)
                {
                    if (session.Room.DustParticleKind == DustParticleKind.Ash)
                        return Color.Black;
                }

                return base.Color;
            }
        }

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
        public override Vector2 Scale => new(Randomizer.Next(.4f, .9f));

        // Speed
        public override Vector2 Speed => new(Randomizer.Next(-8, 8), Randomizer.Next(-1, 1));
    }
}
