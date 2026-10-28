using Adberration;
using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// UIHeartMeter
    /// </summary>
    public sealed class UIHeartMeter : SessionGameObject<GameSession>
    {
        #region Private fields

        private Actor? actor;
        private readonly UICapacity capacityMeter;
        private readonly Sprite container = new(Atlases.UI.HeartMeterContainer) { PivotOrigin = RectanglePoint.Left };
        private readonly Sprite icon = new(Atlases.UI.HeartIcon) { PivotOrigin = RectanglePoint.Center, Position = new(8, 6) };
        private int lastHp = int.MinValue;
        private int lastMaxHp = int.MinValue;
        private readonly FlatMeter meter;
        private readonly FloatTween rotationTween = new();
        private readonly Vector2Tween scaleTween = new();

        #endregion

        #region Constructor

        // Constructor
        public UIHeartMeter(GameSession session)
            : base(session)
        {
            this.meter = new(Colors.Palette.AlmostBlack, Colors.Palette.TerraDark, Colors.Palette.Terra, Colors.Palette.TerraLight, new(21, 5), 0)
            {
                Position = icon.BoundingBox.GetPoint(RectanglePoint.Right, 13, -3),
            };

            container.Position = icon.BoundingBox.GetPoint(RectanglePoint.Right, 1, -1);

            this.capacityMeter = new()
            {
                PivotOrigin = RectanglePoint.Left,
                Position = container.BoundingBox.GetPoint(RectanglePoint.Right, 1, 1),
            };
        }

        #endregion

        #region Private members

        // Animate
        private void Animate()
        {
            rotationTween.Start(TweenStyle.QuadraticInOut, 0, 15, 50, 10);
            scaleTween.Start(TweenStyle.QuadraticInOut, Vector2.One, Vector2.One * 1.3f, 150, 2);

            icon.Tweens.RotationTween = rotationTween;
            icon.Tweens.ScaleTween = scaleTween;
        }

        // Refresh
        private void Refresh()
        {
            if (actor == null)
                return;

            if (lastHp != int.MinValue)
                Animate();

            lastHp = actor.HP;
            lastMaxHp = actor.MaxHP;
            meter.MaximumValue = lastMaxHp;
            meter.Value = lastHp;

            capacityMeter.CurrentAmount = lastHp;
            capacityMeter.MaximumAmount = lastMaxHp;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (actor == null)
                return;

            meter.Draw(gameTime);
            container.Draw(gameTime);
            icon.Draw(gameTime);
            capacityMeter.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Session.Player != actor)
            {
                actor = Session.Player;
                Refresh();
            }

            if (actor == null)
                return;

            if (lastHp != actor.HP || lastMaxHp != actor.MaxHP)
                Refresh();

            icon.Update(gameTime);
            meter.Update(gameTime);
        }

        #endregion
    }
}