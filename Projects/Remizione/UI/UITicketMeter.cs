using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione
{
    /// <summary>
    /// UITicketMeter
    /// </summary>
    public class UITicketMeter : GameObject
    {
        private readonly ImageSprite icon;
        private readonly UIScore score;

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

            // Score
            this.score = new UIScore(game, ColorPalette.Text.Terra, false)
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

        // SetInitialValue
        public void SetInitialValue(int value)
        {
            score.SetInitialValue(value);
        }

        // Value
        public int Value
        {
            get => score.Value;
            set => score.Value = value;
        }
    }
}
