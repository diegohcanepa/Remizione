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
        private readonly TextSprite amountText;
        private readonly Sprite icon = new(Atlases.UI.HeartIcon) { PivotOrigin = RectanglePoint.Center, Position = new(8, 6) };
        private int lastHp = int.MinValue;
        private int lastMaxHp = int.MinValue;
        private readonly FloatTween rotationTween = new();
        private readonly Vector2Tween scaleTween = new();

        #endregion

        #region Constructor

        // Constructor
        public UIHeartMeter(GameSession session)
            : base(session)
        {
            this.amountText = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Terra,
                PivotOrigin = RectanglePoint.Left,
                Position = icon.BoundingBox.GetPoint(RectanglePoint.Right, 1, 0),
                Scale = ScaleInfo.Text.Large
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

            amountText.Text = $"{actor.HP}/{actor.MaxHP}";
            lastHp = actor.HP;
            lastMaxHp = actor.MaxHP;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (actor == null)
                return;

            icon.Draw(gameTime);
            amountText.Draw(gameTime);
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
        }

        #endregion
    }
}