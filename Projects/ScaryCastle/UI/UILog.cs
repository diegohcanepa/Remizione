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
                PivotOrigin = RectanglePoint.Top,
                Scale = ScaleInfo.UIElement.Medium
            };

            // Icon shadow
            iconShadow = new()
            {
                Color = Color.Black,
                Opacity = ColorPalette.ShadowOpacity,
                PivotOrigin = RectanglePoint.Center,
                Scale = ScaleInfo.UIElement.Medium,
            };

            // Verb
            this.verbText = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Top,
                Scale = ScaleInfo.Text.VeryLarge
            };

            // Noun
            this.nounText = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Top,
                Scale = ScaleInfo.Text.Large
            };
        }

        #region Private members

        // ShowCore
        private void ShowCore(string verb, string noun, bool isWarning, AtlasImage? image)
        {
            verbText.Color = isWarning ? ColorPalette.Text.Terra : ColorPalette.Text.Green;
            verbText.Position = new Vector2(Screen.Center.X, 5);
            verbText.Text = verb;

            nounText.Position = verbText.BoundingBox.GetPoint(RectanglePoint.Bottom);
            nounText.Text = noun;
            icon.RenderImage = image;
            icon.Position = nounText.BoundingBox.GetPoint(RectanglePoint.Bottom, 2, -1);

            iconShadow.Position = icon.BoundingBox.Center;
            iconShadow.RenderImage = image;
            iconShadow.X -= 1.5f;
            iconShadow.Y += .5f;

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
            iconShadow.Draw(gameTime);
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
            iconShadow.Opacity = verbText.Opacity * ColorPalette.ShadowOpacity;
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
            if (verb == LogVerb.None)
                return;

            ShowCore(Localization.GetValue(verb), itemDefinition.DisplayName, isWarning, itemDefinition.Image);

            if (verb == LogVerb.Requires)
                Sound.Play(SoundNames.Error);
        }
    }
}
