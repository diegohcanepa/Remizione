using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// DustParticle
    /// </summary>
    public sealed class DustParticle : Particle
    {
        private readonly FloatTween opacityTween = new();

        // Constructor
        public DustParticle(EngendroGame game)
            : base(game)
        {
        }

        // OnActivate
        protected override void OnActivate()
        {
            base.OnActivate();
            opacityTween.Start(TweenStyle.Linear, 0, Opacity, Lifespan / 2, 2);
            Opacity = opacityTween.CurrentValue;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            opacityTween.Update(gameTime);
            Opacity = opacityTween.CurrentValue;
            base.OnUpdate(gameTime);
        }
    }
}
