using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System;

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
        private Vector2 velocity;
        private readonly float weight;      // Masa relativa (afecta la gravedad)

        #endregion

        // Constructor
        public Dice(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Environment;
            //Scale = new(.5f);
            //ShadowOffset = new(0, -2);
            ShadowSpotSize = 0;

            this.initialVelocity = new Vector2(110, -50);
            this.weight = .8f;
            this.bounciness = .6f;
            this.gravity = 500;
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
                    if (bounceCount == 0)
                        Sound.Play(SoundNames.Dice);
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

        // Depth
        public override float Depth => depth;

        // IsRolling
        public bool IsRolling { get; private set; }

        // LastResult
        public int LastResult { get; private set; }

        // Roll
        [ScriptMethod]
        public int Roll()
        {
            owner = Session.Player;
            if (owner == null)
            {
                LastResult = 0;
                return 0;
            }

            this.bounceCount = 0;
            this.isGrounded = false;
            this.velocity = initialVelocity;
            this.X = owner.X;
            this.Y = owner.Y - 4;
            this.floorY = owner.Y;
            this.depth = owner.Depth - .1f;
            this.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.Linear, new Vector2(.2f), new Vector2(.6f), 150);

            if (owner.IsFlippedHorizontally)
                velocity.X *= -1;

            Sound.Play(SoundNames.WhooshA);

            owner.Room?.Children.Add(this);
            AnimationPlayer.Play("Roll", true, owner.Direction == Adberration.FacingDirection.Right ? AnimationDirection.Reverse : AnimationDirection.Forward);
            IsRolling = true;

            LastResult = DiceExpression.Dice6.Roll();

            return LastResult;
        }

        // Stop
        public void Stop()
        {
            if (LastResult > 0)
                AnimationPlayer.Play($"Number{LastResult}");

            IsRolling = false;
        }
    }
}
