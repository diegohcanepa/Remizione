using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Engendro.Audio
{
    /// <summary>
    /// SoundInstance
    /// </summary>
    public sealed partial class SoundInstance : IDisposable
    {
        #region Private fields

        private int delayPlayCooldown;
        private int delayPlayFadeIn;
        private ISoundEmitter? emitter;
        private readonly SoundEffectInstance instance;
        private readonly FloatTween panTween = new();
        private int pauseCount;
        private static readonly List<SoundInstance> runningInstances = [];
        private readonly FloatTween stopTween = new();

        #endregion

        #region Constructor

        // Constructor
        internal SoundInstance(Sound sound, SoundEffect soundEffect)
        {
            this.Sound = sound;
            this.instance = soundEffect.CreateInstance();
            this.Duration = (int)soundEffect.Duration.TotalMilliseconds;
            this.Volume = new Volume(Sound.Name);

            ResetToDefault();
        }

        #endregion

        #region Private members

        // InvalidateVolume
        private void InvalidateVolume()
        {
            if (Volume.FadeState != FadeState.None || Emitter == null)
            {
                var newVolume = Sound.Volume * Volume.Effective * Sound.Category.Volume.Effective;
                if (newVolume != instance.Volume)
                    instance.Volume = newVolume;
            }
            else if (Emitter != null)
            {
                if (Emitter.IsAvailable)
                {
                    var baseVolume = Sound.Volume * Volume.Master * Sound.Category.Volume.Master;
                    Volume.Reset();
                    Emitter.Update(this, baseVolume);
                    instance.Volume = Volume.Current;
                }
                else
                    Stop();
            }

            var scene = EngendroGame.Instance?.SceneManager.CurrentScene;
            if (scene == null || scene.SceneController.TransitionAware)
            {
                if (TransitionAware)
                    instance.Volume *= 1 - TransitionManager.CurrentTransition.VisibleRatio;
            }
        }

        // StopCore
        private void StopCore()
        {
            if (!IsDisposed)
            {
                panTween.Stop();
                stopTween.Stop();
                instance.Stop();
                emitter = null;
                RemainingTime = 0;
                Scene = null;
                runningInstances.Remove(this);
            }
        }

        // UpdateCore
        private void UpdateCore(GameTime gameTime)
        {
            if (State == SoundState.Paused)
                return;

            if (delayPlayCooldown > 0)
            {
                delayPlayCooldown -= gameTime.ElapsedGameTime.Milliseconds;

                if (delayPlayCooldown <= 0)
                {
                    delayPlayCooldown = 0;
                    RemainingTime = 0;
                    Play(delayPlayFadeIn);
                }

                return;
            }

            if (RemainingTime > 0)
            {
                RemainingTime -= gameTime.ElapsedGameTime.Milliseconds;
                if (RemainingTime < 0)
                    RemainingTime = 0;
            }

            if (panTween.IsRunning)
            {
                panTween.Update(gameTime);
                Pan = panTween.CurrentValue;
            }

            if (stopTween.IsRunning)
            {
                stopTween.Update(gameTime);
                instance.Volume = stopTween.CurrentValue;
                if (instance.Volume == 0)
                    StopCore();

                return;
            }

            if (Volume.FadeState != FadeState.None)
            {
                var wasFadingOut = Volume.FadeState == FadeState.Out;
                Volume.Update(gameTime);
                InvalidateVolume();
                if (Volume.FadeState == FadeState.None && Volume.Current == 0 && IsPlaying && wasFadingOut)
                    StopCore();
            }
            else
                InvalidateVolume();
        }

        #endregion

        #region Internal members

        // Update
        internal static void Update(GameTime gameTime)
        {
            // Running instances
            for (var i = runningInstances.Count - 1; i >= 0; i--)
            {
                if (runningInstances[i].RemainingTime == 0)
                    runningInstances[i].StopCore();
                else
                    runningInstances[i].UpdateCore(gameTime);
            }
        }

        #endregion

        // AllowReuse
        public bool AllowReuse { get; set; } = true;

        // Dispose
        public void Dispose()
        {
            if (IsDisposed)
                return;

            StopCore();
            instance.Dispose();
            IsDisposed = true;
        }

        // Duration
        public int Duration { get; }

        // Emitter
        public ISoundEmitter? Emitter
        {
            get => emitter;
            set
            {
                if (value != emitter)
                {
                    emitter = value;
                    InvalidateVolume();
                }
            }
        }

        // GetRunningInstance
        public static SoundInstance? GetRunningInstance(string name)
        {
            for (var i = 0; i < runningInstances.Count; i++)
            {
                if (runningInstances[i].Sound.Name == name)
                    return runningInstances[i];
            }

            return null;
        }

        // GetRunningInstances
        public static IEnumerable<SoundInstance> GetRunningInstances(string name)
        {
            for (var i = 0; i < runningInstances.Count; i++)
            {
                if (runningInstances[i].Sound.Name == name)
                    yield return runningInstances[i];
            }
        }

        // IsDisposed
        public bool IsDisposed { get; private set; }

        // IsLooped
        public bool IsLooped
        {
            get => instance.IsLooped;
            set => instance.IsLooped = value;
        }

        // IsPlaying
        public bool IsPlaying => RemainingTime != 0;

        // Pan
        public float Pan
        {
            get => instance.Pan;
            set => instance.Pan = value;
        }

        // PanTo
        public void PanTo(float finalPan, int duration, TweenStyle tweenStyle)
        {
            if (duration > 0)
                panTween.Start(tweenStyle, Pan, finalPan, duration);
        }

        // Pause
        public void Pause()
        {
            if (RemainingTime == 0)
                return;

            if (IsDisposed)
                return;

            pauseCount++;
            if (pauseCount == 1)
                instance.Pause();
        }

        // PauseAware
        public bool PauseAware { get; set; } = true;

        // Pitch
        public float Pitch
        {
            get => instance.Pitch;
            set => instance.Pitch = value;
        }

        // Play
        public void Play() => Play(0);

        // Play
        public void Play(int fadeIn)
        {
            CodeContract.NotDisposed(nameof(SoundInstance), IsDisposed);

            if (State == SoundState.Stopped)
            {
                this.Scene = EngendroGame.Instance?.SceneManager.CurrentScene;

                if (fadeIn > 0)
                    Volume.FadeIn(fadeIn);

                InvalidateVolume();

                instance.Play();

                RemainingTime = IsLooped ? -1 : Duration;

                if (!runningInstances.Contains(this))
                    runningInstances.Add(this);
            }
        }

        // PlayDelayed
        public void PlayDelayed(int delay) => PlayDelayed(delay, 0);

        // PlayDelayed
        public void PlayDelayed(int delay, int fadeIn)
        {
            if (State == SoundState.Stopped)
            {
                this.Scene = EngendroGame.Instance?.SceneManager.CurrentScene;
                this.delayPlayCooldown = delay;
                this.delayPlayFadeIn = fadeIn;
                this.RemainingTime = IsLooped ? -1 : Duration;
                if (!runningInstances.Contains(this))
                    runningInstances.Add(this);
            }
        }

        // RemainingTime
        public int RemainingTime { get; private set; }

        // ResetToDefault
        public void ResetToDefault()
        {
            AllowReuse = true;
            delayPlayCooldown = 0;
            delayPlayFadeIn = 0;
            panTween.Stop();
            stopTween.Stop();
            RemainingTime = 0;
            Emitter = null;
            Pan = Sound.Pan;
            PauseAware = Sound.PauseAware;
            Pitch = Sound.Pitch;
            Scene = null;
            Volume.Master = 1;
            Volume.Reset();
            TransitionAware = Sound.TransitionAware;
            pauseCount = 0;
        }

        // Resume
        public void Resume()
        {
            if (RemainingTime == 0)
                return;

            if (IsDisposed)
                return;

            if (pauseCount > 0)
            {
                pauseCount--;
                if (pauseCount == 0)
                    instance.Resume();
            }
        }

        // RunningInstances
        public static ReadOnlyCollection<SoundInstance> RunningInstances { get; } = new ReadOnlyCollection<SoundInstance>(runningInstances);

        // Scene
        public Scene? Scene { get; private set; }

        // Sound
        public Sound Sound { get; }

        // State
        public SoundState State
        {
            get
            {
                if (pauseCount > 0)
                    return SoundState.Paused;

                else if (RemainingTime != 0 || delayPlayCooldown > 0)
                    return SoundState.Playing;

                else
                    return SoundState.Stopped;
            }
        }

        // Stop
        public void Stop() => Stop(0);

        // Stop
        public void Stop(int fadeOut)
        {
            if (fadeOut <= 0)
                StopCore();

            else if (!stopTween.IsRunning || stopTween.EndValue > stopTween.StartValue)
            {
                RemainingTime = fadeOut;
                stopTween.Start(TweenStyle.Linear, instance.Volume, 0, fadeOut);
            }
        }

        // ToString
        public override string ToString() => Sound.ToString();

        // TransitionAware
        public bool TransitionAware { get; set; } = true;

        // Volume
        public Volume Volume { get; }
    }
}