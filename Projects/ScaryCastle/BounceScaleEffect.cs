using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// BounceScaleEffect
    /// </summary>
    public sealed class BounceScaleEffect
    {
        private float time;
        private float duration;
        private float finalScale;

        // EaseOutBounce
        private static float EaseOutBounce(float t)
        {
            if (t < 1f / 2.75f)
                return 7.5625f * t * t;
            if (t < 2f / 2.75f)
                return (7.5625f * (t -= 1.5f / 2.75f) * t) + 0.75f;

            if (t < 2.5f / 2.75f)
                return (7.5625f * (t -= 2.25f / 2.75f) * t) + 0.9375f;

            return (7.5625f * (t -= 2.625f / 2.75f) * t) + 0.984375f;
        }

        // IsPlaying
        public bool IsPlaying { get; private set; }

        // Play
        public void Play(float finalScale, float duration = 0.5f)
        {
            this.finalScale = finalScale;
            this.duration = duration;
            this.time = 0;
            this.IsPlaying = true;
            this.Value = 0;
        }

        // Update
        public void Update(GameTime gameTime)
        {
            if (!IsPlaying)
                return;

            time += (float)gameTime.ElapsedGameTime.TotalSeconds;

            float t = MathF.Min(time / duration, 1f);
            Value = EaseOutBounce(t) * finalScale;

            if (t >= 1f)
            {
                Value = finalScale;
                IsPlaying = false;
            }
        }

        // Value
        public float Value { get; private set; }
    }
}
