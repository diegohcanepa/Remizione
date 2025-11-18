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
        private bool isCollecting;
        private MetaItem? metaItem;
        private readonly Vector2Tween scaleTween = new();
        private readonly ImageSprite shadow;

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
            shadow.Position = BoundingBox.GetPoint(RectanglePoint.Bottom);
            if (altitudeTween.IsRunning)
                shadow.Y += altitudeTween.CurrentValue;
            shadow.Draw(gameTime);
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

            if (scaleTween.IsRunning)
                return;

            if (!altitudeTween.IsRunning)
            {
                altitudeTween.Start(TweenStyle.QuadraticInOut, 0, .5f, 200, -1);
                Tweens.AltitudeTween = altitudeTween;
                return;
            }

            if (isCollecting)
            {
                if (!scaleTween.IsRunning)
                {
                    Unparent();
                    Session.ObjectPools.Pickups.Return(this);
                }
            }
            else if (Session.Player?.DistanceTo(this) < 4)
            {
                if (!Session.Inventory.IsFull)
                { 
                    isCollecting = true;
                    DepthOffset = 10;
                    PivotOrigin = RectanglePoint.Top;
                    Y -= BoundingBox.Height;
                    scaleTween.Start(TweenStyle.Linear, Scale, Vector2.Zero, 150);
                    Tweens.ScaleTween = scaleTween;

                    Session.Inventory.Add(metaItem, 1);

                    Session.HUD.Log.Show(LogVerb.PickedUp, metaItem);
                }
            }
        }

        #endregion

        // Drop
        public void Drop(Room room, Vector2 origin, MetaItem metaItem)
        {
            PivotOrigin = RectanglePoint.Bottom;
            Position = origin;
            this.metaItem = metaItem;

            this.DefaultImageName = metaItem.Name;
            isCollecting = false;
            room.Children.Add(this);

            scaleTween.Start(TweenStyle.Linear, Vector2.Zero, ScaleInfo.UIElement.Tiny, 250);
            Tweens.ScaleTween = scaleTween;
        }
    }
}
