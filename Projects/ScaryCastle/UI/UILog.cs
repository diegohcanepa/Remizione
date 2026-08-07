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
        private readonly TextSprite textSprite;

        // Constructor
        public UILog()
        {
            // Icon
            this.icon = new Sprite()
            {
                PivotOrigin = RectanglePoint.Bottom,
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

            // Text sprite
            this.textSprite = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Bottom,
                Scale = ScaleInfo.Text.Huge
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!fadeTween.IsRunning)
                return;

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            textSprite.Draw(gameTime);
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
            textSprite.Update(gameTime);
            icon.Update(gameTime);

            textSprite.Opacity = fadeTween.IsRunning ? fadeTween.CurrentValue : 1;
            icon.Opacity = textSprite.Opacity;
            iconShadow.Opacity = textSprite.Opacity * ColorPalette.ShadowOpacity;
        }

        #endregion

        // Hide
        public void Hide()
        {
            fadeTween.Stop();
        }

        // Show
        public void Show(Item item, bool isWarning = false)
        {
            textSprite.Color = isWarning ? ColorPalette.Text.Terra : ColorPalette.Text.Highlight;
            textSprite.Position = Screen.HUDArea.GetPoint(RectanglePoint.Bottom, 0, -1);
            textSprite.Text = item.DisplayName;

            icon.RenderImage = item.Definition.Image;
            icon.Position = textSprite.BoundingBox.GetPoint(RectanglePoint.Top);

            iconShadow.Position = icon.BoundingBox.Center;
            iconShadow.RenderImage = item.Definition.Image;
            iconShadow.X -= 1.5f;
            iconShadow.Y += .5f;

            fadeTween.Start(TweenStyle.CubicIn, 1, 0, 1000);
        }
    }
}
