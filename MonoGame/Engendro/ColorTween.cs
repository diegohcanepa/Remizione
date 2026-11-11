using Microsoft.Xna.Framework;
using System;

namespace Engendro
{
    /// <summary>
    /// ColorTween
    /// </summary>
    public sealed class ColorTween : Tween<Color>
    {
        #region Protected members

        // Lerp
        protected override Color Lerp(Color startValue, Color endValue, float progress)
        {
            return Color.Lerp(startValue, endValue, progress);
        }

        #endregion

        // Create
        public static ColorTween Create(TweenStyle style, Color startValue, Color endValue, int duration)
        {
            return Create(style, startValue, endValue, duration, 0, null);
        }

        // Create
        public static ColorTween Create(TweenStyle style, Color startValue, Color endValue, int duration, int bounceCount)
        {
            return Create(style, startValue, endValue, duration, bounceCount, null);
        }

        // Create
        public static ColorTween Create(TweenStyle style, Color startValue, Color endValue, int duration, Action? onStop)
        {
            return Create(style, startValue, endValue, duration, 0, onStop);
        }

        // Create
        public static ColorTween Create(TweenStyle style, Color startValue, Color endValue, int duration, int bounceCount, Action? onStop)
        {
            ColorTween result = new();
            result.Start(style, startValue, endValue, duration, bounceCount, onStop);
            return result;
        }
    }
}
