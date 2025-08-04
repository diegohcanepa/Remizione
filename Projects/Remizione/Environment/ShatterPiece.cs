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
        public float bounceFactor = .5f;
        private float gravity = 600;
        private float groundY;
        private readonly ImageSprite image;
        private bool launched;
        private bool tweened;
        public float life = 2;
        private Vector2 velocity;

        // Constructor
        public ShatterPiece(RemizioneGame game, AtlasImage image)
            : base(game)
        {
            this.image = new(game, image)
            {
                PivotOrigin = RectanglePoint.Bottom
            };
        }

        // RandomBetween
        private static float RandomBetween(float min, float max)
        {
            return (float)(Random.Shared.NextDouble() * (max - min) + min);
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!launched)
                return;

            image.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (!launched)
                return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Aplicar gravedad
            velocity.Y += gravity * dt;

            // Mover
            image.X += velocity.X * dt;
            image.Y += velocity.Y * dt;

            // Rotar
            image.Rotation += velocity.X * .01f;

            // Rebotar si toca el suelo
            if (image.Y >= groundY)
            {
                image.Y = groundY;
                velocity.Y *= -bounceFactor;

                // Fricción horizontal opcional
                velocity.X *= .8f;

                // Si el rebote es muy chico, parar
                if (Math.Abs(velocity.Y) < 50)
                    velocity.Y = 0;
            }

            // Contar vida
            life -= dt;

            if (IsDead && !tweened)
            {
                tweened = true;
                image.Tweens.OpacityTween = FloatTween.Create(TweenStyle.Linear, 1, 0, 200);
            }

            image.Update(gameTime);
        }

        #endregion

        // IsDead
        public bool IsDead => life <= 0;

        // Launch
        public void Launch(BreakableProp requester)
        {
            image.Position = requester.BoundingBox.GetPoint(RectanglePoint.LeftTop);
            image.Rotation = RandomBetween(-1f, 1f);
            groundY = requester.Y;

            // Velocidad aleatoria
            velocity = new Vector2(
                RandomBetween(-5f, 5f),
                RandomBetween(-20f, 0f)
            );

            launched = true;
        }
    }
}
