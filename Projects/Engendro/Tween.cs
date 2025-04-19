using Microsoft.Xna.Framework;
using System;

namespace Engendro
{
    /// <summary>
    /// Tween
    /// </summary>
    public abstract class Tween<T> where T : struct
    {
        #region Private fields

        private int bounceCooldown;
        private Action? onCompleteAction;
        private TweenStyle scaleFunctionTweenStyle;
        private int startDelayCooldown;

        #endregion

        #region Private members

        // Bounce
        private void Bounce()
        {
            if (!CanBounce)
                return;

            if (MaximumBounces != -1)
                BounceCount++;

            var newStartValue = EndValue;
            var newEndValue = StartValue;

            CurrentTime = 0;
            StartValue = newStartValue;
            EndValue = newEndValue;
            scaleFunctionTweenStyle = GetInverseScaleFunction(Style);
        }

        // CanBounce
        private bool CanBounce => MaximumBounces == -1 || BounceCount < MaximumBounces - 1;

        // ForceComplete
        private void ForceComplete()
        {
            CurrentTime = Duration;
            UpdateValue();
        }

        // GetInverseScaleFunction
        private static TweenStyle GetInverseScaleFunction(TweenStyle style)
        {
            return style switch
            {
                // QuadraticEaseIn
                TweenStyle.QuadraticIn => TweenStyle.QuadraticOut,

                // QuadraticEaseOut
                TweenStyle.QuadraticOut => TweenStyle.QuadraticIn,

                // CubicEaseIn
                TweenStyle.CubicIn => TweenStyle.CubicOut,

                // CubicEaseOut
                TweenStyle.CubicOut => TweenStyle.CubicIn,

                // QuarticEaseIn
                TweenStyle.QuarticIn => TweenStyle.QuarticOut,

                // QuarticEaseOut
                TweenStyle.QuarticOut => TweenStyle.QuarticIn,

                // QuinticEaseIn
                TweenStyle.QuinticIn => TweenStyle.QuinticOut,

                // QuinticEaseOut
                TweenStyle.QuinticOut => TweenStyle.QuinticIn,

                // Default
                _ => TweenStyle.Linear,
            };
            ;
        }

        // GetScaleFunctionValue
        private static float GetScaleFunctionValue(TweenStyle style, float progress)
        {
            return style switch
            {
                // ElasticIn
                TweenStyle.ElasticIn => EasingFunctions.ElasticIn(progress),

                // ElasticOut
                TweenStyle.ElasticOut => EasingFunctions.ElasticOut(progress),

                // ElasticInOut
                TweenStyle.ElasticInOut => EasingFunctions.ElasticInOut(progress),

                // QuadraticIn
                TweenStyle.QuadraticIn => EasingFunctions.QuadraticIn(progress),

                // QuadraticOut
                TweenStyle.QuadraticOut => EasingFunctions.QuadraticOut(progress),

                // QuadraticInOut
                TweenStyle.QuadraticInOut => EasingFunctions.QuadraticInOut(progress),

                // CubicIn
                TweenStyle.CubicIn => EasingFunctions.CubicIn(progress),

                // CubicOut
                TweenStyle.CubicOut => EasingFunctions.CubicOut(progress),

                // CubicInOut
                TweenStyle.CubicInOut => EasingFunctions.CubicInOut(progress),

                // QuarticIn
                TweenStyle.QuarticIn => EasingFunctions.QuarticIn(progress),

                // QuarticOut
                TweenStyle.QuarticOut => EasingFunctions.QuarticOut(progress),

                // QuarticInOut
                TweenStyle.QuarticInOut => EasingFunctions.QuarticInOut(progress),

                // QuinticIn
                TweenStyle.QuinticIn => EasingFunctions.QuinticIn(progress),

                // QuinticOut
                TweenStyle.QuinticOut => EasingFunctions.QuinticOut(progress),

                // QuinticInOut
                TweenStyle.QuinticInOut => EasingFunctions.QuinticInOut(progress),

                // SineIn
                TweenStyle.SineIn => EasingFunctions.SineIn(progress),

                // SineOut
                TweenStyle.SineOut => EasingFunctions.SineOut(progress),

                // SineInOut
                TweenStyle.SineInOut => EasingFunctions.SineInOut(progress),

                // Linear
                _ => EasingFunctions.Linear(progress),
            };
        }

