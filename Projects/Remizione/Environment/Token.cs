using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// Token
    /// </summary>
    public class Token : Prop
    {
        #region Private fields

        private float angularVelocity;
        private const float bounceFactor = .8f;
        private float delayTimer;
        private const float gravity = 700;
        private float groundY;
        private readonly ImageSprite image;
        private float launchDelay;
        private bool launched;
        private float life = 2;
        private Vector2 velocity;

        #endregion

        #region Constructor

        // Constructor
        public Token(GameSession session)
            : base(session, string.Empty)
        {
            this.image = new(Game, Atlases.Environment.Token)
            {
                PivotOrigin = RectanglePoint.Center,
            };

            this.image.Tweens.OpacityTween = Tweens.OpacityTween = FloatTween.Create(TweenStyle.Linear, .8f, .6f, 80, -1);
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
                    velocity = new(RandomBetween(-65f, 65f), RandomBetween(-30f, 20f));
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

            if (Session.Player?.DistanceTo(image.Position) <= 5)
            {
                Session.Player.PlaySound(SoundNames.PickupToken);
                Session.Player.Tokens++;
                Unparent();
                Session.ObjectPools.Tokens.Return(this);
            }
        }

        #endregion

        // Drop
        public void Drop(GameRoom room, Vector2 origin)
        {
            float yOffset = RandomBetween(-4f, 2f);

            image.Position = new(RandomBetween(origin.X - 15, origin.X + 15f), origin.Y  + yOffset);

            groundY = origin.Y + Randomizer.Next(-3, 3);
            launchDelay = RandomBetween(0, .1f);
            delayTimer = 0;
            launched = false;
            room.Children.Add(this);
        }
    }
}
