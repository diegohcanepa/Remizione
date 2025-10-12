using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// FloatingText
    /// </summary>
    public sealed class FloatingText : GameObject
    {
        private const int fadeDuration = 250;
        private static readonly float defaultScale = ScaleInfo.Text.Huge.X;

        private readonly FloatTween opacityTween = new();
        private readonly GameSession session;
        private readonly TextSprite text;
        private readonly FloatTween xTween = new();
        private readonly FloatTween yTween = new();

        // Constructor
        public FloatingText(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            this.text = new TextSprite(session.Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Bottom
            };
        }

        #region Private members

        // ShowCore
        private void ShowCore(Vector2 origin, string value, Color color, Vector2 distance, float scale, int duration)
        {
            if (duration < fadeDuration)
                duration = fadeDuration;

            text.Color = color;
            text.Scale = new(scale);
            text.Text = value;
            text.Position = origin;

            if (distance.Y != 0)
            {
                yTween.Start(TweenStyle.CubicOut, origin.Y, origin.Y + distance.Y, duration);
                text.Tweens.YTween = yTween;
            }

            if (distance.X != 0)
            {
                xTween.Start(TweenStyle.CubicOut, origin.X, origin.X + distance.X, duration);
                text.Tweens.XTween = xTween;
            }

            opacityTween.StartDelay = duration - fadeDuration;
            opacityTween.Start(TweenStyle.CubicIn, 1, 0, fadeDuration);
            text.Tweens.OpacityTween = opacityTween;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            text.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            text.Update(gameTime);

            if (!IsVisible)
                session.ObjectPools.FloatingTexts.Return(this);
        }

        #endregion

        // IsVisible
        public bool IsVisible => xTween.IsRunning || yTween.IsRunning || opacityTween.IsRunning;

        // Show
        public void Show(Vector2 origin, string value, Color color, int duration = 1000)
        {
            Show(origin, value, color, defaultScale, duration);
        }

        // Show
        public void Show(Vector2 origin, string value, Color color, float scale, int duration = 1000)
        {
            ShowCore(origin, value, color, new Vector2(Randomizer.Next(-5, 5), Randomizer.Next(-12, -1)), scale, duration);
        }

        // Show
        public void Show(Vector2 origin, string value, bool critical)
        {
            var color = critical ? ColorPalette.Text.Highlight : ColorPalette.Text.Default;
            ShowCore(origin, value, color, new(0, -6), defaultScale, 700);
        }
    }
}
