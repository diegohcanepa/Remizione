using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// WreckingBall
    /// </summary>
    public sealed class WreckingBall : Trap, ISpawnNotification
    {
        private Vector2 hitPosition;

        // Constructor
        public WreckingBall(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Props;
            DepthOffset = -1;
            IdleDuration = 10;
            ActivatingDuration = 0;
            ActiveDuration = 5;
            CooldownDuration = 2;
            MatchShadowTransform = false;
        }

        #region ISpawnNotification interface

        // OnSpawned
        void ISpawnNotification.OnSpawned(ProceduralRoom room)
        {
            var crack = new Crack(Session)
            {
                Position = this.Position - new Vector2(0, 5)
            };
            room.Children.Add(crack);

            this.hitPosition = Position;
            this.Shadow.Position = hitPosition;

            IdleDuration = room.RoomNode.Definition.Difficulty switch
            {
                Difficulty.Easy => 10,
                Difficulty.Normal => 8,
                Difficulty.Hard => 6,
                _ => throw new NotImplementedException(),
            };
        }

        #endregion

        #region Private members

        // HitFloor
        private void HitFloor()
        {
            PlaySound(SoundNames.WreckingBallImpact);
            Session.Camera.Shake(TweenStyle.QuadraticInOut, new(.08f, 1), 40, 6);

            if (Room == null || Definition == null)
                return;

            var hitArea = new RectangleF(X - 5, Y - 3, 10, 7);
            for (var i = 0; i < Room.Children.Count; i++)
            {
                if (Room.Children[i] == this)
                    continue;

                if (Room.Children[i] is GameThing target && !target.IsDead && target.MaxHP > 0)
                {
                    if (hitArea.Contains(target.Position))
                    {
                        if (RuntimeHotspot.BoundingRectangleF.Intersects(target.RuntimeHotspot.BoundingRectangleF))
                            EffectDescriptor.Apply(Definition.EffectDescriptors, this, target, EffectContext.Contact);
                    }
                }
            }

            CollisionDetection = true;
        }

        #endregion

        #region Protected members

        // OnStateEnter
        protected override void OnStateEnter(TrapState state)
        {
            if (Room?.WalkArea == null)
                return;

            if (state == TrapState.Idle)
            {
                Y = 0;
                Shadow.Scale = Vector2.Zero;
                CollisionDetection = false;
            }
            else if (state == TrapState.Active)
            {
                Tweens.YTween = FloatTween.Create(TweenStyle.QuadraticIn, Y, hitPosition.Y, 200, HitFloor);
                Shadow.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.QuadraticIn, Shadow.Scale, Vector2.One, 200);
            }

            else if (state == TrapState.Cooldown)
            {
                PlaySound(SoundNames.WreckingBallChain);
                Tweens.YTween = FloatTween.Create(TweenStyle.QuadraticIn, Y, 0, 1500);
                Shadow.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.QuadraticIn, Shadow.Scale, Vector2.Zero, 1500);
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
                DefaultImageName = $"WreckingBallCrack{Random.Shared.Next(1, 4)}";
                PivotOrigin = Engendro.RectanglePoint.Center;
                RenderLayer = RenderLayer.Background;
            }
        }
    }
}
