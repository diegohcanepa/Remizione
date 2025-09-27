using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Firecracker
    /// </summary>
    public sealed class Firecracker : PlacedItem
    {
        private int cooldown;
        private SoundInstance? explosionSound;
        private bool isExploding;
        private SoundInstance? fuseHissingSound;
        private readonly FloatTween xTween = new();
        private readonly FloatTween yTween = new();

        // Constructor
        public Firecracker(GameSession session)
            : base(session)
        {
            RenderLayer = RenderLayer.Background;
            var animation = AddAnimation("Default");
            animation.AddFrameSequence(nameof(Firecracker), 30, 1, 3);
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (isExploding)
                return;

            base.OnDraw(gameTime);
        }

        // OnPlaced
        protected override void OnPlaced()
        {
            cooldown = 1500;
            explosionSound = null;
            isExploding = false;
            fuseHissingSound = PlaySound(SoundNames.FuseHissing);

            xTween.Start(TweenStyle.Linear, X, X + .5f, 40, -1);
            yTween.Start(TweenStyle.Linear, Y, Y + .9f, 30, -1);

            Tweens.XTween = xTween;
            Tweens.YTween = yTween;

            Session.Room?.Children.Add(this);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (cooldown > 0)
            {
                cooldown -= gameTime.ElapsedGameTime.Milliseconds;
            }
            else if (!isExploding)
            {
                isExploding = true;
                fuseHissingSound?.Stop();

                if (Item != null && Session.Room != null)
                {
                    if (Item.MetaItem.Sound != null)
                        explosionSound = PlaySound(Item.MetaItem.Sound);

                    for (var i = 0; i < Session.Room.CulledThings.Count; i++)
                    {
                        if (Session.Room.CulledThings[i] is GameThing target)
                        {
                            if (target.DistanceTo(this) <= Item.Range)
                                Item.MetaItem.ApplyDamage(Item.Owner, target);
                        }
                    }
                }

                if (Session.Camera.ShakeState == CameraShakeState.None)
                    Session.Camera.Shake(TweenStyle.Linear, new(1, 1), 50, 4);
            }
            else if (explosionSound == null || !explosionSound.IsPlaying)
            {
                explosionSound = null;
                Unparent();
            }
        }

        #endregion
    }
}
