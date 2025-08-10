using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// LootBag
    /// </summary>
    public sealed class LootBag : Pickup
    {
        private readonly FloatTween altitudeTween = new();
        private readonly FloatTween opacityTween = new();

        #region Constructor

        // Constructor
        public LootBag(GameSession session, string name)
            : base(session, name)
        {
            IgnoreThrowables = true;
            PickUpSound = Sound.Find(SoundNames.PickupBag);
            Scale = new(.75f);
        }

        #endregion

        // Drop
        public void Drop(GameRoom room, Vector2 position, MetaItem metaItem)
        {
            if (Session.Room == null)
                return;

            ItemName = metaItem.Name;

            Position = position;
            room.Children.Add(this);

            altitudeTween.Start(TweenStyle.QuadraticIn, 8, 0, 250);
            opacityTween.Start(TweenStyle.CubicIn, 0, 1, 100);

            Tweens.AltitudeTween = altitudeTween;
            Tweens.OpacityTween = opacityTween;

            LocalizedDisplayName = $"{TextRepository.GetValue("Prop.Bag")} ({LocalizedDisplayName})";
        }
    }
}
