using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// LootBag
    /// </summary>
    public sealed class LootBag : Prop
    {
        private readonly FloatTween altitudeTween = new();
        private bool isBeginCollected;
        private readonly FloatTween opacityTween = new();
        private readonly Vector2Tween scaleTween = new();

        #region Constructor

        // Constructor
        public LootBag(GameSession session)
            : base(session, string.Empty)
        {
            Atlas = Atlases.Environment;
            DefaultImageName = "LootBag";
        }

        #endregion

        #region Protected members

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            /*
            // Being collected
            if (isBeginCollected)
            {
                if (scaleTween.IsRunning)
                {
                    scaleTween.Update(gameTime);
                    
                    if (!scaleTween.IsRunning)
                    {
                        Unparent();
                        Session.ObjectPools.LootBags.Return(this);

                        if (Loot != null && Session.Player != null)
                            Session.Player.AddItem(Loot, 1);
                    }
                }

                return;
            }

            if (Session.Player != null && Session.Player.DistanceTo(this) <= 5)
            {
                isBeginCollected = true;
                scaleTween.Start(TweenStyle.Linear, Scale, Scale / 2, 100);
                Tweens.ScaleTween = scaleTween;
                Sound.Play(SoundNames.LootBagPickup);
            }
            */
        }

        #endregion

        // Drop
        public void Drop(Vector2 position, ItemName itemName)
        {
            Loot = null;
            Scale = Vector2.One;
            isBeginCollected = false;

            if (Session.Room != null && MetaItem.Find(itemName) is MetaItem metaItem)
            {
                Loot = metaItem;
                Position = position;
                Session.Room.Children.Add(this);
            }

            altitudeTween.Start(TweenStyle.CubicIn, 12, 0, 300);
            opacityTween.Start(TweenStyle.CubicIn, 0, 1, 200);

            Tweens.AltitudeTween = altitudeTween;
            Tweens.OpacityTween = opacityTween;
        }

        // Loot
        public MetaItem? Loot { get; private set; }
    }
}
