using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// EnergyBolt
    /// </summary>
    public sealed class EnergyBolt : Prop
    {
        #region Private fields

        private readonly FloatTween altitudeTween = new();
        private const float bounceFactor = .8f;
        private static string? displayName;
        private const float gravity = 300;
        private float groundY;
        private bool isCollecting;
        private float launchDelay;
        private bool launched;
        private float life = 2;
        private static readonly Color particleColor = new Color(207, 117, 43) * .8f;
        private readonly ParticlePopEffect particleEffect;
        private readonly Vector2Tween scaleTween = new();
        private Vector2 velocity;
        private readonly FloatTween yTween = new();

        #endregion

        // Constructor
        public EnergyBolt(GameSession session)
            : base(session, string.Empty)
        {
            this.Atlas = Atlases.UI;
            this.Color = new(240, 181, 65);
            this.DefaultImageName = MetaItem.EnergyBoltName;
            this.DepthOffset = 5;

            this.particleEffect = new ParticlePopEffect(Game)
            {
                BurstSize = new(16, 26),
                HorizontalSpeed = 50,
                ParticleLifetime = .3f,
                Scale = .75f
            };

            Tweens.AltitudeTween = FloatTween.Create(TweenStyle.CubicInOut, 0, 1, 300, -1);
            displayName ??= $"+{TextRepository.GetValue("@Prop.EnergyBolt")}";
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

            if (life > 0)
            {
                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
                life -= dt;

                if (!launched)
                {
                    velocity = new(-50, RandomBetween(-20f, 10f));
                    launched = true;
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

                    if (Math.Abs(velocity.Y) < 6)
                        velocity.Y = 0;
                }
            }

            if (isCollecting)
            {
                if (!scaleTween.IsRunning && !particleEffect.IsActive)
                {
                    Unparent();
                    Session.ObjectPools.EnergyBolts.Return(this);
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

                Session.Player.PlaySound(SoundNames.EnergyBolt);

                Session.Power += 1;

                particleEffect.Spawn(BoundingBox.Center, particleColor);

                if (displayName != null && !Session.PowerRestored)
                    Session.Player.ShowFloatingText(displayName, ColorPalette.EnergyBolt, 1000);
            }
        }

        #endregion

        // Drop
        public void Drop(Room room, Vector2 origin, RectangleF bounds)
        {
            float yOffset = RandomBetween(-4f, 2f);
            Position = new(RandomBetween(bounds.Left - 10, bounds.Right + 10),
                           RandomBetween(bounds.Top, bounds.Bottom) + yOffset);

            yTween.Stop();
            scaleTween.Stop();
            isCollecting = false;
            life = 2;
            groundY = origin.Y + Randomizer.Next(-3, 3);
            launchDelay = RandomBetween(0, .1f);
            Scale = ScaleInfo.UIElement.Tiny;
            launched = false;

            room.Children.Add(this);
        }
    }
}
