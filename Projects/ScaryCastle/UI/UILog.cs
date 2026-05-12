using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// UILog
    /// </summary>
    public sealed class UILog : GameObject
    {
        private readonly FloatTween fadeTween = new() { StartDelay = 2600 };
        private readonly Sprite icon;
        private readonly Sprite iconShadow;
        private readonly TextSprite nounText;
        private readonly TextSprite verbText;

        // Constructor
        public UILog()
        {
            // Icon
            this.icon = new Sprite()
            {
                PivotOrigin = RectanglePoint.LeftTop,
                Scale = ScaleInfo.UIElement.Medium
            };

            // Icon shadow
            iconShadow = new()
            {
                Color = Color.Black,
                Opacity = ColorPalette.ShadowOpacity,
                PivotOrigin = RectanglePoint.LeftTop,
                Scale = ScaleInfo.UIElement.Medium,
            };

            // Verb
            this.verbText = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.LeftTop,
                Scale = ScaleInfo.Text.Giant
            };

            // Noun
            this.nounText = new(Fonts.CommonOutline)
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
            verbText.Position = new Vector2(6, 12);
            verbText.Text = verb;

            nounText.Position = verbText.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 0, -2);
            nounText.Text = noun;
            icon.RenderImage = image;
            icon.Position = nounText.BoundingBox.GetPoint(RectanglePoint.LeftBottom);

            iconShadow.Position = icon.Position;
            iconShadow.RenderImage = image;
            iconShadow.X -= 1;
            iconShadow.Y += 1;

            fadeTween.Start(TweenStyle.CubicIn, 1, 0, 1000);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!fadeTween.IsRunning)
                return;

            verbText.Draw(gameTime);
            nounText.Draw(gameTime);

            iconShadow.Draw(gameTime);
            icon.Draw(gameTime);
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
            iconShadow.Opacity = verbText.Opacity;
        }

        #endregion

        // Hide
        public void Hide()
        {
            fadeTween.Stop();
        }

        // Show
        public void Show(LogVerb verb, ItemDefinition itemDefinition, bool isWarning = false)
        {
            ShowCore(Localization.GetValue(verb), itemDefinition.DisplayName, isWarning, itemDefinition.Image);

            if (verb == LogVerb.Requires)
                Sound.Play(SoundNames.Error);
        }
    }
}
