using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScaryCastle
{
    /// <summary>
    /// HUDMessage
    /// </summary>
    public sealed class HUDMessage : GameObject
    {
        private readonly Sprite container = new(Atlases.UI.GetImage("MessageContainer")) { PivotOrigin = RectanglePoint.Center };
        private readonly FloatTween fadeTween = new();
        private readonly Sprite icon = new() { PivotOrigin = RectanglePoint.Right };
        private readonly TextSprite messageText;
        private readonly Vector2 scale = ScaleInfo.Text.Huge;
        private readonly Vector2Tween scaleTween = new();

        // Constructor
        public HUDMessage(RectanglePoint pivotOrigin, Vector2 position)
        {
            this.messageText = new(Fonts.Common)
            {
                Color = ColorPalette.Text.Sentence,
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

            Game.SpriteBatch.Begin(Game.Camera);
            container.Draw(gameTime);
            icon.Draw(gameTime);
            Game.SpriteBatch.End();

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
            icon.Opacity = fadeTween.CurrentValue;
            icon.Position = messageText.BoundingBox.GetPoint(RectanglePoint.Left, -2, -1);

            if (IsVisible && !fadeTween.IsRunning)
                IsVisible = false;
        }

        #endregion

        // Hide
        public void Hide()
        {
            fadeTween.Stop();
            IsVisible = false;
        }

        // IsVisible
        public bool IsVisible { get; private set; }

        // Show
        public void Show(MessageKind message, int duration = 2000)
        {
            var text = Localization.GetValue(message);
            icon.RenderImage = null;

            var color = ColorPalette.Text.Highlight;

            if (message is MessageKind.NotEnoughCoins or MessageKind.ItemDiscarded or MessageKind.LiftNotAllowed)
            {
                Sound.Play(SoundNames.Error);
            }

            else if (message == MessageKind.ExtraEnergy)
            {
                icon.RenderImage = Atlases.UI.GooIcon;
            }

            else if (message == MessageKind.ExtraHeart)
            {
                icon.RenderImage = Atlases.UI.HeartIcon;
            }

            else if (message == MessageKind.NotEnoughFaith)
            {
                icon.RenderImage = Atlases.UI.FaithIcon;
                Sound.Play(SoundNames.Error);
            }

            else if (message == MessageKind.InventoryFull)
            {
                icon.RenderImage = Atlases.UI.Sack;
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
            container.Position = messageText.Position;
            container.Y += 3;
            IsVisible = true;
        }
    }
}
