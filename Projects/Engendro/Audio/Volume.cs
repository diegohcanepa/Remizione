using Microsoft.Xna.Framework;

namespace Engendro.Audio
{
    /// <summary>
    /// Volume
    /// </summary>
    public sealed class Volume
    {
        #region Private fields

        private float current = 1;
        private float master = 1;
        private readonly FloatTween tween = new();

        #endregion

        #region Constructor

        // Constructor
        internal Volume(string name)
        {
            this.Name = name;
        }

        #endregion

        #region Internal members

        // Update
        internal void Update(GameTime gameTime)
        {
            if (tween.IsRunning)
            {
                tween.Update(gameTime);
                Current = tween.CurrentValue;
                if (!tween.IsRunning)
                    FadeState = FadeState.None;
            }
        }

        #endregion

        // Current
        public float Current
        {
            get => current;
            set
            {
                if (value != current)
                    current = MathHelper.Clamp(value, 0, 1);
            }
        }

        // Effective
        public float Effective => Master * current;

        // FadeIn
        public void FadeIn(int duration) => FadeIn(duration, 1);

        // FadeIn
        public void FadeIn(int duration, float finalVolume)
        {
            if (duration < 1)
            {
                Current = 1;
            }
            else
            {
                Current = 0;
                tween.Start(TweenStyle.CubicIn, 0, finalVolume, duration);
                FadeState = FadeState.In;
            }
        }

        // FadeOut
        public void FadeOut(int duration) => FadeOut(duration, 0);

        // FadeOut
        public void FadeOut(int duration, float finalVolume)
        {
            if (duration < 1)
            {
                Current = 0;
            }
            else
            {
                tween.Start(TweenStyle.CubicOut, Current, finalVolume, duration);
                FadeState = FadeState.Out;
            }
        }

        // FadeState
        public FadeState FadeState { get; private set; }

        // FadeTo
        public bool FadeTo(int duration, float finalVolume)
        {
            if (Current == finalVolume)
                return false;

            FadeState = finalVolume > Current ? FadeState.In : FadeState.Out;

            if (duration < 1)
                Current = finalVolume;
            else
                tween.Start(TweenStyle.CubicIn, Current, finalVolume, duration);

            return true;
        }

        // Master
        public float Master
        {
            get => master;
            set
            {
                if (value != master)
                    master = MathHelper.Clamp(value, 0, 1);
            }
        }

        // Name
        public string Name { get; }

        // Reset
        public void Reset()
        {
            StopFade();
            Current = 1;
        }

        // StopFade
        public void StopFade()
        {
            tween.Stop();
            FadeState = FadeState.None;
        }
    }
}
