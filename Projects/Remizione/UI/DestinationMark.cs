using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione.UI
{
    /// <summary>
    /// DestinationMark
    /// </summary>
    public sealed class DestinationMark : GameObject
    {
        private Vector2? position;
        private readonly GameSession session;
        private readonly ImageSprite sprite;
        private readonly Vector2Tween scaleTween = new();

        // Constructor
        public DestinationMark(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            // DestinationMark
            this.sprite = new ImageSprite(Game, Atlases.Environment.MoveDestinationMark)
            {
                Color = ColorPalette.DestinationMark,
                Opacity = .3f,
                PivotOrigin = RectanglePoint.Middle,
                Scale = new(.7f)
            };

            sprite.Tweens.OpacityTween = FloatTween.Create(TweenStyle.CubicInOut, .3f, .5f, 500, -1);
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (position.HasValue || scaleTween.IsRunning)
            {
                Game.SpriteBatch.Begin(session.Camera);
                sprite.Draw(gameTime);
                Game.SpriteBatch.End();
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            sprite.Update(gameTime);
        }

        #endregion

        // Position
        public Vector2? Position
        {
            get => position;
            set
            {
                if (value != position)
                {
                    this.position = value;

                    if (position.HasValue)
                    {
                        sprite.Position = position.Value;
                        scaleTween.Start(TweenStyle.CubicIn, Vector2.Zero, new(.8f), 150);
                    }
                    else
                    {
                        scaleTween.Start(TweenStyle.CubicIn, sprite.Scale, Vector2.Zero, 300);
                    }

                    sprite.Tweens.ScaleTween = scaleTween;
                }
            }
        }
    }
}
