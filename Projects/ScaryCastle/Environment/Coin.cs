using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// Coin
    /// </summary>
    public class Coin : Prop
    {
        #region Private fields

        private float angularVelocity;
        private const float bounceFactor = .8f;
        private bool collected;
        private float delayTimer;
        private const float gravity = 700;
        private float groundY;
        private readonly ImageSprite image;
        private float launchDelay;
        private bool launched;
        private float life = 2;
        private readonly Vector2Tween scaleTween = new();
        private Vector2 velocity;

        #endregion

        #region Constructor

        // Constructor
        public Coin(GameSession session)
            : base(session, string.Empty)
        {
            this.image = new(Game, Atlases.Environment.Coin)
            {
                PivotOrigin = RectanglePoint.Center,
                Scale = new(.75f)
            };
        }

        #endregion

        #region Private members

        // RandomBetween
        private static float RandomBetween(float min, float max)
        {
            return (float)((Random.Shared.NextDouble() * (max - min)) + min);
        }

        // Release
        private void Release()
        {
            Unparent();
            Session.ObjectPools.Coins.Return(this);
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
                    velocity = new(RandomBetween(-115, 115), RandomBetween(-30f, 20f));
                    angularVelocity = RandomBetween(-5, 5);
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

            if (!collected)
            {
                if (Session.Player?.DistanceTo(image.Position) <= 3)
                {
                    collected = true;
                    Sound.Play(SoundNames.PickupCoin);
                    Session.Coins++;
                    scaleTween.Start(TweenStyle.Linear, image.Scale, Vector2.Zero, 100, Release);
                    image.Tweens.ScaleTween = scaleTween;
                }
            }
        }

        #endregion

        // Drop
        public void Drop(GameRoom room, Vector2 origin)
        {
            float yOffset = RandomBetween(-4f, 2f);

            image.Position = new(RandomBetween(origin.X - 15, origin.X + 15f), origin.Y + yOffset);
            groundY = origin.Y + Random.Shared.Next(-5, 4);
            launchDelay = RandomBetween(0, .1f);
            delayTimer = 0;
            launched = false;
            collected = false;

            room.Children.Add(this);
        }
    }
}
