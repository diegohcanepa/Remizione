using Microsoft.Xna.Framework;
using System;

namespace Engendro
{
    /// <summary>
    /// Vector2Tween
    /// </summary>
    public sealed class Vector2Tween : Tween<Vector2>
    {
        #region Protected members

        // Lerp
        protected override Vector2 Lerp(Vector2 startValue, Vector2 endValue, float progress)
        {
            Vector2 result = Vector2.Lerp(startValue, endValue, progress);

            if (Decimals >= 0)
                result = result.Round(Decimals);

            return result;
        }

        #endregion

        // Decimals
        public int Decimals { get; set; } = -1;

        // Create
        public static Vector2Tween Create(TweenStyle style, float startValue, float endValue, int duration)
        {
            return Create(style, new Vector2(startValue), new Vector2(endValue), duration, 0, null);
        }

        // Create
        public static Vector2Tween Create(TweenStyle style, Vector2 startValue, Vector2 endValue, int duration)
        {
            return Create(style, startValue, endValue, duration, 0, null);
        }

        // Create
        public static Vector2Tween Create(TweenStyle style, float startValue, float endValue, int duration, int bounceCount)
        {
            return Create(style, new Vector2(startValue), new Vector2(endValue), duration, bounceCount, null);
        }

        // Create
        public static Vector2Tween Create(TweenStyle style, Vector2 startValue, Vector2 endValue, int duration, int bounceCount)
        {
            return Create(style, startValue, endValue, duration, bounceCount, null);
        }

        // Create
        public static Vector2Tween Create(TweenStyle style, float startValue, float endValue, int duration, Action? onStop)
        {
            return Create(style, new Vector2(startValue), new Vector2(endValue), duration, onStop);
        }

        // Create
        public static Vector2Tween Create(TweenStyle style, Vector2 startValue, Vector2 endValue, int duration, Action? onStop)
        {
            return Create(style, startValue, endValue, duration, 0, onStop);
        }

        // Create
        public static Vector2Tween Create(TweenStyle style, float startValue, float endValue, int duration, int bounceCount, Action? onStop)
        {
            return Create(style, new Vector2(startValue), new Vector2(endValue), duration, bounceCount, onStop);
        }

        // Create
        public static Vector2Tween Create(TweenStyle style, Vector2 startValue, Vector2 endValue, int duration, int bounceCount, Action? onStop)
        {
            Vector2Tween result = new();
            result.Start(style, startValue, endValue, duration, bounceCount, onStop);
            return result;
        }
    }
}
