using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScaryCastle
{
    /// <summary>
    /// UILog
    /// </summary>
    public sealed class UILog : GameObject
    {
        private readonly FloatTween fadeTween = new() { StartDelay = 2000 };
        private readonly ImageSprite icon;
        private readonly TextSprite nounText;
        private readonly TextSprite verbText;

        // Constructor
        public UILog(EngendroGame game)
            : base(game)
        {
            // Icon
            this.icon = new ImageSprite(game)
            {
                PivotOrigin = RectanglePoint.LeftTop,
                Scale = ScaleInfo.UIElement.Small
            };

            // Verb
            this.verbText = new(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.LeftTop,
                Scale = ScaleInfo.Text.Giant
            };

            // Noun
            this.nounText = new(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.LeftTop,
                Scale = ScaleInfo.Text.VeryLarge
            };
        }

        #region Private members

        // ShowCore
        private void ShowCore(string verb, string noun, bool isWarning, AtlasImage? image)
        {
            verbText.Color = isWarning ? ColorPalette.Text.Orange : ColorPalette.Text.Green;
            verbText.Position = new Vector2(8, 20);
            verbText.Text = verb;

            nounText.Position = verbText.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 0, -2);
            nounText.Text = noun;
            icon.Image = image;
            icon.Position = nounText.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 0, -1);

            fadeTween.Start(TweenStyle.CubicIn, 1, 0, 1000);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!fadeTween.IsRunning)
                return;

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            verbText.Draw(gameTime);
            nounText.Draw(gameTime);
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera);
            icon.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            fadeTween.Update(gameTime);
            verbText.Update(gameTime);
            nounText.Update(gameTime);
            icon.Update(gameTime);

            verbText.Opacity = fadeTween.IsRunning ? fadeTween.CurrentValue : 1;
            nounText.Opacity = verbText.Opacity;
            icon.Opacity = verbText.Opacity;
        }

        #endregion

        // Hide
        public void Hide()
        {
            fadeTween.Stop();
        }

        // Show
        public void Show(string message, bool isWarning, AtlasImage? image = null)
        {
            ShowCore(message, string.Empty, isWarning, image);
        }

        // Show
        public void Show(LogVerb verb, string noun, bool isWarning, AtlasImage? image = null)
        {
            ShowCore(Localization.GetValue(verb), noun, isWarning, image);
        }

        // Show
        public void Show(LogVerb verb, MetaItem metaItem)
        {
            var isWarning = verb is LogVerb.Used or LogVerb.Needs;

            ShowCore(Localization.GetValue(verb), metaItem.LocalizedDisplayName, isWarning, metaItem.Image);

            if (verb == LogVerb.Found)
            {
                if (metaItem.PickupSound != null)
                    metaItem.PickupSound.Play();
                else
                    Sound.Play(SoundNames.PickupGeneric);
            }

            else if (verb == LogVerb.Needs)
                Sound.Play(SoundNames.Error);
        }
    }
}
