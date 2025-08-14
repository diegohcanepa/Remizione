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
        private readonly FloatTween xTween = new();
        private readonly FloatTween yTween = new();

        #region Constructor

        // Constructor
        public LootBag(GameSession session, string name)
            : base(session, name)
        {
            DepthOffset = -2;
            IgnoreThrowables = true;
            PickUpSound = Sound.Find(SoundNames.PickupBag);
            Scale = new(.75f);
        }

        #endregion

        // DropCore
        private bool DropCore(GameRoom room, Vector2 position, MetaItem metaItem)
        {
            if (Session.Room == null)
                return false;

            ItemName = metaItem.Name;
            LocalizedDisplayName = $"{TextRepository.GetValue("Prop.Bag")} ({LocalizedDisplayName})";

            Position = position;
            room.Children.Add(this);

            return true;
        }

        // Drop
        public void Drop(GameRoom room, Vector2 position, MetaItem metaItem)
        {
            if (!DropCore(room, position, metaItem))
                return;

            altitudeTween.Start(TweenStyle.QuadraticIn, 8, 0, 250);
            opacityTween.Start(TweenStyle.CubicIn, 0, 1, 100);

            Tweens.AltitudeTween = altitudeTween;
            Tweens.OpacityTween = opacityTween;
        }

        // DropJumping
        public void DropJumping(GameRoom room, Vector2 startPos, Vector2 endPos, MetaItem metaItem)
        {
            void Fall(Vector2 endPos)
            {
                yTween.Start(TweenStyle.QuadraticIn, Y, endPos.Y, 200, Bounce);
                Tweens.YTween = yTween;
            }

            // Bounce
            void Bounce()
            {
                Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.QuadraticInOut, Scale, Scale * new Vector2(1, .8f), 100, 2, Land);
            }

            // Land
            void Land()
            {
                PlaySound(SoundNames.LootBagLand);
                DepthOffset = -2;
            }

            DepthOffset = 12;
            IgnoreWalkArea = true;

            if (!DropCore(room, startPos, metaItem))
                return;

            xTween.Start(TweenStyle.QuadraticIn, startPos.X, endPos.X, 400);
            yTween.Start(TweenStyle.QuadraticIn, startPos.Y, startPos.Y - 4, 200, () => Fall(endPos));

            Tweens.XTween = xTween;
            Tweens.YTween = yTween;

            PlaySound(SoundNames.LootBagJump);
        }
    }
}
