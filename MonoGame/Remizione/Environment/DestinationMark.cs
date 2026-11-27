using Adberration;
using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// DestinationMark
    /// </summary>
    public sealed class DestinationMark : SessionGameObject<GameSession>
    {
        private Vector2? position;
        private readonly Sprite sprite;
        private readonly Vector2Tween scaleTween = new();

        // Constructor
        public DestinationMark(GameSession session)
            : base(session)
        {
            // DestinationMark
            this.sprite = new Sprite(Atlases.Environment.DestinationMark)
            {
                Color = Colors.DestinationMark,
                Opacity = .5f,
                PivotOrigin = RectanglePoint.Center,
                Scale = new(.7f)
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (position.HasValue || scaleTween.IsRunning)
                sprite.Draw(gameTime);
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
                        scaleTween.Start(TweenStyle.CubicIn, Vector2.Zero, new(.6f), 150);
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
