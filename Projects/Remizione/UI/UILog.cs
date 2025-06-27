using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Remizione.UI
{
    /// <summary>
    /// UILog
    /// </summary>
    public sealed class UILog : GameObject
    {
        private readonly FloatTween fadeTween = new() { StartDelay = 1500 };
        private readonly Queue<(string verb, string noun, bool isWarning)> queue = [];
        private readonly TextSprite nounText;
        private readonly TextSprite verbText;

        // Constructor
        public UILog(EngendroGame game)
            : base(game)
        {
            this.verbText = new(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Left,
                Scale = ScaleInfo.Text.Large
            };

            this.nounText = new(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default * .7f,
                PivotOrigin = RectanglePoint.LeftTop,
                Scale = ScaleInfo.Text.Large
            };
        }

        #region Private members

        // ShowCore
        private void ShowCore(string verb, string noun, bool isWarning)
        {
            if (isWarning)
            {
                queue.Clear();
            }
            else if (fadeTween.IsRunning)
            {
                queue.Enqueue(new(verb, noun, isWarning));
                return;
            }

            verbText.Color = isWarning ? ColorPalette.Text.Highlight : ColorPalette.Text.Default;
            verbText.Position = new Vector2(5, 28);
            verbText.Text = verb;

            nounText.Position = verbText.BoundingBox.GetPoint(RectanglePoint.LeftBottom);
            nounText.Text = noun;

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
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            fadeTween.Update(gameTime);
            verbText.Update(gameTime);
            nounText.Update(gameTime);

            verbText.Opacity = fadeTween.IsRunning ? fadeTween.CurrentValue : 1;
            nounText.Opacity = fadeTween.IsRunning ? fadeTween.CurrentValue : 1;

            if (!fadeTween.IsRunning && queue.Count > 0)
            {
                var log = queue.Dequeue();
                ShowCore(log.verb, log.noun, log.isWarning);
            }
        }

        #endregion

        // Hide
        public void Hide() => fadeTween.Stop();

        // Show
        public void Show(LogMessage message, bool isWarning) => ShowCore(Localization.GetValue(message), string.Empty, isWarning);

        // Show
        public void Show(LogVerb verb, string noun) => ShowCore(Localization.GetValue(verb), noun, false);
    }
}
