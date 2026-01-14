using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// FloatingHeart
    /// </summary>
    public sealed class FloatingHeart : GameObject
    {
        private readonly ImageSprite image;
        private readonly FloatTween opacityTween = new();
        private readonly FloatTween rotationTween = new();
        private readonly Vector2Tween scaleTween = new();
        private readonly GameSession session;
        private static bool spawnLeft;
        private readonly FloatTween xTween = new();
        private readonly FloatTween yTween = new();

        // Constructor
        public FloatingHeart(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            this.image = new(Game)
            {
                PivotOrigin = RectanglePoint.Center,
                Scale = ScaleInfo.UIElement.Large
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            image.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (!IsVisible)
                session.ObjectPools.FloatingHearts.Return(this);
            else
                image.Update(gameTime);
        }

        #endregion

        // IsVisible
        public bool IsVisible => xTween.IsRunning || yTween.IsRunning || opacityTween.IsRunning;

        // Show
        public void Show(Vector2 origin, bool half)
        {
            var distance = new Vector2(Random.Shared.Next(3, 14), Random.Shared.Next(-10, -5));

            if (spawnLeft)
                distance.X *= -1;

            var duration = Random.Shared.Next(1000, 2500);

            image.Image = half ? Atlases.UI.HeartHalf : Atlases.UI.Heart;

            yTween.Start(TweenStyle.CubicOut, origin.Y, origin.Y + distance.Y, duration * 2);
            xTween.Start(TweenStyle.CubicOut, origin.X, origin.X + distance.X, duration);

            opacityTween.StartDelay = (int)(duration * .9f);
            opacityTween.Start(TweenStyle.CubicIn, 1, 0, duration - opacityTween.StartDelay);

            var r = Random.Shared.Next(6, 11);
            rotationTween.Start(TweenStyle.Linear, -r, r, 100, -1);

            scaleTween.Start(TweenStyle.Linear, Vector2.Zero, ScaleInfo.UIElement.Large, Random.Shared.Next(100, 300));

            image.Tweens.ScaleTween = scaleTween;
            image.Tweens.XTween = xTween;
            image.Tweens.YTween = yTween;
            image.Tweens.OpacityTween = opacityTween;
            image.Tweens.RotationTween = rotationTween;

            spawnLeft = !spawnLeft;
        }
    }
}
