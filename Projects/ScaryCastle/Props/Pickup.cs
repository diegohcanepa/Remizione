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
        private int delayCoolDown;
        private readonly FloatTween yTween = new();
        private bool isCoin;
        private MetaItem? metaItem;
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
            this.Hotspot = new("16,8;16,14;4,14;4,8");
            this.IgnoreWalkArea = false;

            this.shadow = new(Game, Atlases.UI.PickupShadow)
            {
                Opacity = ColorPalette.ShadowOpacity,
                PivotOrigin = RectanglePoint.Center
            };
        }

        #region Private members

        // Pop
        private void Pop()
        {
            bounceScaleEffect.Play(ScaleInfo.UIElement.Tiny.X, .75f);

            if (Sound.Find(isCoin ? SoundNames.LootCoin : SoundNames.LootSack)?.PopInstance() is SoundInstance soundInstance)
                soundInstance.PlayDelayed(300);

            yTween.Start(TweenStyle.Linear, 0, -1, 200, -1);

            this.shadowScaleTween.Start(TweenStyle.CubicIn, Vector2.Zero, ScaleInfo.UIElement.Medium, 300);
            this.shadow.Tweens.ScaleTween = shadowScaleTween;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            shadow.Draw(gameTime);
            
            Y += yTween.CurrentValue;
            base.OnDraw(gameTime);
            Y -= yTween.CurrentValue;
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

            yTween.Update(gameTime);
            shadow.Update(gameTime);

            if (delayCoolDown >= 0)
            {
                delayCoolDown -= gameTime.ElapsedGameTime.Milliseconds;

                if (delayCoolDown < 0)
                    Pop();

                return;
            }

            base.OnUpdate(gameTime);

            if (bounceScaleEffect.IsPlaying)
            {
                bounceScaleEffect.Update(gameTime);
                Scale = new(bounceScaleEffect.Value);
            }
        }

        #endregion

        // Collect
        [ScriptMethod]
        public void Collect()
        {
            if (metaItem == null)
                return;

            if (Room is ProceduralRoom procRoom)
            {
                if (isCoin)
                    procRoom.RoomGraph.HasCoin = false;
                else
                    procRoom.RoomGraph.SackCount--;
            }

            Session.Player?.Animate(AnimationNames.PickUp);
            Session.Inventory.Add(metaItem, 1);
            Session.HUD.Log.Show(LogVerb.PickedUp, metaItem);
            Session.ObjectPools.Pickups.Return(this);
            Session.HUD.SackSlot.AddItem(metaItem, Position);
            Unparent();
        }

        // Drop
        public void Drop(Room room, Vector2 origin, MetaItem metaItem)
        {
            room.Children.Add(this);
            this.Position = origin;
            this.metaItem = metaItem;
            this.delayCoolDown = 300;
            this.shadow.Position = origin - Vector2.UnitY;
            this.shadow.Scale = Vector2.Zero;
            this.yTween.Stop();

            Sprite.ClearAnimations();
            
            this.DefaultImageName = metaItem.Name;
            this.isCoin = metaItem.Name == MetaItem.CoinItemName;
            this.Scale = Vector2.Zero;
            this.DisplayNameKey = $"Item.{metaItem.Name}.Name";
        }
    }
}
