using Microsoft.Xna.Framework;
using System;

namespace Engendro
{
    /// <summary>
    /// Transition
    /// </summary>
    public class Transition : IUpdate, IDraw
    {
        #region Private fields

        private Action? onComplete;
        private bool onCompleteDone;
        private readonly FloatTween tween = new();

        #endregion

        #region Private members

        // Start
        private void Start(TransitionState mode, int duration, Action? onComplete)
        {
            if (duration <= 0)
            {
                VisibleRatio = mode == TransitionState.In ? 1 : 0;
                RemainingMilliseconds = 0;
                onComplete?.Invoke();
                return;
            }

            this.TransitionState = mode;

            var endValue = mode == TransitionState.In ? 1 : 0;

            tween.Start(TweenStyle, VisibleRatio, endValue, duration);
            this.onComplete = onComplete;

            OnStart(duration);

            RemainingMilliseconds = duration;
            onCompleteDone = false;
        }

        #endregion

        #region Protected members

        // OnComplete
        protected virtual void OnComplete()
        {
        }

        // OnDraw
        protected virtual void OnDraw(GameTime gameTime)
        {
        }

        // OnStart
        protected virtual void OnStart(int duration)
        {
        }

        // OnUpdate
        protected virtual void OnUpdate(GameTime gameTime)
        {
        }

        #endregion

        // DefaultDuration
        public int DefaultDuration { get; set; } = 1000;

        // Draw
        public void Draw(GameTime gameTime)
        {
            OnDraw(gameTime);
        }

        // In
        public void In()
        {
            In(DefaultDuration, null);
        }

        // In
        public void In(int duration)
        {
            In(duration, null);
        }

        // In
        public void In(int duration, Action? onComplete)
        {
            Start(TransitionState.In, duration, onComplete);
        }

        // IsRunning
        public bool IsRunning => tween != null && tween.IsRunning;

        // Out
        public void Out()
        {
            Out(DefaultDuration, null);
        }

        // Out
        public void Out(int duration)
        {
            Out(duration, null);
        }

        // Out
        public void Out(int duration, Action? onComplete)
        {
            Start(TransitionState.Out, duration, onComplete);
        }

        // RemainingMilliseconds
        public int RemainingMilliseconds { get; private set; }

        // TransitionState
        public TransitionState TransitionState { get; private set; }

        // TweenStyle
        public TweenStyle TweenStyle { get; set; }

        // Update
        public void Update(GameTime gameTime)
        {
            if (IsRunning)
            {
                tween.Update(gameTime);
                VisibleRatio = tween.CurrentValue;
                RemainingMilliseconds -= gameTime.ElapsedGameTime.Milliseconds;
                if (RemainingMilliseconds < 0)
                {
                    RemainingMilliseconds = 0;
                }

                OnUpdate(gameTime);
            }
            else
            {
                if (!onCompleteDone)
                {
                    onCompleteDone = true;
                    OnComplete();
                    onComplete?.Invoke();
                    onComplete = null;
                }
            }
        }

        // VisibleRatio
        public float VisibleRatio { get; private set; }
    }
}
