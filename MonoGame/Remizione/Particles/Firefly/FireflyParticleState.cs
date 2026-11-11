using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// FireflyParticleState
    /// </summary>
    public sealed class FireflyParticleState : ParticleState
    {
        private readonly Vector2 acceleration;
        private readonly Color[] colors = [new(250, 248, 90), new(222, 250, 90), new(198, 255, 0)];
        private readonly Vector2 scale;
        private readonly Vector2 speed;

        // Constructor
        public FireflyParticleState()
            : base()
        {
            AddImages(Atlases.Environment.FireflyParticles);

            scale = new Vector2(.5f);

            speed.X = RandomHelper.Next(Random.Shared, 2.5f, 5);
            if (DiceExpression.Dice10.Roll() <= 5)
                speed.X *= -1;

            speed.Y = Random.Shared.Next(1, 4);
            if (DiceExpression.Dice10.Roll() <= 5)
                speed.Y *= -1;

            acceleration.X = Math.Abs(speed.X / 2.5f);
            if (speed.X > 0)
                acceleration.X *= -1;

            acceleration.Y = Math.Abs(speed.Y / 2.5f);
            if (speed.Y > 0)
                acceleration.Y *= -1;
        }

        // Acceleration
        public override Vector2 Acceleration => acceleration;

        // Color
        public override Color Color => colors[Random.Shared.Next(colors.Length)];

        // Gravity
        public override Vector2 Gravity => Vector2.Zero;

        // MinLifespan
        public override int MinLifespan => 500;

        // MaxLifespan
        public override int MaxLifespan => 1500;

        // Opacity
        public override float Opacity => .7f;

        // Scale
        public override Vector2 Scale => scale;

        // Speed
        public override Vector2 Speed => speed;
    }
}
