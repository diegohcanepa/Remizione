using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// WreckingBall
    /// </summary>
    public sealed class WreckingBall : Trap
    {
        private Vector2 hitPosition;

        // Constructor
        public WreckingBall(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Props;
            DepthOffset = 10;
            IdleDuration = 10;
            ActivatingDuration = 0;
            ActiveDuration = 5;
            CooldownDuration = 2;
        }

        #region Private members

        // HitFloor
        private void HitFloor()
        {
            PlaySound(SoundNames.WreckingBallImpact);
            Session.Camera.Shake(TweenStyle.QuadraticInOut, new(0, .8f), 40, 6);

            if (Room == null || Session.IsAwaiting || Definition == null)
                return;

            for (var i = 0; i < Room.Children.Count; i++)
            {
                if (Room.Children[i] == this)
                    continue;

                if (Room.Children[i] is GameThing target && !target.IsDead && target.MaxHP > 0)
                {
                    if (RuntimeCollider.BoundingRectangleF.Bottom >= target.Y && RuntimeCollider.BoundingRectangleF.Intersects(target.RuntimeHotspot.BoundingRectangleF))
                        EffectDescriptor.Apply(Definition.EffectDescriptors, this, target, EffectContext.Contact);
                }
            }
        }

        #endregion

        #region Protected members

        // OnParentChanged
        protected override void OnParentChanged(Entity? previousParent)
        {
            base.OnParentChanged(previousParent);
            
            if (Room != null && previousParent is null)
            {
                var crack = new Crack(Session)
                {
                    Position = this.Position
                };
                Room.Children.Add(crack);
                this.hitPosition = Position;
            }
        }

        // OnStateEnter
        protected override void OnStateEnter(TrapState state)
        {
            if (Room?.WalkArea == null)
                return;

            if (state == TrapState.Idle)
            {
                Y = 10;
            }
            else if (state == TrapState.Active)
            {
                Tweens.YTween = FloatTween.Create(TweenStyle.QuadraticIn, Y, hitPosition.Y, 400, HitFloor);
            }

            else if (state == TrapState.Cooldown)
            {
                Tweens.YTween = FloatTween.Create(TweenStyle.QuadraticIn, Y, 10, 1000);
            }
        }

        #endregion

        /// <summary>
        /// Crack
        /// </summary>
        public sealed class Crack : GameThing
        {
            // Constructor
            public Crack(GameSession session)
                : base(session, string.Empty)
            {
                Atlas = Atlases.Props;
                DefaultImageName = "WreckingBallCrack";
                PivotOrigin = Engendro.RectanglePoint.Center;
                RenderLayer = RenderLayer.Background;
            }
        }
    }
}
