using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// UIWillMeter
    /// </summary>
    public sealed class UIWillMeter : GameObject
    {
        private readonly TextSprite labelText;
        private readonly Meter meter;
        private readonly GameSession session;

        // Constructor
        public UIWillMeter(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            // Meter
            this.meter = new Meter(Game, ColorPalette.Text.TerraDarkest, ColorPalette.Text.Orange, new(34, 5))
            {
                Position = Screen.HUDArea.GetPoint(RectanglePoint.Top, 0, 8)
            };

            // Label
            this.labelText = new(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Bottom,
                Position = meter.BoundingBox.GetPoint(RectanglePoint.Top, 0, 2),
                Scale = ScaleInfo.Text.ExtraLarge,
                Text = TextRepository.GetValue("Misc.Will")
            };

            meter.MaximumValue = 100;
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            meter.Draw(gameTime);
            labelText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            meter.Value = (int)session.Will;
        }

        #endregion
    }
}
