using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// HUDMessage
    /// </summary>
    public sealed class HUDMessage : GameObject
    {
        private readonly FloatTween fadeTween = new();
        private readonly TextSprite messageText;
        private readonly Vector2 scale;
        private readonly Vector2Tween scaleTween = new();

        // Constructor
        public HUDMessage(EngendroGame game, RectanglePoint pivotOrigin, Vector2 position, Vector2 scale)
            : base(game)
        {
            this.scale = scale;

            // Message text
            this.messageText = new(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                MaximumWidth = (int)(Screen.HUDArea.Width * .7f),
                PivotOrigin = pivotOrigin,
                Position = position
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!fadeTween.IsRunning)
                return;

            messageText.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            fadeTween.Update(gameTime);
            messageText.Update(gameTime);
            messageText.Opacity = fadeTween.IsRunning ? fadeTween.CurrentValue : 1;
        }

        #endregion

        // Hide
        public void Hide()
        {
            fadeTween.Stop();
        }

        // Show
        public void Show(MessageKind message, int duration = 1500)
        {
            var text = Localization.GetValue(message);
            var color = ColorPalette.Text.Highlight;

            if (message is MessageKind.NotEnoughCoins)
            {
                color = ColorPalette.Text.Orange;
                Sound.Play(SoundNames.Error);
            }
            else if (message == MessageKind.Courage)
            {
                color = ColorPalette.Text.Green;
                Sound.Play(SoundNames.Error);
            }
            else
            {
                color = ColorPalette.Text.Highlight;
                Sound.Play(SoundNames.Error);
            }

            Show(text, color, duration);
        }

        // Show
        public void Show(string text, int duration = 1500)
        {
            Show(text, ColorPalette.Text.Highlight, duration);
        }

        // Show
        public void Show(string text, Color color, int duration = 1500)
        {
            messageText.Color = color;
            messageText.Text = text;
            fadeTween.StartDelay = duration;
            fadeTween.Start(TweenStyle.CubicIn, 1, 0, 200);
            scaleTween.Start(TweenStyle.CubicIn, scale * .8f, scale, 50);
            messageText.Tweens.ScaleTween = scaleTween;
        }
    }
}
