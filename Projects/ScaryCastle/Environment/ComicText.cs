using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// ComicText
    /// </summary>
    public sealed class ComicText : GameObject
    {
        #region private fields

        private int endingPhaseCooldown;
        private readonly Vector2 maxScale = new(.75f);
        private readonly FloatTween opacityTween = new();
        private readonly Vector2Tween scaleTween = new();
        private readonly Sprite sprite;
        private readonly FloatTween xTween = new();
        private readonly FloatTween yTween = new();

        #endregion

        #region Constructor

        // Constructor
        public ComicText()
        {
            this.sprite = new Sprite(null)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Scale = maxScale
            };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!IsActive)
                return;

            sprite.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (!IsActive)
                return;

            sprite.Update(gameTime);

            if (endingPhaseCooldown > 0)
            {
                endingPhaseCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                if (endingPhaseCooldown <= 0)
                {
                    opacityTween.Start(TweenStyle.CubicOut, 1, 0, 1000);
                    xTween.Start(TweenStyle.Linear, sprite.X, sprite.X + 1, 30, -1);
                    yTween.Start(TweenStyle.Linear, sprite.Y, sprite.Y + 1, 300);

                    sprite.Tweens.OpacityTween = opacityTween;
                    sprite.Tweens.XTween = xTween;
                    sprite.Tweens.YTween = yTween;
                }
            }
            else if (!opacityTween.IsRunning)
            {
                IsActive = false;
            }
        }

        #endregion

        // Kind
        public ComicTextKind Kind { get; private set; }

        // IsActive
        public bool IsActive { get; private set; }

        // IsVanishing
        public bool IsVanishing => opacityTween.IsRunning;

        // Show
        public void Show(ComicTextKind kind, Vector2 position)
        {
            const string prefix = nameof(ComicText);

            this.Kind = kind;

            sprite.Tweens.Reset();
            endingPhaseCooldown = 200;
            sprite.RenderImage = Atlases.Environment.FindImage($"{prefix}_{kind}");
            sprite.Opacity = 1;
            sprite.Position = position;
            sprite.Rotation = RandomHelper.Next(Random.Shared, -.5f, .5f);
            scaleTween.Start(TweenStyle.Linear, maxScale / 2, maxScale, 100);
            sprite.Tweens.ScaleTween = scaleTween;

            IsActive = true;
        }
    }
}
