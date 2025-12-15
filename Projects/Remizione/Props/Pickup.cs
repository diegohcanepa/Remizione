using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
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
        private BounceScaleEffect bounceScaleEffect = new();

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
            bounceScaleEffect.Play(isCoin ? .6f : 1, .75f);

            if (Sound.Find(isCoin ? SoundNames.LootCoin : SoundNames.LootSack)?.PopInstance() is SoundInstance soundInstance)
                soundInstance.PlayDelayed(300);
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
                return;
            }

            base.OnUpdate(gameTime);

            bounceScaleEffect.Update(gameTime);
            Scale = new(bounceScaleEffect.Value);
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
            Sprite.ClearAnimations();
            PivotOrigin = RectanglePoint.Bottom;
            Position = origin;
            this.metaItem = metaItem;
            this.isCoin = metaItem.Name == MetaItem.CoinItemName;

            popCooldown = Random.Shared.Next(800, 1500);

            this.AllowInteraction = false;
            this.DefaultImageName = isCoin ? string.Empty : nameof(Atlases.Environment.Sack);
            this.DisplayNameKey = $"Item.{metaItem.Name}.Name";
            this.DepthOffset = isCoin ? -100 : -1;
            this.PivotOrigin = isCoin ? RectanglePoint.Center : RectanglePoint.Bottom;
            this.Scale = Vector2.Zero;

            if (isCoin)
            {
                var animation = Sprite.AddAnimation("Coin");
                animation.AddFrame("Coin01", 2500);
                animation.AddFrame("Coin02", 100);
                animation.AddFrame("Coin03", 100);
                animation.AddFrame("Coin04", 100);
            }

            AnimationPlayer.Play("Coin", true);
            room.Children.Add(this);
        }
    }
}
