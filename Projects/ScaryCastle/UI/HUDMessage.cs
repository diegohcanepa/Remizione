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
        public HUDMessage(RectanglePoint pivotOrigin, Vector2 position, Vector2 scale)
        {
            this.scale = scale;

            // Message text
            this.messageText = new(Fonts.CommonOutline)
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
        public void Show(MessageKind message, int duration = 2000)
        {
            var text = Localization.GetValue(message);

            var color = ColorPalette.Text.Highlight;
            if (message is MessageKind.NotEnoughCoins)
            {
                color = ColorPalette.Text.Orange;
                Sound.Play(SoundNames.Error);
            }
            else if (message == MessageKind.Cursed)
            {
                color = ColorPalette.Text.Purple;
            }
            else if (message == MessageKind.SacrificeDone)
            {
                color = ColorPalette.Text.SteelBlue;
            }
            else if (message == MessageKind.Poisoned)
            {
                color = ColorPalette.Text.Green;
            }
            else
            {
                Sound.Play(SoundNames.Error);
            }

            Show(text, color, duration);
        }

        // Show
        public void Show(string text, int duration = 2000)
        {
            Show(text, ColorPalette.Text.Highlight, duration);
        }

        // Show
        public void Show(string text, Color color, int duration = 2000)
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
