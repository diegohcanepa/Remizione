using Engendro;
using Microsoft.Xna.Framework;
using System.Globalization;

namespace ScaryCastle
{
    /// <summary>
    /// UIBossMeter
    /// </summary>
    public sealed class UIBossMeter : GameObject
    {
        #region Private fields

        private readonly Sprite amountContainer;
        private readonly TextSprite amountText;
        private readonly Sprite icon;
        private readonly FlatMeter meter;

        #endregion

        #region Constructor

        // Constructor
        public UIBossMeter()
            : base()
        {
            this.meter = new FlatMeter(ColorPalette.HPMeter.Back, ColorPalette.HPMeter.Fore, ColorPalette.HPMeter.Diff, new(40, 6), 1)
            {
                Position = Screen.HUDArea.GetPoint(RectanglePoint.Bottom, 0, -10)
            };

            this.amountContainer = new(Atlases.UI.BossMeterAmount)
            {
                PivotOrigin = RectanglePoint.Left,
                Position = meter.BoundingBox.GetPoint(RectanglePoint.Right, -1, 0)
            };

            this.amountText = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Center,
                Position = amountContainer.BoundingBox.GetPoint(RectanglePoint.Center),
                Scale = ScaleInfo.Text.ExtraLarge,
                ShadowColor = ColorPalette.Shadow,
                ShadowOffset = new(0, 1)
            };

            this.icon = new(Atlases.UI.BossMeter)
            {
                PivotOrigin = RectanglePoint.Right,
                Position = meter.BoundingBox.GetPoint(RectanglePoint.Left, 1, 0)
            };
        }

        #endregion

        #region Private members

        // CanDisplay
        private bool CanDisplay()
        {
            return Target != null && Target.MaxHP != 0 && Target.HP != 0;
        }

        // Refresh
        private void Refresh()
        {
            if (!CanDisplay())
            {
                amountText.Clear();
                meter.MaximumValue = 0;
                meter.Value = 0;
                return;
            }

            if (Target != null)
            {
                meter.MaximumValue = Target.MaxHP;
                meter.Value = Target.HP;
                amountText.Text = Target.HP.ToString(CultureInfo.InvariantCulture);
            }
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
            icon.Draw(gameTime);
            amountContainer.Draw(gameTime);
            amountText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (!CanDisplay() || Target == null)
                return;

            if (Target.HP != meter.Value || Target.MaxHP != meter.MaximumValue)
                Refresh();

            meter.Update(gameTime);
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
                    Refresh();
                }
            }
        }
    }
}
