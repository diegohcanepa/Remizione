using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// LargeHand
    /// </summary>
    public sealed class LargeHand : GameObject
    {
        private readonly AnimatedSprite handSprite;
        private Vector2 origin;
        private GameThing? target;
        private enum HitState { Idle, Entering, Hitting, Exiting }
        private HitState currentState;
        private readonly Vector2Tween positionTween = new();

        // Constructor
        public LargeHand()
            : base()
        {
            this.handSprite = new()
            {
                Atlas = Atlases.Environment,
                PivotOrigin = RectanglePoint.RightTop,
                Scale = new(.24f)
            };

            var prefix = "Devil";

            var anim = handSprite.AddAnimation("Idle");
            anim.AddFrame($"{prefix}Hand01", 1000);

            anim = handSprite.AddAnimation("Hit");
            anim.AddFrame($"{prefix}Hand02", 100);
            anim.AddFrame($"{prefix}Hand03", 100);
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            handSprite.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            handSprite.Update(gameTime);

            if (currentState == HitState.Entering)
            {
                if (!handSprite.Tweens.IsTweeningPosition)
                {
                    currentState = HitState.Hitting;
                    handSprite.Player.Play("Hit", false);
                }
            }

            else if (currentState == HitState.Hitting)
            {
                if (!handSprite.Player.IsPlaying)
                {
                    currentState = HitState.Exiting;
                    positionTween.Start(TweenStyle.CubicIn, handSprite.Position, origin, 400);
                    handSprite.Tweens.PositionTween = positionTween;
                    if (target != null)
                    {
                        target.Die();
                        Game.Camera.Shake(TweenStyle.Linear, Vector2.One, 30, 6);
                    }
                }
            }

            else if (currentState == HitState.Exiting)
            {
                if (!handSprite.Tweens.IsTweeningPosition)
                {
                    currentState = HitState.Idle;
                }
            }
        }

        #endregion

        // Hit
        public void Hit(GameThing target)
        {
            this.target = target;

            if (target.X > Screen.NativeWidth / 2)
            {
                origin = new(241, 0);
                handSprite.PivotOrigin = RectanglePoint.LeftTop;
                handSprite.FlipLeft();
            }
            else
            {
                origin = new(-2, 0);
                handSprite.PivotOrigin = RectanglePoint.RightTop;
                handSprite.FlipRight();
            }

            handSprite.Position = origin;

            handSprite.Player.Play("Idle");

            var targetPosition = target.RuntimeHotspot.BoundingRectangleF.Center;
            targetPosition.Y -= handSprite.BoundingBox.Height * .95f;

            var startPos = origin;
            startPos.Y -= 20;

            positionTween.Start(TweenStyle.CubicOut, startPos, targetPosition, 400);
            handSprite.Tweens.PositionTween = positionTween;
            currentState = HitState.Entering;

            Sound.Play(SoundNames.WhooshA);
        }

        // IsBusy
        public bool IsBusy => currentState != HitState.Idle;
    }
}