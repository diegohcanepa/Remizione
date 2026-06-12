using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScaryCastle
{
    /// <summary>
    /// UICoinMeter
    /// </summary>
    public class UICoinMeter : GameObject
    {
        private bool isInitialized;
        private readonly Sprite icon;
        private readonly Vector2 iconScale = Vector2.One;
        private int lastKnownValue = -1;
        private readonly FloatTween rotationTween = new();
        private readonly Vector2Tween scaleTween = new();
        private readonly GameSession session;
        private readonly TextSprite valueText;

        // Constructor
        public UICoinMeter(GameSession session)
            : base()
        {
            this.session = session;

            // Icon
            this.icon = new(Atlases.UI.CoinIcon)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.Area.GetPoint(RectanglePoint.RightBottom, -7, -14),
            };

            // Score text
            this.valueText = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Right,
                Position = icon.BoundingBox.GetPoint(RectanglePoint.Left, -1, 1),
                Scale = ScaleInfo.Text.VeryLarge
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            icon.Draw(gameTime);
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            valueText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (lastKnownValue != session.Coins)
            {
                if (isInitialized)
                    Sound.Play(SoundNames.CollectCoin);

                isInitialized = true;
                lastKnownValue = session.Coins;
                valueText.Text = $"{session.Coins}";

                if (!icon.Tweens.IsTweening)
                {
                    rotationTween.Start(TweenStyle.QuadraticInOut, 0, 15, 50, 6);
                    icon.Tweens.RotationTween = rotationTween;

                    scaleTween.Start(TweenStyle.QuadraticInOut, iconScale, iconScale * 1.3f, 150, 2);
                    icon.Tweens.ScaleTween = scaleTween;
                }
            }

            icon.Update(gameTime);
        }

        #endregion

        // IconBoundingBox
        public RectangleF IconBoundingBox => icon.BoundingBox;
    }
}
