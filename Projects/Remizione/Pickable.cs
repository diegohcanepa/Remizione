using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// Pickable
    /// </summary>
    public sealed class Pickable : Prop
    {
        #region Private fields

        private const float bounceFactor = .8f;
        private static bool dropLeft;
        private const float gravity = 300;
        private float groundY;
        private bool isCollecting;
        private float life = 2;
        private MetaItem? metaItem;
        private readonly Vector2Tween scaleTween = new();
        private Vector2 velocity;

        #endregion

        // Constructor
        public Pickable(GameSession session)
            : base(session, string.Empty)
        {
            this.Atlas = Atlases.UI;
            this.DefaultImageName = MetaItem.EnergyBoltName;
            this.DepthOffset = 5;
        }

        #region Protected members

        // OnUnload
        protected override void OnUnload()
        {
            base.OnUnload();
            this.metaItem = null;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (metaItem == null)
                return;

            base.OnUpdate(gameTime);

            if (life > 0)
            {
                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
                life -= dt;

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
                if (!scaleTween.IsRunning)
                {
                    Unparent();
                    Session.ObjectPools.Pickables.Return(this);
                }
            }
            else if (Session.Player?.DistanceTo(this) <= 5)
            {
                isCollecting = true;

                DepthOffset = 10;

                scaleTween.Start(TweenStyle.Linear, Scale, Vector2.Zero, 150);
                Tweens.ScaleTween = scaleTween;

                Session.Player?.Inventory.GetContainer(metaItem.Category).Add(metaItem, 1);

                Session.HUD.Log.Show(LogVerb.PickedUp, metaItem.LocalizedDisplayName, metaItem.Image);
            }
        }

        #endregion

        // Drop
        public void Drop(Room room, MetaItem metaItem, Vector2 origin)
        {
            Position = origin;

            var xVelocity = 50;
            if (dropLeft)
                xVelocity *= -1;

            velocity = new(xVelocity, -40);

            this.metaItem = metaItem;
            this.DefaultImageName = metaItem.Name;

            scaleTween.Stop();
            isCollecting = false;
            life = 2;
            groundY = origin.Y + Randomizer.Next(-3, 3);
            Scale = ScaleInfo.UIElement.Tiny;

            room.Children.Add(this);

            dropLeft = !dropLeft;
        }
    }
}
