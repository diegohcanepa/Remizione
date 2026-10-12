using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;

namespace Engendro
{
    /// <summary>
    /// Scene
    /// </summary>
    public abstract class Scene : GameObject, IDisposable, IInputHandler
    {
        #region Private fields

        private ContentManager? content;
        private bool isDisposed;
        private int pauseCount;

        #endregion

        #region Private members

        // InvalidateBoundingBox
        private void InvalidateBoundingBox()
        {
            BoundingBox = new Rectangle(0, 0, Width, Height);
        }

        #endregion

        #region Protected members

        // Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
                return;

            if (disposing)
                UnloadContent();

            isDisposed = true;
        }

        // OnActivate
        protected virtual void OnActivate()
        {
        }

        // OnContinuousUpdate
        protected virtual void OnContinuousUpdate(GameTime gameTime)
        {
        }

        // OnDeactivate
        protected virtual void OnDeactivate()
        {
        }

        // OnHandleInput
        protected virtual HandleInputResult OnHandleInput(GameTime gameTime)
        {
            return HandleInputResult.Unhandled;
        }

        // OnInitialize
        protected virtual void OnInitialize()
        {
        }

        // OnLoadContent
        protected virtual void OnLoadContent()
        {
        }

        // OnPause
        protected virtual void OnPause()
        {
        }

        // OnResume
        protected virtual void OnResume()
        {
        }

        // OnUnloadContent
        protected virtual void OnUnloadContent()
        {
        }

        #endregion

        #region Internal members

        // Activate
        internal void Activate()
        {
            OnActivate();
        }

        // ContinuousUpdate
        internal void ContinuousUpdate(GameTime gameTime)
        {
            OnContinuousUpdate(gameTime);
        }

        // Deactivate
        internal void Deactivate()
        {
            OnDeactivate();
        }

        #endregion

        // BackgroundColor
        public Color BackgroundColor { get; set; } = Color.Transparent;

        // BoundingBox
        public Rectangle BoundingBox { get; private set; }

        // ClearBackground
        public void ClearBackground()
        {
            if (BackgroundColor != Color.Transparent)
                Game.GraphicsDevice.Clear(BackgroundColor);
        }

        // Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // ExclusiveDraw
        public bool ExclusiveDraw { get; init; }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (!InputManager.IsSuspended && IsCurrentScene)
                return OnHandleInput(gameTime);
            else
                return HandleInputResult.Unhandled;
        }

        // Height
        public int Height
        {
            get;
            protected set
            {
                field = value;
                InvalidateBoundingBox();
            }
        }

        // Initialize
        public void Initialize()
        {
            OnInitialize();
        }

        // IsContentLoaded
        public bool IsContentLoaded { get; private set; }

        // IsCurrentScene
        public bool IsCurrentScene => Game.SceneManager.CurrentScene == this;

        // IsPaused
        public bool IsPaused => pauseCount > 0;

        // LoadContent
        public void LoadContent()
        {
            if (!IsContentLoaded)
            {
                content = Game.GetNewContentManager();
                OnLoadContent();
                IsContentLoaded = true;
            }
        }

        // Pause
        public void Pause()
        {
            pauseCount++;

            if (pauseCount == 1)
            {
                SoundInstance.PauseSceneInstances(this, PausedAudioCategories);
                OnPause();
            }
        }

        // PausedAudioCategories
        public AudioCategories PausedAudioCategories { get; protected set; } = AudioCategories.FX | AudioCategories.Voice;

        // PausePreviousScenes
        public bool PausePreviousScenes { get; init; }

        // Resume
        public void Resume()
        {
            if (pauseCount > 0)
            {
                pauseCount--;

                if (pauseCount == 0)
                {
                    SoundInstance.ResumeSceneInstances(this, PausedAudioCategories);
                    OnResume();
                }
            }
        }

        // TransitionAware
        public bool TransitionAware { get; init; } = true;

        // UnloadContent
        public void UnloadContent()
        {
            if (IsContentLoaded)
            {
                OnUnloadContent();
                SoundInstance.StopSceneInstances(this);
                content?.Dispose();
                content = null;
                IsContentLoaded = false;
            }
        }

        // Width
        public int Width
        {
            get;
            protected set
            {
                field = value;
                InvalidateBoundingBox();
            }
        }
    }
}