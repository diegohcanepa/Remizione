using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// UIGuardMeter
    /// </summary>
    public sealed class UIGuardMeter : GameObject
    {
        #region Private fields

        private readonly TextSprite labelText;
        private readonly Meter meter;

        #endregion

        #region Constructor

        // Constructor
        public UIGuardMeter()
            : base()
        {
            this.meter = new Meter(ColorPalette.Text.TerraDarker, ColorPalette.Text.Red, ColorPalette.Text.Orange, new(40, 6), 1)
            {
                Position = Screen.HUDArea.GetPoint(RectanglePoint.Bottom, 0, -8)
            };

            this.labelText = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Bottom,
                Position = meter.BoundingBox.GetPoint(RectanglePoint.Top),
                Scale = ScaleInfo.Text.Large
            };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (Target == null)
                return;

            Game.SpriteBatch.Begin(Game.Camera);
            meter.Draw(gameTime);
            labelText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Target != null)
            {
                if (Target.IsDead)
                {
                    Target = null;
                }
                else
                {
                    if (meter.Value != Target.HP)
                        meter.Value = Target.HP;

                    meter.Update(gameTime);
                }
            }
        }

        #endregion

        // Target
        public Actor? Target
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;

                    if (field != null && field.MaxHP == 0)
                        field = null;

                    if (field != null)
                    {
                        meter.MaximumValue = field.MaxHP;
                        labelText.Text = field.DisplayName;
                    }
                }
            }
        }
    }
}
