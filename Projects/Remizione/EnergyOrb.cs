using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// EnergyOrb
    /// </summary>
    public sealed class EnergyOrb : GameThing
    {
        #region Private fields

        private const float bounceFactor = .8f;
        private float delayTimer;
        private const float gravity = 300;
        private float groundY;
        private bool isCollecting;
        private float launchDelay;
        private bool launched;
        private float life = 2;
        private readonly FloatTween opacityTween = new();
        private static readonly Color particleColor = new Color(207, 117, 43) * .8f;
        private readonly ParticlePopEffect particleEffect;
        private readonly Vector2Tween scaleTween = new();
        private Vector2 velocity;
        private readonly FloatTween yTween = new();

        #endregion

        // Constructor
        public EnergyOrb(GameSession session)
            : base(session, string.Empty)
        {
            this.Atlas = Atlases.Environment;
            this.Color = new(240, 181, 65);
            this.DefaultImageName = "EnergyOrb";
            this.DepthOffset = 5;
            this.particleEffect = new ParticlePopEffect(Game)
            {
                BurstSize = new(10, 16),
                HorizontalSpeed = 50,
                ParticleLifetime = .3f,
                Scale = .5f
            };
        }

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
            base.OnDraw(gameTime);
            particleEffect.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            particleEffect.Update(gameTime);

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!launched)
            {
                delayTimer += dt;
                if (delayTimer >= launchDelay)
                {
                    velocity = new(RandomBetween(-30f, 30f), RandomBetween(-20f, 10f));
                    launched = true;
                }
                return;
            }

            velocity.Y += gravity * dt;
            X += velocity.X * dt;
            Y += velocity.Y * dt;

            if (Y >= groundY)
            {
                Y = groundY;
                velocity.Y *= -bounceFactor;
                velocity.X *= .7f;

                if (Math.Abs(velocity.Y) < 6f)
                    velocity.Y = 0;
            }

            life -= dt;

            if (isCollecting)
            {
                if (!scaleTween.IsRunning && !particleEffect.IsActive)
                {
                    Unparent();
                    Session.ObjectPools.EnergyOrbs.Return(this);
                }
            }
            else if (Session.Player?.DistanceTo(this) <= 5)
            {
                isCollecting = true;

                DepthOffset = 10;

                yTween.Start(TweenStyle.Linear, Y, Y - 5, 150);
                Tweens.YTween = yTween;

                scaleTween.Start(TweenStyle.Linear, Scale, Vector2.Zero, 150);
                Tweens.ScaleTween = scaleTween;

                Session.Player.PlaySound(SoundNames.EnergyOrb);
                
                Session.Energy += 1;

                particleEffect.Spawn(BoundingBox.Center, particleColor);
            }
        }

        #endregion

        // Launch
        public void Launch(Room room, Vector2 origin, RectangleF bounds)
        {
            float yOffset = RandomBetween(-4f, 2f);
            Position = new(RandomBetween(bounds.Left + 5f, bounds.Right - 5f),
                           RandomBetween(bounds.Top, bounds.Bottom) + yOffset);

            yTween.Stop();
            scaleTween.Stop();
            isCollecting = false;
            life = 2;
            groundY = origin.Y + Randomizer.Next(-3, 3);
            launchDelay = RandomBetween(0, .1f);
            delayTimer = 0;
            Scale = ScaleInfo.UIElement.Small;
            launched = false;

            opacityTween.Start(TweenStyle.Linear, 1, .7f, 40, -1);
            Tweens.OpacityTween = opacityTween;

            room.Children.Add(this);
        }
    }
}
