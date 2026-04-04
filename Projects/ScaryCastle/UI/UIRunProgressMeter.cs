using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// UIRunProgressMeter
    /// </summary>
    public sealed class UIRunProgressMeter : GameObject
    {
        #region Private fields

        private readonly TextSprite amountText;
        private readonly Sprite icon;
        private int lastKnownValue = -1;
        private readonly GameSession session;

        #endregion

        #region Constructor

        // Constructor
        public UIRunProgressMeter(GameSession session)
        {
            this.session = session;

            // Icon
            this.icon = new(Atlases.UI.TunnelIcon)
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.Area.GetPoint(RectanglePoint.RightTop, -4, 3),
            };

            // Amount
            this.amountText = new TextSprite(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Right,
                Position = icon.BoundingBox.GetPoint(RectanglePoint.Left, -1, 2),
                Scale = ScaleInfo.Text.Huge,
                Spacing = -6
            };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (session.CurrentRun == null)
                return;

            icon.Draw(gameTime);
            amountText.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (session.CurrentRun == null)
                return;

            icon.Update(gameTime);

            if (lastKnownValue != session.CurrentRun.CorridorIndex)
            {
                lastKnownValue = session.CurrentRun.CorridorIndex;
                amountText.Text = $"{session.CurrentRun.CorridorIndex + 1}/{session.CurrentRun.MaxCorridors}";
            }
        }

        #endregion
    }
}
