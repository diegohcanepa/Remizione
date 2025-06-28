using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione
{
    /// <summary>
    /// HUDMessage
    /// </summary>
    public sealed class HUDMessage : GameObject
    {
        private readonly FloatTween fadeTween = new() { StartDelay = 1500 };
        private readonly TextSprite messageText;

        // Constructor
        public HUDMessage(EngendroGame game)
            : base(game)
        {
            this.messageText = new(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                MaximumWidth = (int)(Screen.HUDArea.Width * .7f),
                PivotOrigin = RectanglePoint.Top,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.Top, 0, 5),
                Scale = ScaleInfo.Text.VeryLarge
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!fadeTween.IsRunning)
                return;

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            messageText.Draw(gameTime);
            Game.SpriteBatch.End();
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
        public void Hide() => fadeTween.Stop();

        // Show
        public void Show(HUDMessageKind message, bool isWarning)
        {
            messageText.Text = Localization.GetValue(message);
            fadeTween.Start(TweenStyle.CubicIn, 1, 0, 1000);
            if (isWarning)
            {
                messageText.Color = ColorPalette.Text.Highlight;
                Sound.Play(SoundNames.Error);
            }
            else
                messageText.Color = ColorPalette.Text.Default;
        }
    }
}
