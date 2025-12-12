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
        private const float gravity = 400;
        private float groundY;
        private readonly ImageSprite image;
        private float launchDelay;
        private bool launched;
        private float life = 2;
        private GameRoom? room;
        private static readonly Color shadowColor = Color.Black * .3f;
        private static readonly Vector2 shadowOffset = new(.5f);
        private Vector2 velocity;

        #endregion

        #region Constructor

        // Constructor
        public ShatterPiece(RemizioneGame game, AtlasImage image, Vector2 scale)
            : base(game)
        {
            this.image = new(Game, image)
            {
                PivotOrigin = RectanglePoint.Center,
                Scale = scale
            };
        }

        #endregion

        #region Private members

        // RandomBetween
        private static float RandomBetween(float min, float max)
        {
            return (float)((Random.Shared.NextDouble() * (max - min)) + min);
        }

        #endregion

        #region Protected members

        // OnDraw
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

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!launched)
            {
                delayTimer += dt;
                if (delayTimer >= launchDelay)
                {
                    velocity = new(RandomBetween(-35f, 35f), RandomBetween(-20f, 10f));
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

            CheckWalkAreaCollision();

            life -= dt;


            image.Update(gameTime);
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
            float yOffset = RandomBetween(-4f, 2f);

            image.Position = new(RandomBetween(bounds.Left + 5f, bounds.Right - 5f),
                                RandomBetween(bounds.Top, bounds.Bottom) + yOffset);

            groundY = owner.Y + Random.Shared.Next(-3, 4);
            launchDelay = RandomBetween(0, .1f);
            delayTimer = 0;
            room = owner.Session.Room;
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
