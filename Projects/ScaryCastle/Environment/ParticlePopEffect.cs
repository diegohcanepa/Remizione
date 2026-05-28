using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// ParticlePopEffect
    /// </summary>
    public class ParticlePopEffect : GameObject
    {
        #region Private fields

        private Color color;
        private readonly List<Particle> particles = [];
        private AtlasImage renderImage = Atlases.UI.Pixel;

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            foreach (var p in particles)
            {
                float alpha = MathHelper.Clamp(p.Life / p.MaxLife, 0f, 1f);
                var origin = new Vector2(renderImage.TextureArea.Width / 2, renderImage.TextureArea.Height / 2);
                Game.SpriteBatch.Draw(renderImage.Atlas.Texture, p.Position, renderImage.TextureArea, color * alpha, 0, origin, Scale, SpriteEffects.None, 0);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = particles.Count - 1; i >= 0; i--)
            {
                var p = particles[i];
                p.Life -= dt;
                if (p.Life <= 0)
                {
                    particles.RemoveAt(i);
                }
                else
                {
                    p.Velocity.Y += Gravity * dt;
                    p.Position += p.Velocity * dt;
                    particles[i] = p;
                }
            }
        }

        #endregion

        // BurstSize
        public Int32Range BurstSize { get; set; } = new(3, 6);

        // Gravity
        public float Gravity { get; set; } = 150;

        // HorizontalSpeed
        public float HorizontalSpeed { get; set; } = 30;

        // IsActive
        public bool IsActive => particles.Count > 0;

        // ParticleLifetime
        public float ParticleLifetime { get; set; } = .25f;

        // Scale
        public float Scale { get; set; } = 1;

        // Spawn
        public void Spawn(Vector2 position, Color splashColor)
        {
            particles.Clear();

            color = splashColor;
            int count = BurstSize.GetRandomValue(Random.Shared);

            for (int i = 0; i < count; i++)
            {
                var particle = new Particle();

                float vx = (float)((Random.Shared.NextDouble() * 2) - 1) * HorizontalSpeed;
                float vy = -(float)((Random.Shared.NextDouble() * VerticalSpeed.Delta) + VerticalSpeed.Minimum);

                particle.Position = position;
                particle.Velocity = new Vector2(vx, vy);
                particle.Life = ParticleLifetime;
                particle.MaxLife = ParticleLifetime;

                particles.Add(particle);
            }
        }

        // VerticalSpeed
        public FloatRange VerticalSpeed { get; set; } = new(25, 35);

        /// <summary>
        /// Particle
        /// </summary>
        private struct Particle
        {
            public float Life;
            public float MaxLife;
            public Vector2 Position;
            public Vector2 Velocity;
        }
    }
}
