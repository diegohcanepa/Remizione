using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Loot
    /// </summary>
    public sealed class Loot : Pickup
    {
        private readonly FloatTween altitudeTween = new();
        private readonly FloatTween opacityTween = new();
        private readonly FloatTween xTween = new();
        private readonly FloatTween yTween = new();

        #region Constructor

        // Constructor
        public Loot(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.UI;
            CollisionDetection = false;
            DepthOffset = -2;
            IgnoreThrowables = true;
            PickUpSound = Sound.Find(SoundNames.PickupGeneric);
            Scale = ScaleInfo.UIElement.Tiny;
        }

        #endregion

        // DropCore
        private bool DropCore(GameRoom room, Vector2 position, MetaItem metaItem)
        {
            if (Session.Room == null)
                return false;

            this.Sprite.ClearAnimations();
            var anim = this.Sprite.AddAnimation(metaItem.Name);
            anim.AddFrame(metaItem.Name, 1000);

            DisplayNameKey = metaItem.LocalizedDisplayName;
            ItemName = metaItem.Name;
            Position = position;
            room.Children.Add(this);

            return true;
        }

        // Float
        private void Float()
        {
            altitudeTween.Start(TweenStyle.QuadraticIn, 0, 1, 250, -1);
            Tweens.AltitudeTween = altitudeTween;
        }

        // Drop
        public void Drop(GameRoom room, Vector2 position, MetaItem metaItem)
        {
            if (!DropCore(room, position, metaItem))
                return;

            altitudeTween.Start(TweenStyle.QuadraticIn, 8, 0, 250, Float);
            opacityTween.Start(TweenStyle.CubicIn, 0, 1, 100);

            Tweens.AltitudeTween = altitudeTween;
            Tweens.OpacityTween = opacityTween;
        }

        // DropJumping
        public void DropJumping(GameRoom room, Vector2 startPos, Vector2 endPos, MetaItem metaItem)
        {
            // Fall
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
                Float();
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
