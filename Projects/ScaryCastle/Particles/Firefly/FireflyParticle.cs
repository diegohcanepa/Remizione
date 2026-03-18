using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// Firefly
    /// </summary>
    public sealed class FireflyParticle : Particle
    {
        private readonly FloatTween opacityTween = new();

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
