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
            var distance = new Vector2(Randomizer.Next(-8, 8), Randomizer.Next(-22, -10));
            var duration = Randomizer.Next(1000, 3000);
            var fadeDuration = Randomizer.Next(500, 1400);

            image.Image = half ? Atlases.UI.MiniHeartHalfIcon : Atlases.UI.MiniHeartIcon;

            yTween.Start(TweenStyle.Linear, origin.Y, origin.Y + distance.Y, duration);

            if (distance.X != 0)
                xTween.Start(TweenStyle.CubicOut, origin.X, origin.X + distance.X, duration);

            opacityTween.StartDelay = duration - fadeDuration;
            opacityTween.Start(TweenStyle.CubicIn, 1, 0, fadeDuration);

            rotationTween.Start(TweenStyle.CubicOut, -4, 4, 100, -1);

            image.Tweens.XTween = xTween;
            image.Tweens.YTween = yTween;
            image.Tweens.OpacityTween = opacityTween;
            image.Tweens.RotationTween = rotationTween;
        }
    }
}
