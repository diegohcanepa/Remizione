using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione
{
    /// <summary>
    /// EchoMessage
    /// </summary>
    public sealed class EchoMessage : GameObject
    {
        private readonly FloatTween opacityTween = new();
        private readonly TextSprite textSprite;

        #region Constructor

        // Constructor
        public EchoMessage(RemizioneGame game)
            : base(game)
        {
            this.textSprite = new TextSprite(Game, Fonts.Common)
            {
                Color = ColorPalette.Text.Default,
                MaximumWidth = (int)(Screen.NativeWidth * .7f),
                PauseOnPunctuationMarks = false,
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.Area.GetPoint(RectanglePoint.Bottom, 0, -10),
                Scale = ScaleInfo.Text.VeryLarge
            };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            textSprite.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            textSprite.Update(gameTime);
        }

        #endregion

        // Hide
        public void Hide()
        {
            if (textSprite.Opacity > 0)
            {
                if (!opacityTween.IsRunning || opacityTween.EndValue != 0)
                {
                    opacityTween.Start(TweenStyle.CubicIn, 1, 0, 500);
                    textSprite.Tweens.OpacityTween = opacityTween;
                }
            }
        }

        // IsVisible
        public bool IsVisible => textSprite.Opacity > 0;

        // Show
        public void Show(string text)
        {
            textSprite.Text = text;
            opacityTween.Start(TweenStyle.CubicIn, 0, 1, 500);
            textSprite.Tweens.OpacityTween = opacityTween;
            textSprite.StartTyping();
        }
    }
}
