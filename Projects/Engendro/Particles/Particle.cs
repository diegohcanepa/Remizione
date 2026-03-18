using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engendro
{
    /// <summary>
    /// Particle
    /// </summary>
    public class Particle() : GameObject
    {
        #region Private members

        // UpdatePosition
        private void UpdatePosition(float totalSeconds)
        {
            Velocity += Acceleration * totalSeconds;
            Position += Velocity * totalSeconds;
        }

        #endregion

        #region Protected members

        // OnActivate
        protected virtual void OnActivate()
        {
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (Image != null)
            {
                var origin = new Vector2(Image.TextureArea.Width / 2f, Image.TextureArea.Height / 2f);
                Game.SpriteBatch.Draw(Image.Atlas.Texture, Position, Image.TextureArea, Color * Opacity, Rotation, origin, Scale, SpriteEffects.None, 0);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (!IsActive)
                return;

            var totalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            UpdatePosition(totalSeconds);

            Rotation += RotationSpeed * totalSeconds;

            Age += gameTime.ElapsedGameTime.Milliseconds;

            if (Age >= Lifespan)
            {
                IsActive = false;
            }
        }

        #endregion

        // Activate
        public void Activate(ParticleState particleState, Vector2 emitterPosition, IEmitterType emitterType)
        {
            Acceleration = particleState.Acceleration;
            Age = 0;
            Color = particleState.Color;
            Image = particleState.GetImage();
            Lifespan = particleState.GenerateLifespan();
            Opacity = particleState.GenerateOpacity();
            Rotation = particleState.GenerateRotation();
            RotationSpeed = particleState.RotationSpeed;
            Position = emitterType.GetParticlePosition(emitterPosition);
            Scale = particleState.GenerateScale();
            Velocity = particleState.GenerateSpeed() * emitterType.GetParticleDirection();

            IsActive = true;

            OnActivate();
        }

        // Acceleration
        public Vector2 Acceleration { get; private set; }

        // Age
        public int Age { get; private set; }

        // Color
        public Color Color { get; private set; }

        // Image
        public AtlasImage? Image { get; private set; }

        // IsActive
        public bool IsActive { get; private set; }

        // Lifespan
        public int Lifespan { get; private set; }

        // Opacity
        public float Opacity { get; protected set; }

        // Position
        public Vector2 Position { get; private set; }

        // Rotation
        public float Rotation { get; private set; }

        // RotationSpeed
        public float RotationSpeed { get; private set; }

        // Scale
        public Vector2 Scale { get; private set; }

        // Velocity
        public Vector2 Velocity { get; private set; }
    }
}
