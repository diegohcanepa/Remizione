using Microsoft.Xna.Framework;
using System;

namespace Engendro
{
    /// <summary>
    /// EasingFunctions
    /// </summary>
    public static class EasingFunctions
    {
        #region Private members

        // EaseInPower
        private static float EaseInPower(float progress, int power)
        {
            return (float)Math.Pow(progress, power);
        }

        // EaseOutPower
        private static float EaseOutPower(float progress, int power)
        {
            var sign = power % 2 == 0 ? -1 : 1;
            return (float)(sign * (Math.Pow(progress - 1, power) + sign));
        }

        // EaseInOutPower
        private static float EaseInOutPower(float progress, int power)
        {
            progress *= 2;
            if (progress < 1)
            {
                return (float)Math.Pow(progress, power) / 2f;
            }
            else
            {
                var sign = power % 2 == 0 ? -1 : 1;
                return (float)(sign / 2.0 * (Math.Pow(progress - 2, power) + sign * 2));
            }
        }

        #endregion

        // CubicIn
        public static float CubicIn(float progress)
        {
            return EaseInPower(progress, 3);
        }

        // CubicOut
        public static float CubicOut(float progress)
        {
            return EaseOutPower(progress, 3);
        }

        // CubicInOut
        public static float CubicInOut(float progress)
        {
            return EaseInOutPower(progress, 3);
        }

        // ElasticIn
        public static float ElasticIn(float progress)
        {
            var ts = progress * progress;
            var tc = ts * progress;

            return (33 * tc * ts + -59 * ts * ts + 32 * tc + -5 * ts);
        }

        // ElasticOut
        public static float ElasticOut(float progress)
        {
            var ts = progress * progress;
            var tc = ts * progress;

            return (33 * tc * ts + -106 * ts * ts + 126 * tc + -67 * ts + 15 * progress);
        }

        // ElasticInOut
        public static float ElasticInOut(float t)
        {
            return (t <= 0.5f) ? ElasticIn(t * 2) / 2 : ElasticOut(t * 2 - 1) / 2 + 0.5f;
        }

        // Linear
        public static float Linear(float progress)
        {
            return progress;
        }

        // QuadraticIn
        public static float QuadraticIn(float progress)
        {
            return EaseInPower(progress, 2);
        }

        // QuadraticOut
        public static float QuadraticOut(float progress)
        {
            return EaseOutPower(progress, 2);
        }

        // QuadraticInOut
        public static float QuadraticInOut(float progress)
        {
            return EaseInOutPower(progress, 2);
        }

        // QuarticIn
        public static float QuarticIn(float progress)
        {
            return EaseInPower(progress, 4);
        }

        // QuarticOut
        public static float QuarticOut(float progress)
        {
            return EaseOutPower(progress, 4);
        }

        // QuarticInOut
        public static float QuarticInOut(float progress)
        {
            return EaseInOutPower(progress, 4);
        }

        // QuinticIn
        public static float QuinticIn(float progress)
        {
            return EaseInPower(progress, 5);
        }

        // QuinticOut
        public static float QuinticOut(float progress)
        {
            return EaseOutPower(progress, 5);
        }

        // QuinticInOut
        public static float QuinticInOut(float progress)
        {
            return EaseInOutPower(progress, 5);
        }

        // SineIn
        public static float SineIn(float progress)
        {
            return -(float)Math.Cos(MathHelper.PiOver2 * progress) + 1;
        }

        // SineOut
        public static float SineOut(float progress)
        {
            return (float)Math.Sin(MathHelper.PiOver2 * progress);
        }

        // SineInOut
        public static float SineInOut(float progress)
        {
            return -(float)Math.Cos(MathHelper.Pi * progress) / 2f + .5f;
        }
    }
}
