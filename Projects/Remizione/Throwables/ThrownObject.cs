using Microsoft.Xna.Framework;
using System;

namespace Remizione.Throwables
{
    /// <summary>
    /// ThrownObject
    /// </summary>
    public abstract class ThrownObject : GameThing
    {
        private readonly float bounciness;  // Cuánto rebota (0=sin rebote, 1=rebotar igual de fuerte)
        private float floorY;               // Cuánto se frena en horizontal al chocar
        private readonly float friction;    // Cuánto se frena en horizontal al chocar
        private readonly float gravity;     // gravedad base
        private readonly float radius;      // "tamaño" del objeto en píxeles
        private Vector2 velocity;
        private readonly float weight;      // Masa relativa (afecta la gravedad)

        // Constructor
        public ThrownObject(GameSession session, float weight, float bounciness, float friction, float gravity, float radius)
            : base(session, string.Empty)
        {
            this.weight = weight;
            this.bounciness = bounciness;
            this.friction = friction;
            this.gravity = gravity;
            this.radius = radius;
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Gravedad ajustada por el "peso"
            if (!IsGrounded)
                velocity += new Vector2(0, gravity * weight * dt);

            // Mover posición
            Position += velocity * dt;

            // Rotación ligada a velocidad horizontal
            Rotation += (velocity.X / radius) * dt;

            // ====== Colisión con suelo ======
            if (Position.Y >= floorY)
            {
                Position = new Vector2(Position.X, floorY);

                if (Math.Abs(velocity.Y) > 10f) // margen para decidir si rebota
                {
                    // Rebote vertical según "elasticidad"
                    velocity = new Vector2(velocity.X, -velocity.Y * bounciness);

                    // Fricción horizontal
                    if (velocity.X > 0)
                        velocity = new Vector2(MathF.Max(0, velocity.X - friction * dt), velocity.Y);
                    else if (velocity.X < 0)
                        velocity = new Vector2(MathF.Min(0, velocity.X + friction * dt), velocity.Y);
                }
                else
                {
                    // Si ya no tiene energía suficiente -> se queda en el suelo
                    velocity = Vector2.Zero;
                    IsGrounded = true;
                }
            }

            // ====== Colisión con otros objetos ======
            // Aquí iría tu sistema de colisiones
            // if (CheckCollisionWithSomething(Position)) { 
            //     velocity = new Vector2(-velocity.X, velocity.Y); 
            // }
        }

        #endregion

        // IsGrounded
        public bool IsGrounded { get; private set; }

        // Launch
        public void Launch(Vector2 startPosition, Vector2 initialVelocity, float floorLevel)
        {
            this.IsGrounded = false;
            this.Position = startPosition;
            this.Rotation = 0;
            this.velocity = initialVelocity;
            this.floorY = floorLevel;
        }
    }
}