        // UpdateValue
        private void UpdateValue()
        {
            CurrentValue = Lerp(StartValue, EndValue, GetScaleFunctionValue(scaleFunctionTweenStyle, Progress));
        }

        #endregion

        #region Protected members

        // Lerp
        protected abstract T Lerp(T startValue, T endValue, float progress);

        #endregion

        // BounceCount
        public int BounceCount { get; private set; }

        // BounceDelay
        public int BounceDelay { get; set; }

        // CurrentTime
        public float CurrentTime { get; private set; }

        // CurrentValue
        public T CurrentValue { get; private set; }

        // Duration
        public int Duration { get; private set; }

        // EndValue
        public T EndValue { get; private set; }

        // IsRunning
        public bool IsRunning => State == RunningState.Running;

        // MaximumBounces
        public int MaximumBounces { get; private set; }

        // Pause
        public void Pause()
        {
            if (State == RunningState.Running)
                State = RunningState.Paused;
        }

        // Progress
        public float Progress => CurrentTime / Duration;

        // RandomizeTime
        public void RandomizeTime() => CurrentTime = Randomizer.Next(0, Duration);

        // Restart
        public void Restart() => Start(Style, StartValue, EndValue, Duration, MaximumBounces);

        // Resume
        public void Resume()
        {
            if (State == RunningState.Paused)
                State = RunningState.Running;
        }

        // Start
        public void Start(TweenStyle style, T startValue, T endValue, int duration)
        {
            Start(style, startValue, endValue, duration, 0, null);
        }

        // Start
        public void Start(TweenStyle style, T startValue, T endValue, int duration, int bounces)
        {
            Start(style, startValue, endValue, duration, bounces, null);
        }

        // Start
        public void Start(TweenStyle style, T startValue, T endValue, int duration, Action? onStop)
        {
            Start(style, startValue, endValue, duration, 0, onStop);
        }

        // Start
        public void Start(TweenStyle style, T startValue, T endValue, int duration, int bounces, Action? onComplete)
        {
            if (IsRunning)
                Stop();

            this.Style = style;
            this.scaleFunctionTweenStyle = style;
            this.CurrentTime = 0;
            this.StartValue = startValue;
            this.EndValue = endValue;
            this.Duration = duration;
            this.MaximumBounces = bounces;
            this.onCompleteAction = onComplete;

            BounceCount = 0;
            startDelayCooldown = StartDelay;

            if (duration <= 0)
            {
                CurrentValue = startValue;
            }
            else
            {
                State = RunningState.Running;
                UpdateValue();
            }
        }

        // StartDelay
        public int StartDelay { get; set; }

        // StartValue
        public T StartValue { get; private set; }

        // State
        public RunningState State { get; private set; }

        // Stop
        public void Stop()
        {
            Stop(StopBehavior.AsIs);
        }

        // Stop
        public void Stop(StopBehavior stopBehavior)
        {
            State = RunningState.Stopped;

            if (stopBehavior == StopBehavior.ForceComplete)
                ForceComplete();

            bounceCooldown = 0;
            startDelayCooldown = 0;

            if (stopBehavior == StopBehavior.ForceComplete)
                onCompleteAction?.Invoke();
        }

        // Style
        public TweenStyle Style { get; private set; }

        // Update
        public void Update(GameTime gameTime)
        {
            Update(gameTime, 1);
        }

        // Update
        public void Update(GameTime gameTime, float timeScale)
        {
            if (!IsRunning)
                return;

            // Milliseconds based on time scale
            var ms = timeScale == 1 ? gameTime.ElapsedGameTime.Milliseconds : (int)(gameTime.ElapsedGameTime.Milliseconds * timeScale);

            if (startDelayCooldown > 0)
            {
                startDelayCooldown -= ms;
                if (startDelayCooldown > 0)
                    return;
            }

            if (bounceCooldown > 0)
            {
                bounceCooldown -= ms;
                if (bounceCooldown > 0)
                    return;
                else
                    Bounce();
            }

            CurrentTime += ms;
            if (CurrentTime >= Duration)
            {
                ForceComplete();

                if (CanBounce)
                {
                    if (BounceDelay > 0)
                    {
                        bounceCooldown = BounceDelay;
                        return;
                    }
                    else
                    {
                        Bounce();
                    }
                }
                else
                {
                    Stop(StopBehavior.ForceComplete);
                }
            }

            UpdateValue();
        }
    }
}
