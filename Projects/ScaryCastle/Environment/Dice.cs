using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScaryCastle
{
    /// <summary>
    /// Dice
    /// </summary>
    public sealed class Dice : GameThing
    {
        #region Private fields

        private int bounceCount;
        private readonly float bounciness;  // Cuánto rebota (0=sin rebote, 1=rebotar igual de fuerte)
        private float depth;
        private float floorY;               // Cuánto se frena en horizontal al chocar
        private readonly float gravity;     // gravedad base
        private readonly float horizontalDamping = 0.7f; // cuánto se reduce X en cada golpe
        private readonly Vector2 initialVelocity;     // gravedad base
        private bool isGrounded;
        private readonly int maxBounces = 3;    // gravedad base
        private GameThing? owner;
        private readonly float radius;      // "tamaño" del objeto en píxeles
        private Vector2 velocity;
        private readonly float weight;      // Masa relativa (afecta la gravedad)

        #endregion

        // Constructor
        public Dice(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Environment;
            Scale = new(.5f);
            ShadowOffset = new(0, -2);
            ShadowSpotSize = 9;
            
            this.initialVelocity = new Vector2(110, -50);
            this.weight = .8f;
            this.bounciness = .6f;
            this.gravity = 500;
            this.radius = 10;
        }

        #region Private members

        // UpdateFloorCollision
        private void UpdateFloorCollision()
        {
            if (Position.Y >= floorY)
            {
                Position = new Vector2(Position.X, floorY);

                if (MathF.Abs(velocity.Y) > 10f && bounceCount < maxBounces)
                {
                    // Rebote vertical
                    velocity = new Vector2(velocity.X * horizontalDamping, -velocity.Y * bounciness);
                    bounceCount++;
                }
                else
                {
                    // Se queda quieto después de usar sus rebotes
                    velocity = Vector2.Zero;
                    DepthOffset = 0;
                    isGrounded = true;
                    Stop();
                }
            }
        }

        #endregion

        #region Protected members

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            Stop();
        }

        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Apply gravity
            if (!isGrounded)
                velocity += new Vector2(0, gravity * weight * dt);

            // Movement
            Position += velocity * dt;

            // Floor collision
            UpdateFloorCollision();
        }

        #endregion

        // IsRolling
        public bool IsRolling { get; private set; }

        // Roll
        [ScriptMethod]
        public void Roll()
        {
            owner = Session.Player;
            if (owner == null)
                return;

            this.bounceCount = 0;
            this.isGrounded = false;
            this.velocity = initialVelocity;
            this.X = owner.X;
            this.Y = owner.Y - owner.Height / 2 - this.radius;
            this.floorY = owner.Y;

            if (owner.IsFlippedHorizontally)
                velocity.X *= -1;

            owner.Room?.Children.Add(this);
            AnimationPlayer.Play("Roll", true, owner.Direction == Adberration.FacingDirection.Right ? AnimationDirection.Reverse : AnimationDirection.Forward);
            IsRolling = true;
        }

        // Stop
        public void Stop()
        {
            AnimationPlayer.Play("Idle");
            IsRolling = false;
        }
    }
}
