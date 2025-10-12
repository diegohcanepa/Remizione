using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione.UI
{
    /// <summary>
    /// UITicketMeter
    /// </summary>
    public class UITicketMeter : GameObject
    {
        private readonly ImageSprite icon;
        private readonly UIScore score;
        private readonly TextSprite title;

        // Constructor
        public UITicketMeter(EngendroGame game)
            : base(game)
        {
            // Icon
            this.icon = new ImageSprite(Game, Atlases.UI.GetImageNotNull("TicketIcon"))
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightTop),
                Scale = ScaleInfo.UIElement.Small
            };

            // Title
            this.title = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Green,
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightTop),
                Scale = ScaleInfo.Text.Large,
                Text = "Tickets"
            };

            // Score
            this.score = new UIScore(game, ColorPalette.Text.Terra)
            {
                PivotOrigin = RectanglePoint.Right,
                Position = icon.BoundingBox.GetPoint(RectanglePoint.Left, -1, .5f)
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (score.Value == 0)
                return;

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp, BlendState.AlphaBlend, null);
            //title.Draw(gameTime);
            icon.Draw(gameTime);
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
            get => score.Value;
            set => score.Value = value;
        }
    }
}
