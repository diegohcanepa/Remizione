using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione
{
    /// <summary>
    /// UILog
    /// </summary>
    public sealed class UILog : GameObject
    {
        private readonly FloatTween fadeTween = new() { StartDelay = 2600 };
        private readonly Sprite icon;
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

            // Text sprite
            this.textSprite = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Terra,
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
        }

        #endregion

        // Hide
        public void Hide()
        {
            fadeTween.Stop();
        }

        // Show
        public void Show(string displayName, AtlasImage image)
        {
            textSprite.Color = ColorPalette.Text.Highlight;
            textSprite.Position = Screen.HUDArea.GetPoint(RectanglePoint.Bottom, 0, -1);
            textSprite.Text = displayName;

            icon.RenderImage = image;
            icon.Position = textSprite.BoundingBox.GetPoint(RectanglePoint.Top);

            fadeTween.Start(TweenStyle.CubicIn, 1, 0, 1000);
        }
    }
}
