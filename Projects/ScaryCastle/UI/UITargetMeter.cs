using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// UITargetMeter
    /// </summary>
    public sealed class UITargetMeter : GameObject
    {
        private readonly Meter meter;
        private readonly GameThing target;
        private readonly TextSprite targetNameLabel;
        private readonly TextSprite targetHPLabel;

        // Constructor
        public UITargetMeter(GameThing target)
            : base(target.Game)
        {
            this.target = target;

            // Meter
            this.meter = new(Game, new(80, 20, 20), new Color(199, 47, 47), new(30, 5))
            {
                Alignment = HorizontalAlignment.Center,
                MaximumValue = target.MaxHP,
                Value = target.HP,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.Top, 0, 8)
            };

            // Label
            this.targetNameLabel = new(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Bottom,
                Position = meter.BoundingBox.GetPoint(RectanglePoint.Top),
                Scale = ScaleInfo.Text.ExtraLarge,
                Text = target.LocalizedDisplayName
            };

            // HP text
            this.targetHPLabel = new(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Top,
                Position = meter.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, 1),
                Scale = ScaleInfo.Text.Large
            };

            Refresh();
        }

        #region Private members

        // Refresh
        private void Refresh()
        {
            meter.Value = target.HP;
            targetHPLabel.Text = $"{target.HP} HP";
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (target.IsDead)
                return;
            
            meter.Draw(gameTime);
            targetNameLabel.Draw(gameTime);
            targetHPLabel.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (target.IsDead)
                return;

            if (meter.Value != target.HP)
                Refresh();               

            meter.Update(gameTime);
        }

        #endregion
    }
}
