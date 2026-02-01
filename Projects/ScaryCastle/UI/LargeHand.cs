using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace ScaryCastle.UI
{
    /// <summary>
    /// LargeHand
    /// </summary>
    public sealed class LargeHand : GameObject
    {
        private readonly AnimatedSprite handSprite;
        private readonly Vector2 origin;
        private readonly FighterInfo source;
        private readonly FighterInfo target;
        private enum HitState { Idle, Entering, Hitting, Exiting }
        private HitState currentState;
        private readonly Vector2Tween positionTween = new();

        // Constructor
        public LargeHand(EngendroGame game, LargHandStyle style, FighterInfo source, FighterInfo target)
            : base(game)
        {
            this.Style = style;
            this.source = source;
            this.target = target;

            this.handSprite = new(Game)
            {
                Atlas = Atlases.UI,
                PivotOrigin = style == LargHandStyle.God ? RectanglePoint.LeftTop : RectanglePoint.RightTop,
                Scale = new(.24f)
            };

            var prefix = style.ToString();

            var anim = handSprite.AddAnimation("Idle");
            anim.AddFrame($"{prefix}Hand01", 1000);

            anim = handSprite.AddAnimation("Hit");
            anim.AddFrame($"{prefix}Hand02", 100);
            anim.AddFrame($"{prefix}Hand03", 100);

            origin = style == LargHandStyle.God ? new(241, 0) : new(-2, 0);

            handSprite.Position = origin;
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
                    if (source.Card is Card card)
                    {
                        card.Definition.Apply(source.Actor, target.Actor);
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
        public void Hit()
        {
            handSprite.Player.Play("Idle");

            var targetPosition = target.Actor.RuntimeHotspot.BoundingRectangleF.Center;
            targetPosition.Y -= handSprite.BoundingBox.Height * .95f;

            var startPos = origin;
            startPos.Y -= 20;

            positionTween.Start(TweenStyle.CubicOut, startPos, targetPosition, 200);
            handSprite.Tweens.PositionTween = positionTween;
            currentState = HitState.Entering;

            Sound.Play(SoundNames.WhooshA);
        }

        // Style
        public LargHandStyle Style { get; }
    }
}
