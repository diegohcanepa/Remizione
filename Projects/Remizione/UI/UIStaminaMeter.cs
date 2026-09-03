using Adberration;
using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// UIStaminaMeter
    /// </summary>
    public sealed class UIStaminaMeter : SessionGameObject<GameSession>
    {
        private readonly Sprite icon;
        private readonly Meter meter;

        // Constructor
        public UIStaminaMeter(GameSession session)
            : base(session)
        {
            this.icon = new(Atlases.UI.StaminaIcon)
            {
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, new(3, -1))
            };

            this.meter = new(icon.BoundingBox.GetPoint(RectanglePoint.RightTop, 1, 1), MeterColor.Orange);
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (Session.Player == null)
                return;

            Game.SpriteBatch.Begin(Game.Camera);
            icon.Draw(gameTime);
            meter.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Session.Player == null)
                return;

            meter.MaximumValue = Session.Player.MaxStamina;
            meter.Value = Session.Player.Stamina;
        }

        #endregion

        // BoundingBox
        public RectangleF BoundingBox => RectangleF.Union(icon.BoundingBox, meter.BoundingBox);
    }
}