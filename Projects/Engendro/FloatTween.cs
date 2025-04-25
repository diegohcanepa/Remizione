using System;

namespace Engendro
{
    /// <summary>
    /// FloatTween
    /// </summary>
    public class FloatTween : Tween<float>
    {
        #region Protected members

        // Lerp
        protected override float Lerp(float startValue, float endValue, float progress)
        {
            var result = startValue + (endValue - startValue) * progress;
            if (Decimals >= 0)
                result = result.Round(Decimals);

            return result;
        }

        #endregion

        // Create
        public static FloatTween Create(TweenStyle style, float startValue, float endValue, int duration)
        {
            return Create(style, startValue, endValue, duration, 0, null);
        }

        // Create
        public static FloatTween Create(TweenStyle style, float startValue, float endValue, int duration, int bounces)
        {
            return Create(style, startValue, endValue, duration, bounces, null);
        }

        // Create
        public static FloatTween Create(TweenStyle style, float startValue, float endValue, int duration, Action? onStop)
        {
            return Create(style, startValue, endValue, duration, 0, onStop);
        }

        // Create
        public static FloatTween Create(TweenStyle style, float startValue, float endValue, int duration, int bounces, Action? onStop)
        {
            FloatTween result = new();
            result.Start(style, startValue, endValue, duration, bounces, onStop);
            return result;
        }

        // Decimals
        public int Decimals { get; set; } = -1;
    }
}
