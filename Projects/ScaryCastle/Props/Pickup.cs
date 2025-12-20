using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// Pickup
    /// </summary>
    public sealed class Pickup : Prop
    {
        #region Private fields

        private int delayCoolDown;
        private bool isCoin;
        private MetaItem? metaItem;
        private readonly BounceScaleEffect bounceScaleEffect = new();
        private bool showItemIcon;

        #endregion

        // Constructor
        public Pickup(GameSession session, string name)
            : base(session, name)
        {
            this.CollisionDetection = false;
            this.DepthOffset = -1;
            this.HighlightInteraction = false;
            this.Hotspot = new("7,0;7,7;0,7;0,0");
            this.IgnoreWalkArea = false;
        }

        #region Private members

        // DropCore
        private void DropCore(Room room, Vector2 position, MetaItem metaItem, bool showItemIcon, int delay)
        {
            room.Children.Add(this);
            this.Position = position;
            this.metaItem = metaItem;
            this.showItemIcon = showItemIcon;
            this.delayCoolDown = delay;

            Sprite.ClearAnimations();
            this.Atlas = showItemIcon ? Atlases.UI : Atlases.Environment;
            this.DefaultImageName = showItemIcon ? metaItem.Name : nameof(Atlases.Environment.Sack);
            this.DisplayNameKey = $"Item.{metaItem.Name}.Name";
            this.isCoin = metaItem.Name == MetaItem.CoinItemName;
            this.Scale = Vector2.Zero;
        }

        // Pop
        private void Pop()
        {
            float scale = showItemIcon ? ScaleInfo.UIElement.Tiny.X : 1;
            bounceScaleEffect.Play(scale, .75f);

            if (Sound.Find(isCoin ? SoundNames.LootCoin : SoundNames.LootSack)?.PopInstance() is SoundInstance soundInstance)
                soundInstance.PlayDelayed(300);
        }

        #endregion

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
            if (metaItem != null)
            {
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
                Unparent();
                Session.ObjectPools.Pickups.Return(this);
            }
        }

        // DropCoin
        public void Drop(Room room, Vector2 origin)
        {
            DropCore(room, origin, MetaItem.FindNotNull(MetaItem.CoinItemName), true, 0);
        }

        // DropSack
        public void Drop(Room room, Vector2 origin, MetaItem metaItem)
        {
            DropCore(room, origin, metaItem, false, 0);
        }
    }
}
