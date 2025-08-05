using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    public class ShatterPiece : GameObject
    {
        private float bounceFactor = 0.6f;
        private float gravity = 300;
        private float groundY;
        private readonly ImageSprite image;
        private float life = 2;
        private Vector2 velocity;
        private float angularVelocity;

        private float launchDelay;
        private float delayTimer;
        private bool launched;

        public ShatterPiece(RemizioneGame game, AtlasImage image)
            : base(game)
        {
            this.image = new(game, image)
            {
                PivotOrigin = RectanglePoint.Middle
            };
        }

        private static float RandomBetween(float min, float max)
        {
            return (float)(Random.Shared.NextDouble() * (max - min) + min);
        }

        protected override void OnDraw(GameTime gameTime)
        {
            if (launched)
                image.Draw(gameTime);
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!launched)
            {
                delayTimer += dt;
                if (delayTimer >= launchDelay)
                {
                    // Velocidad más contenida
                    velocity = new(
                        RandomBetween(-30f, 30f),   // menos dispersión horizontal
                        RandomBetween(-20f, 10f)    // caída más natural
                    );
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
                velocity.X *= 0.7f;
                angularVelocity *= 0.7f;

                if (Math.Abs(velocity.Y) < 6f)
                    velocity.Y = 0;
            }

            life -= dt;

            image.Update(gameTime);
        }

        public bool IsDead => life <= 0;

        public void Launch(BreakableProp requester)
        {
            var bounds = requester.BoundingBox;

            // Offset vertical aleatorio para evitar que todas salgan alineadas en Y
            float yOffset = RandomBetween(-4f, 2f);

            image.Position = new(RandomBetween(bounds.Left + 2f, bounds.Right - 2f), 
                                RandomBetween(bounds.Top, bounds.Bottom) + yOffset);

            image.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.Linear, Vector2.One, new(.75f), 400);

            groundY = requester.Y + Randomizer.Next(-3, 3);

            launchDelay = RandomBetween(0f, 0.1f);
            delayTimer = 0f;
            launched = false;
        }
    }
}
