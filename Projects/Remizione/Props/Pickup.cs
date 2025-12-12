using Adberration;
using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Pickup
    /// </summary>
    public sealed class Pickup : Prop
    {
        #region Private fields

        private MetaItem? metaItem;
        private readonly Vector2Tween scaleTween = new();

        #endregion

        // Constructor
        public Pickup(GameSession session, string name)
            : base(session, name)
        {
            this.Atlas = Atlases.Environment;
            this.CollisionDetection = false;
            this.DepthOffset = -1;
            this.HighlightInteraction = false;
            this.Hotspot = new("0,4;7,4;8,8;-1,8");
            this.IgnoreWalkArea = false;
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
        }

        #endregion

        // Collect
        [ScriptMethod]
        public void Collect()
        {
            if (metaItem != null)
            {
                Session.Player?.Animate("TakeSack");
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
            var isCoin = metaItem.Name == MetaItem.CoinItemName;
            this.DefaultImageName = isCoin ? nameof(Atlases.Environment.Coin) : nameof(Atlases.Environment.Sack);
            scaleTween.Start(TweenStyle.Linear, Vector2.Zero, Vector2.One, 250);

            this.DisplayNameKey = $"Item.{metaItem.Name}.Name";

            room.Children.Add(this);

            Tweens.ScaleTween = scaleTween;

            PlaySound(SoundNames.ItemPop);
        }
    }
}
