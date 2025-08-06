using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// FootstepEffect
    /// </summary>
    public class FootstepEffect : GameObject
    {
        #region Private fields

        private Color color;
        private readonly float gravity = 150;
        private readonly float horizontalSpeed = 30;
        private readonly float particleLifetime = .25f;
        private readonly List<Particle> particles = [];
        private readonly Texture2D pixel;
        private static readonly ObjectPool<Particle> pool = new ObjectPool<Particle>(() => new Particle(), 100, 50);
        private static readonly Random random = new();
        private readonly float verticalSpeedMin = 25;
        private readonly float verticalSpeedMax = 35;

        #endregion

        // Constructor
        public FootstepEffect(EngendroGame game)
            : base(game)
        {
            pixel = new Texture2D(Game.GraphicsDevice, 1, 1);
            pixel.SetData([Color.White]);
        }

        #region Private members

        // CleanUp
        private void CleanUp()
        {
            for (var i = 0; i < particles.Count; i++)
            {
                pool.Return(particles[i]);
            }

            particles.Clear();
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            foreach (var p in particles)
            {
                float alpha = MathHelper.Clamp(p.Life / p.MaxLife, 0f, 1f);
                Game.SpriteBatch.Draw(pixel, p.Position, null, color * alpha, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
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
                    pool.Return(p);
                }
                else
                {
                    p.Velocity.Y += gravity * dt;
                    p.Position += p.Velocity * dt;
                    particles[i] = p;
                }
            }
        }

        #endregion

        // IsActive
        public bool IsActive => particles.Count > 0;

        // Spawn
        public void Spawn(Vector2 position, Color splashColor)
        {
            CleanUp();

            color = splashColor;
            int count = random.Next(3, 6);

            for (int i = 0; i < count; i++)
            {
                if (pool.Get() is Particle particle)
                {
                    float vx = (float)(random.NextDouble() * 2 - 1) * horizontalSpeed;
                    float vy = -(float)(random.NextDouble() * (verticalSpeedMax - verticalSpeedMin) + verticalSpeedMin);

                    particle.Position = position;
                    particle.Velocity = new Vector2(vx, vy);
                    particle.Life = particleLifetime;
                    particle.MaxLife = particleLifetime;

                    particles.Add(particle);
                }
            }
        }

        /// <summary>
        /// Particle
        /// </summary>
        private sealed class Particle
        {
            public float Life;
            public float MaxLife;
            public Vector2 Position;
            public Vector2 Velocity;
        }
    }
}
