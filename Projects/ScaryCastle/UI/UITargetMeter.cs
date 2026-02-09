using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// UITargetMeter
    /// </summary>
    public sealed class UITargetMeter : GameObject
    {
        private Vector2 lastKnownTargetPosition;
        private readonly Meter meter;
        private readonly GameThing target;

        // Constructor
        public UITargetMeter(GameThing target)
            : base(target.Game)
        {
            this.target = target;

            // Meter
            this.meter = new Meter(Game, Color.Black, new Color(199, 47, 47), new(10, 2))
            {
                MaximumValue = target.MaxHP,
                Value = target.HP,
            };

            Refresh();
        }

        #region Private members

        // Refresh
        private void Refresh()
        {
            meter.Value = target.HP;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (target.IsDead)
                return;
            
            meter.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (target.IsDead)
                return;

            if (target.Position != lastKnownTargetPosition)
            {
                meter.Position = target.RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.Top, 0, -5);
                lastKnownTargetPosition = target.Position;
            }

            if (meter.Value != target.HP)
                Refresh();

            meter.Update(gameTime);
        }

        #endregion
    }
}
