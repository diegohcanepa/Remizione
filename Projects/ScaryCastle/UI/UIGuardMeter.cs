using Engendro;
using Microsoft.Xna.Framework;
using System.Globalization;

namespace ScaryCastle
{
    /// <summary>
    /// UIGuardMeter
    /// </summary>
    public sealed class UIGuardMeter : GameObject
    {
        #region Private fields

        private readonly TextSprite amountText;
        private readonly Sprite icon;
        private readonly TextSprite labelText;
        private readonly Meter meter;

        #endregion

        #region Constructor

        // Constructor
        public UIGuardMeter()
            : base()
        {
            this.meter = new Meter(ColorPalette.GuardMeter.Back, ColorPalette.GuardMeter.Fore, ColorPalette.GuardMeter.Diff, new(40, 6), 1)
            {
                Position = Screen.HUDArea.GetPoint(RectanglePoint.Bottom, 0, -10)
            };

            this.labelText = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Bottom,
                Position = meter.BoundingBox.GetPoint(RectanglePoint.Top, 0, .25f),
                Scale = ScaleInfo.Text.ExtraLarge
            };

            this.amountText = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Left,
                Position = meter.BoundingBox.GetPoint(RectanglePoint.Right, 1.5f, .5f),
                Scale = ScaleInfo.Text.ExtraLarge
            };

            this.icon = new(Atlases.UI.SkullIcon)
            {
                PivotOrigin = RectanglePoint.Right,
                Position = meter.BoundingBox.GetPoint(RectanglePoint.Left, -1, .2f)
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
            icon.Draw(gameTime);
            meter.Draw(gameTime);
            labelText.Draw(gameTime);
            amountText.Draw(gameTime);
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
                    {
                        meter.Value = Target.HP;
                        amountText.Text = Target.HP.ToString(CultureInfo.InvariantCulture);
                    }

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
                        amountText.Text = field.HP.ToString(CultureInfo.InvariantCulture);
                        labelText.Text = field.DisplayName;
                    }
                }
            }
        }
    }
}
