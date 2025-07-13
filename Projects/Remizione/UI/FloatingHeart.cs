using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
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
        private readonly FloatTween xTween = new();
        private readonly FloatTween yTween = new();

        // Constructor
        public FloatingHeart(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            this.image = new(Game, Atlases.UI.MiniHeartIcon)
            {
                PivotOrigin = RectanglePoint.Middle,
                Scale = ScaleInfo.UIElement.Medium
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
            var distance = new Vector2(Randomizer.Next(-13, 13), Randomizer.Next(-22, -10));
            var duration = Randomizer.Next(1000, 3000);

            image.Image = half ? Atlases.UI.MiniHeartHalfIcon : Atlases.UI.MiniHeartIcon;

            yTween.Start(TweenStyle.CubicOut, origin.Y, origin.Y + distance.Y, duration * 2);

            //if (distance.X != 0)
                xTween.Start(TweenStyle.CubicOut, origin.X, origin.X + distance.X, duration);

            opacityTween.StartDelay = (int)(duration * .9f);
            opacityTween.Start(TweenStyle.CubicIn, 1, 0, duration - opacityTween.StartDelay);

            var r = Randomizer.Next(6, 10);
            rotationTween.Start(TweenStyle.Linear, -r, r, 100, -1);

            scaleTween.Start(TweenStyle.Linear, Vector2.Zero, ScaleInfo.UIElement.Medium, Randomizer.Next(100, 300));

            image.Tweens.ScaleTween = scaleTween;
            image.Tweens.XTween = xTween;
            image.Tweens.YTween = yTween;
            image.Tweens.OpacityTween = opacityTween;
            image.Tweens.RotationTween = rotationTween;
        }
    }
}
