using Adberration;
using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// HUDStaminaMeter
    /// </summary>
    public sealed class HUDStaminaMeter : SessionGameObject<GameSession>
    {
        private readonly Sprite icon;
        private readonly Meter meter;

        // Constructor
        public HUDStaminaMeter(GameSession session)
            : base(session)
        {
            this.icon = new(Atlases.UI.StaminaIcon)
            {
                Position = new(16, 12)
            };

            this.meter = new(icon.BoundingBox.GetPoint(RectanglePoint.RightTop, 0, 1), MeterColor.Orange);
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
    }
}