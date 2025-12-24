using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// Pickup
    /// </summary>
    public sealed class Pickup : Prop
    {
        #region Private fields

        private readonly BounceScaleEffect bounceScaleEffect = new();
        private bool collected;
        private readonly Vector2 defaultScale = ScaleInfo.UIElement.Large; 
        private int delayCoolDown;
        private MetaItem? metaItem;
        private ProceduralRoom? room;
        private readonly ImageSprite shadow;
        private readonly Vector2Tween shadowScaleTween = new();

        #endregion

        // Constructor
        public Pickup(GameSession session, string name)
            : base(session, name)
        {
            this.Atlas = Atlases.UI;
            this.CollisionDetection = false;
            this.DepthOffset = -1;
            this.HighlightInteraction = false;
            this.HotspotPlacement = PlacementMode.Absolute;
            this.IgnoreWalkArea = false;
            this.FloatingForce = 1;

            this.shadow = new(Game, Atlases.UI.PickupShadow)
            {
                Opacity = ColorPalette.ShadowOpacity,
                PivotOrigin = RectanglePoint.Center
            };
        }

        #region Private members

        // Collect
        private void Collect()
        {
            if (metaItem == null)
                return;

            if (Session.Player != null)
            {
                Session.Player.Animate(AnimationNames.PickUp);
                metaItem.Effect.ApplyHP(Session.Player);
            }
            Session.ObjectPools.Pickups.Return(this);
            
            if (room != null)
                room.RoomGraph.HeartCount--;

            Unparent();
        }

        // Pop
        private void Pop()
        {
            bounceScaleEffect.Play(defaultScale.X, .75f);

            // TODO: Check old LootSack sound
            if (Sound.Find(SoundNames.LootSack)?.PopInstance() is SoundInstance soundInstance)
                soundInstance.PlayDelayed(300);

            this.shadowScaleTween.Start(TweenStyle.CubicIn, Vector2.Zero, ScaleInfo.UIElement.Medium, 300);
            this.shadow.Tweens.ScaleTween = shadowScaleTween;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            shadow.Draw(gameTime);
            base.OnDraw(gameTime);
        }

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

            shadow.Update(gameTime);

            base.OnUpdate(gameTime);

            if (delayCoolDown >= 0)
            {
                delayCoolDown -= gameTime.ElapsedGameTime.Milliseconds;

                if (delayCoolDown < 0)
                    Pop();

                return;
            }

            if (bounceScaleEffect.IsPlaying)
            {
                bounceScaleEffect.Update(gameTime);
                Scale = new(bounceScaleEffect.Value);
            }

            if (Session.Player != null && Session.Player.HP < Session.Player.MaxHP)
            {
                if (!collected)
                {
                    if (Session.Player.DistanceTo(Position) <= 3)
                    {
                        collected = true;
                        Collect();
                    }
                }
            }
        }

        #endregion

        // Drop
        public bool Drop(ProceduralRoom room, Vector2 origin, MetaItem metaItem)
        {
            if (metaItem.Category != ItemCategory.Life)
                return false;

            this.room = room;

            room.Children.Add(this);
            
            this.collected = false;
            this.Position = origin;
            this.metaItem = metaItem;
            this.delayCoolDown = 300;
            this.shadow.Position = origin - Vector2.UnitY;
            this.shadow.Scale = Vector2.Zero;
            this.DefaultImageName = metaItem.Name;
            this.DisplayNameKey = $"Item.{metaItem.Name}.Name";
            this.Scale = defaultScale;
            this.Hotspot = new Polygon(BoundingBox.GetPoints());
            this.Scale = Vector2.Zero;

            return true;
        }
    }
}
