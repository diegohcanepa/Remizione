using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione.UI
{
    /// <summary>
    /// UITokens
    /// </summary>
    public class UITokens : GameObject
    {
        private readonly UIScore score;
        private readonly TextSprite title;

        // Constructor
        public UITokens(EngendroGame game)
            : base(game)
        {
            // Title
            this.title = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightTop),
                Scale = ScaleInfo.Text.ExtraLarge,
                Text = "Tokens"
            };

            // Score
            this.score = new UIScore(game, title.Color)
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = title.BoundingBox.GetPoint(RectanglePoint.RightBottom, 0, -2)
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp, BlendState.AlphaBlend, null);
            title.Draw(gameTime);
            score.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            score.Update(gameTime);
        }

        #endregion

        // Value
        public int Value
        {
            get => score.Score;
            set => score.Score = value;
        }
    }
}
