using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// UIFearMeter
    /// </summary>
    public sealed class UIFearMeter : GameObject
    {
        private readonly Sprite icon;
        private readonly Meter meter;
        private readonly GameSession session;
        private readonly Vector2Tween tween = Vector2Tween.Create(TweenStyle.Linear, Vector2.One, Vector2.One * .95f, 200, -1);

        #region Constructor

        // Constructor
        public UIFearMeter(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            this.icon = new(Game, Atlases.UI.FearIcon)
            {
                PivotOrigin = RectanglePoint.Center,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, 6, -8)
            };

            this.meter = new Meter(Game, ColorPalette.Text.TerraDarker, ColorPalette.Text.Red, new(125, 56, 51), new(40, 5), 1)
            {
                MaximumValue = session.FearManager.MaximumValue,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, 33, -10)
            };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            icon.Draw(gameTime);
            meter.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            tween.Update(gameTime);

            if (meter.Ratio <= .3)
            {
                meter.ForeColor = ColorPalette.FearMeter.Green;
                meter.DiffColor = ColorPalette.FearMeter.GreenDiff;
                icon.Scale = Vector2.One;
            }
            else if (meter.Ratio <= .6)
            {
                meter.ForeColor = ColorPalette.FearMeter.Yellow;
                meter.DiffColor = ColorPalette.FearMeter.YellowDiff;
                icon.Scale = Vector2.One;
            }
            else if (meter.Ratio <= .8)
            {
                meter.ForeColor = ColorPalette.FearMeter.Orange;
                meter.DiffColor = ColorPalette.FearMeter.OrangeDiff;
                icon.Scale = Vector2.One;
            }
            else
            {
                meter.ForeColor = ColorPalette.FearMeter.Red;
                meter.DiffColor = ColorPalette.FearMeter.RedDiff;
                icon.Scale = tween.CurrentValue;
            }

            meter.Value = session.FearManager.CurrentValue;
            meter.Update(gameTime);
        }

        #endregion
    }
}
