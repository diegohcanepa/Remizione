using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// FloatingText
    /// </summary>
    public sealed class FloatingText : GameObject
    {
        private const int fadeDuration = 300;

        private readonly FloatTween opacityTween = new();
        private readonly GameSession session;
        private readonly TextSprite text;
        private readonly FloatTween yTween = new();

        // Constructor
        public FloatingText(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            this.text = new TextSprite(session.Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                Scale = ScaleInfo.Text.Huge,
                PivotOrigin = RectanglePoint.Bottom
            };
        }

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
        public bool IsVisible => yTween.IsRunning || opacityTween.IsRunning;

        // MessageId
        public int MessageId { get; set; }

        // Show
        public void Show(Vector2 origin, string value) => Show(origin, value, ColorPalette.Text.Default);

        // Show
        public void Show(Vector2 origin, string value, Color color, int duration = 1000)
        {
            if (duration < fadeDuration)
                duration = fadeDuration;

            text.Color = color;
            text.Text = value;
            text.Position = origin;
            yTween.Start(TweenStyle.CubicOut, origin.Y, origin.Y - 2, duration);
            opacityTween.StartDelay = duration - fadeDuration;
            opacityTween.Start(TweenStyle.CubicIn, 1, 0, fadeDuration);

            text.Tweens.OpacityTween = opacityTween;
            text.Tweens.YTween = yTween;
        }
    }
}
