using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// SepiaEffect
    /// </summary>
    public sealed class SepiaEffect : GameObject
    {
        private readonly FloatTween tween = new();

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (Intensity > 0)
            {
                RemizioneGame.Effects.Sepia.Intensity = Intensity;
                Game.RenderTargets.Swap();
                Game.SpriteBatch.Begin(effect: RemizioneGame.Effects.Sepia.Effect);
                Game.SpriteBatch.Draw(Game.RenderTargets.PreviousTarget, Vector2.Zero, Color.White);
                Game.SpriteBatch.End();
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (tween.IsRunning)
            {
                tween.Update(gameTime);
                Intensity = tween.CurrentValue;
            }
        }

        #endregion

        // Intensity
        public float Intensity { get; set; }

        // IsRunning
        public bool IsRunning => tween.IsRunning;

        // Start
        public void Start(float intensity, int duration) => Start(intensity, duration, 0);

        // Start
        public void Start(float intensity, int duration, int bounces)
        {
            if (duration == 0)
            {
                tween.Stop();
                Intensity = intensity;
            }
            else
                tween.Start(TweenStyle.QuadraticIn, this.Intensity, intensity, duration, bounces);
        }

        // Stop
        public void Stop(bool immediate)
        {
            if (tween.IsRunning)
            {
                if (immediate)
                    tween.Stop();
                else
                    tween.Start(TweenStyle.QuadraticOut, Intensity, 0, 2000);
            }
        }
    }
}
