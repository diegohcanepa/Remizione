using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// ShatterPiece
    /// </summary>
    public class ShatterPiece : GameObject
    {
        #region Private fields

        private float angularVelocity;
        private const float bounceFactor = .8f;
        private float delayTimer;
        private const float gravity = 350;
        private float groundY;
        private readonly ImageSprite image;
        private float launchDelay;
        private bool launched;
        private float life = 2;
        private readonly GameThing owner;
        private Vector2 velocity;

        #endregion

        #region Constructor

        // Constructor
        public ShatterPiece(GameThing owner, AtlasImage image)
            : base(owner.Game)
        {
            this.owner = owner;

            this.image = new(Game, image)
            {
                PivotOrigin = RectanglePoint.Center
            };
        }

        #endregion

        #region Private members

        // RandomBetween
        private static float RandomBetween(float min, float max)
        {
            return (float)(Random.Shared.NextDouble() * (max - min) + min);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (launched)
                image.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!launched)
            {
                delayTimer += dt;
                if (delayTimer >= launchDelay)
                {
                    velocity = new(RandomBetween(-30f, 30f), RandomBetween(-20f, 10f));
                    angularVelocity = RandomBetween(-5f, 5f);
                    launched = true;
                }

                return;
            }

            velocity.Y += gravity * dt;

            image.X += velocity.X * dt;
            image.Y += velocity.Y * dt;

            image.Rotation += angularVelocity * dt;

            if (image.Y >= groundY)
            {
                image.Y = groundY;
                velocity.Y *= -bounceFactor;
                velocity.X *= .7f;
                angularVelocity *= .7f;

                if (Math.Abs(velocity.Y) < 6f)
                    velocity.Y = 0;
            }

            life -= dt;

            image.Update(gameTime);
        }

        #endregion

        // Launch
        public void Launch()
        {
            var bounds = owner.BoundingBox;
            float yOffset = RandomBetween(-4f, 2f);

            image.Position = new(RandomBetween(bounds.Left + 5f, bounds.Right - 5f),
                                RandomBetween(bounds.Top, bounds.Bottom) + yOffset);

            groundY = owner.Y + Randomizer.Next(-3, 3);
            launchDelay = RandomBetween(0, .1f);
            delayTimer = 0;
            launched = false;
        }

        // Opacity
        public float Opacity
        {
            get => image.Opacity;
            set => image.Opacity = value;
        }

        // Scale
        public Vector2 Scale
        {
            get => image.Scale;
            set => image.Scale = value;
        }
    }
}
