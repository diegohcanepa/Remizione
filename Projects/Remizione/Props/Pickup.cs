using Adberration;
using Adberration.Scripting;
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

        private bool isCoin;
        private MetaItem? metaItem;
        private int popCooldown;
        private readonly Vector2Tween scaleTween = new();

        #endregion

        // Constructor
        public Pickup(GameSession session, string name)
            : base(session, name)
        {
            this.Atlas = Atlases.Environment;
            this.CollisionDetection = false;
            this.HighlightInteraction = false;
            this.Hotspot = new("7,0;7,7;0,7;0,0");
            this.IgnoreWalkArea = false;
        }

        #region Private members

        // Pop
        private void Pop()
        {
            AllowInteraction = true;    
            Tweens.ScaleTween = scaleTween;
            PlaySound(isCoin ? SoundNames.LootCoin : SoundNames.LootSack);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (popCooldown > 0)
                return;

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

            if (popCooldown > 0)
            {
                popCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                if (popCooldown <= 0)
                    Pop();
            }

            base.OnUpdate(gameTime);
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

        // Drop
        public void Drop(Room room, Vector2 origin, MetaItem metaItem)
        {
            PivotOrigin = RectanglePoint.Bottom;
            Position = origin;
            this.metaItem = metaItem;
            this.isCoin = metaItem.Name == MetaItem.CoinItemName;

            popCooldown = Random.Shared.Next(800, 1500);
            
            this.AllowInteraction = false;
            this.DefaultImageName = isCoin ? nameof(Atlases.Environment.Coin) : nameof(Atlases.Environment.Sack);
            this.DisplayNameKey = $"Item.{metaItem.Name}.Name";
            this.DepthOffset = isCoin ? -100 : -1;
            this.PivotOrigin = isCoin ? RectanglePoint.Center : RectanglePoint.Bottom;

            var scale = isCoin ? new(.6f) : Vector2.One;
            scaleTween.Start(TweenStyle.Linear, scale, scale * 1.1f, 100, -1);
            room.Children.Add(this);
        }
    }
}
