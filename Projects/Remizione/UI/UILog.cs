using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// UILog
    /// </summary>
    public sealed class UILog : GameObject
    {
        private readonly FloatTween fadeTween = new() { StartDelay = 1500 };
        private readonly ImageSprite icon;
        private readonly Queue<(string verb, string noun, bool isWarning, AtlasImage? image)> queue = [];
        private readonly TextSprite nounText;
        private readonly TextSprite verbText;

        // Constructor
        public UILog(EngendroGame game)
            : base(game)
        {
            this.icon = new ImageSprite(game)
            {
                PivotOrigin = RectanglePoint.LeftTop,
                Scale = ScaleInfo.UIElement.Medium
            };

            this.verbText = new(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Left,
                Scale = ScaleInfo.Text.VeryLarge
            };

            this.nounText = new(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default * .7f,
                PivotOrigin = RectanglePoint.LeftTop,
                Scale = ScaleInfo.Text.VeryLarge
            };
        }

        #region Private members

        // ShowCore
        private void ShowCore(string verb, string noun, bool isWarning, AtlasImage? image)
        {
            if (isWarning)
            {
                queue.Clear();
            }
            else if (fadeTween.IsRunning)
            {
                queue.Enqueue(new(verb, noun, isWarning, image));
                return;
            }

            verbText.Color = isWarning ? ColorPalette.Text.Highlight : ColorPalette.Text.Default;
            verbText.Position = new Vector2(5, 25);
            verbText.Text = verb;

            nounText.Position = verbText.BoundingBox.GetPoint(RectanglePoint.LeftBottom);
            nounText.Text = noun;
            icon.Image = image;
            icon.Position = nounText.BoundingBox.GetPoint(RectanglePoint.LeftBottom);

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

            if (!fadeTween.IsRunning && queue.Count > 0)
            {
                var (verb, noun, isWarning, image) = queue.Dequeue();
                ShowCore(verb, noun, isWarning, image);
            }
        }

        #endregion

        // Hide
        public void Hide() => fadeTween.Stop();

        // Show
        public void Show(string message, bool isWarning, AtlasImage? image = null) => ShowCore(message, string.Empty, isWarning, image);

        // Show
        public void Show(LogVerb verb, string noun, AtlasImage? image = null) => ShowCore(Localization.GetValue(verb), noun, false, image);
    }
}
