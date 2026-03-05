using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
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
        private const float gravity = 400;
        private float groundY;
        private readonly Sprite image;
        private float launchDelay;
        private bool launched;

        // OPTIMIZACIÓN: Flag para saber si ya se detuvo
        private bool isStopped;

        private GameRoom? room;
        private static readonly Color shadowColor = Color.Black * .3f;
        private static readonly Vector2 shadowOffset = new(.5f);
        private Vector2 velocity;

        #endregion

        #region Constructor

        public ShatterPiece(EngendroGame game, AtlasImage image, Vector2 scale)
            : base(game)
        {
            this.image = new(Game, image)
            {
                PivotOrigin = RectanglePoint.Center,
                Scale = scale
            };
        }

        #endregion

        #region Protected members

        protected override void OnDraw(GameTime gameTime)
        {
            if (launched)
            {
                var c = image.Color;

                image.Position += shadowOffset;
                image.Color = shadowColor;
                image.Draw(gameTime);
                image.Color = c;
                image.Position -= shadowOffset;

                image.Draw(gameTime);
            }
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            // OPTIMIZACIÓN: Si ya se detuvo, no calculamos nada más.
            if (isStopped) return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!launched)
            {
                delayTimer += dt;
                if (delayTimer >= launchDelay)
                {
                    velocity = new(Random.Shared.Next(-35f, 35f), Random.Shared.Next(-20f, 10f));
                    angularVelocity = Random.Shared.Next(-5f, 5f);
                    launched = true;
                }
                return;
            }

            velocity.Y += gravity * dt;

            // Guardamos X anterior para evitar tunneling (el fix anterior)
            float previousX = image.X;

            image.X += velocity.X * dt;
            image.Y += velocity.Y * dt;

            image.Rotation += angularVelocity * dt;

            // Lógica de suelo y detención
            if (image.Y >= groundY)
            {
                image.Y = groundY;
                velocity.Y *= -bounceFactor;
                velocity.X *= .7f;      // Fricción del suelo
                angularVelocity *= .7f; // Fricción de rotación

                // Si el rebote vertical es muy pequeño, lo anulamos
                if (Math.Abs(velocity.Y) < 6f)
                {
                    velocity.Y = 0;
                }

                // OPTIMIZACIÓN: Chequeo de detención total
                // Si no rebota en Y, y la velocidad en X es casi nula (menor a 1 pixel/segundo)
                if (velocity.Y == 0 && Math.Abs(velocity.X) < 1f)
                {
                    velocity = Vector2.Zero;
                    angularVelocity = 0;
                    isStopped = true; // Dejamos de actualizar desde el próximo frame
                }
            }

            // Chequeo de WalkArea (solo si no se ha detenido aún)
            if (!isStopped && CheckWalkAreaCollision())
            {
                image.X = previousX;
            }

            // Solo actualizamos el sprite si se mueve o anima
            if (!isStopped)
            {
                image.Update(gameTime);
            }
        }

        #endregion

        // CheckWalkAreaCollision
        private bool CheckWalkAreaCollision()
        {
            if (room?.WalkArea is WalkArea walkArea)
            {
                if (image.Y >= walkArea.Polygon.BoundingRectangleF.Top && !walkArea.Contains(image.Position))
                {
                    velocity = new Vector2(-velocity.X, velocity.Y) * RandomHelper.Next(Random.Shared, .2f, .5f);
                    return true;
                }
            }
            return false;
        }

        // Launch
        public void Launch(GameThing owner)
        {
            var bounds = owner.BoundingBox;
            float yOffset = Random.Shared.Next(-4f, 2f);

            image.Position = new(Random.Shared.Next(bounds.Left + 5f, bounds.Right - 5f),
                                Random.Shared.Next(bounds.Top, bounds.Bottom) + yOffset);

            groundY = owner.Y + Random.Shared.Next(-3, 4);
            launchDelay = Random.Shared.Next(0, .1f);
            delayTimer = 0;
            room = owner.Session.Room;
            launched = false;

            // Reiniciamos el estado para que pueda volver a moverse si se relanza
            isStopped = false;
        }

        // Propiedades...
        public float Opacity
        {
            get => image.Opacity;
            set => image.Opacity = value;
        }

        public Vector2 Scale
        {
            get => image.Scale;
            set => image.Scale = value;
        }
    }
}