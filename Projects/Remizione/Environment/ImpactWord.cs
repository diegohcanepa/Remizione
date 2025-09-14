using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ImpacWord
    /// </summary>
    public sealed class ImpactWord : GameObject
    {
        private int endingPhaseCooldown;
        private readonly FloatTween opacityTween = new();
        private readonly Vector2 maxScale = new(.75f);
        private readonly Vector2Tween scaleTween = new();
        private readonly ImageSprite sprite;
        private readonly FloatTween xTween = new();
        private readonly FloatTween yTween = new();

        // Constructor
        public ImpactWord(EngendroGame game)
            : base(game)
        {
            this.sprite = new ImageSprite(game, null)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Scale = maxScale
            };
        }

        #region Protected members

        // IsActiveInGameLoop
        public override bool IsActiveInGameLoop => IsActive && base.IsActiveInGameLoop;

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            sprite.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            sprite.Update(gameTime);

            if (endingPhaseCooldown > 0)
            {
                endingPhaseCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                if (endingPhaseCooldown <= 0)
                {
                    opacityTween.Start(TweenStyle.CubicOut, 1, 0, 600);
                    xTween.Start(TweenStyle.Linear, sprite.X, sprite.X + 1, 30, -1);
                    yTween.Start(TweenStyle.Linear, sprite.Y, sprite.Y + 1, 300);

                    sprite.Tweens.OpacityTween = opacityTween;
                    sprite.Tweens.XTween = xTween;
                    sprite.Tweens.YTween = yTween;
                }
            }
            else if (!opacityTween.IsRunning)
                IsActive = false;
        }

        #endregion

        // IsActive
        public bool IsActive { get; private set; }

        // Show
        public void Show(ImpactWordName kind, Vector2 position)
        {
            sprite.Tweens.Reset();
            endingPhaseCooldown = 200;
            sprite.Image = Atlases.Environment.GetImage(kind.ToString());
            sprite.Opacity = 1;
            sprite.Position = position;
            sprite.Rotation = Randomizer.Next(-.5f, .5f);
            scaleTween.Start(TweenStyle.Linear, maxScale / 2, maxScale, 100);
            sprite.Tweens.ScaleTween = scaleTween;

            IsActive = true;
        }
    }
}
