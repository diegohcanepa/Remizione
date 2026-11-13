using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// CycleManager
    /// </summary>
    public sealed class CycleManager
    {
        private static readonly Color defaultLightMapColor = new(45, 45, 45);

        // Constructor
        public CycleManager()
        {
            IsEnabled = false;
            SetCycle(Cycle.Light, skipTransition: true);
        }

        #region Private members

        // GetCycleDuration
        private static float GetCycleDuration(Cycle cycle) =>
            cycle == Cycle.Dark
                ? GameData.WorldSettings.DarkCycleDuration
                : GameData.WorldSettings.LightCycleDuration;

        // GetElapsedTime
        private float GetElapsedTime()
        {
            float totalCycleDuration = GetCycleDuration(CurrentCycle);
            return Math.Max(0f, totalCycleDuration - CycleRemainingTime);
        }

        #endregion

        // ApplyCycle
        public void ApplyCycle()
        {
            if (!IsEnabled)
                return;

            float transitionDuration = GameData.WorldSettings.CycleTransitionDuration;
            float elapsedTime = GetElapsedTime();

            float linearProgress = transitionDuration > 0f
                ? Math.Clamp(elapsedTime / transitionDuration, 0f, 1f)
                : 1f;

            // Curva CubicIn
            float easedProgress = linearProgress * linearProgress * linearProgress;

            if (CurrentCycle == Cycle.Dark)
            {
                LightOpacity = 1f - easedProgress;
                LightMapColor = Color.Lerp(defaultLightMapColor, Color.Black, easedProgress);
            }
            else
            {
                LightOpacity = easedProgress;
                LightMapColor = Color.Lerp(Color.Black, defaultLightMapColor, easedProgress);
            }
        }

        // CurrentCycle
        public Cycle CurrentCycle { get; private set; } = Cycle.Light;

        // CycleChanged
        public event Action<Cycle>? CycleChanged;

        // CycleCount
        public int CycleCount { get; set; }

        // CycleRemainingTime
        public float CycleRemainingTime { get; private set; }

        // IsEnabled
        public bool IsEnabled
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    if (field)
                        ApplyCycle();
                }
            }
        }

        // IsTransitioning
        public bool IsTransitioning => IsEnabled && GetElapsedTime() < GameData.WorldSettings.CycleTransitionDuration;

        // LightMapColor
        public Color LightMapColor { get; private set; } = defaultLightMapColor;

        // LightOpacity
        public float LightOpacity { get; private set; } = 1;

        // SetCycle
        public void SetCycle(Cycle cycle, float? remainingTime = null, bool skipTransition = false)
        {
            CurrentCycle = cycle;

            if (remainingTime.HasValue)
            {
                CycleRemainingTime = Math.Max(0f, remainingTime.Value);
            }
            else
            {
                float duration = GetCycleDuration(cycle);
                CycleRemainingTime = skipTransition
                    ? Math.Max(0f, duration - GameData.WorldSettings.CycleTransitionDuration)
                    : duration;
            }

            ApplyCycle();
        }

        // Update
        public void Update(GameTime gameTime)
        {
            if (!IsEnabled)
                return;

            CycleRemainingTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (CycleRemainingTime <= 0)
            {
                Cycle nextCycle = CurrentCycle == Cycle.Dark ? Cycle.Light : Cycle.Dark;
                SetCycle(nextCycle);
                CycleChanged?.Invoke(nextCycle);
            }
            else
            {
                ApplyCycle();
            }
        }
    }
}