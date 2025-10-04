using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// Pickup
    /// </summary>
    public sealed class Pickup : Prop
    {
        #region Private fields

        private readonly FloatTween altitudeTween = new();
        private const float bounceFactor = .8f;
        private static bool dropLeft;
        private const float gravity = 300;
        private float groundY;
        private bool isCollecting;
        private float life = 2;
        private MetaItem? metaItem;
        private readonly Vector2Tween scaleTween = new();
        private readonly ImageSprite shadow;
        private Vector2 velocity;

        #endregion

        // Constructor
        public Pickup(GameSession session)
            : base(session, string.Empty)
        {
            this.CollisionDetection = false;
            this.Atlas = Atlases.UI;
            this.Collider = new("20,12;20,22;0,22;0,12");
            this.DepthOffset = -5;
            this.IgnoreWalkArea = false;

            // Shadow
            this.shadow = new ImageSprite(session.Game, Atlases.UI.GetImage(nameof(Pickup) + "Shadow"))
            {
                Opacity = ColorPalette.ShadowOpacity,
                PivotOrigin = RectanglePoint.Bottom,
            };
        }

        #region Protected members

        // OnDrawShadow
        protected override void OnDrawShadow(GameTime gameTime)
        {
            /*
            shadow.Position = BoundingBox.GetPoint(RectanglePoint.Bottom);
            if (altitudeTween.IsRunning)
                shadow.Y += altitudeTween.CurrentValue;

            shadow.Draw(gameTime);
            */
        }

        // OnUnload
        protected override void OnUnload()
        {
            base.OnUnload();
            this.metaItem = null;
            altitudeTween.Stop();
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
            else if (!altitudeTween.IsRunning)
            {
                altitudeTween.Start(TweenStyle.QuadraticInOut, 0, .5f, 200, -1);
                Tweens.AltitudeTween = altitudeTween;
            }

            if (isCollecting)
            {
                if (!scaleTween.IsRunning)
                {
                    Unparent();
                    Session.ObjectPools.Pickups.Return(this);
                }
            }
            else if (life <= 0 && Session.Player?.RuntimeHotspot.BoundingRectangleF.Intersects(BoundingBox) == true)
            {
                isCollecting = true;

                DepthOffset = 10;

                scaleTween.Start(TweenStyle.Linear, Scale, Vector2.Zero, 250);
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
