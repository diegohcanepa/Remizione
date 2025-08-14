using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;

namespace Engendro
{
    /// <summary>|
    /// Scene
    /// </summary>
    public abstract class Scene : GameObject, IDisposable, IInputHandler
    {
        #region Private fields

        private int height;
        private bool isDisposed;
        private int pauseCount;
        private int width;

        #endregion

        #region Constructors

        // Constructor
        protected Scene(EngendroGame game)
            : this(game, 0)
        {
        }

        // Constructor
        protected Scene(EngendroGame game, SceneSettings settings)
            : this(game, settings, string.Empty)
        {
        }

        // Constructor
        protected Scene(EngendroGame game, SceneSettings settings, string name)
            : base(game)
        {
            this.SceneController = new SceneController(this)
            {
                ExclusiveDraw = settings.HasFlag(SceneSettings.ExclusiveDraw),
                PausePreviousScenes = settings.HasFlag(SceneSettings.PausePreviousScenes)
            };

            this.SceneSettings = settings;
            this.SceneName = string.IsNullOrWhiteSpace(name) ? GetType().Name : name;
        }

        #endregion

        #region Private members

        // InvalidateBoundingBox
        private void InvalidateBoundingBox()
        {
            BoundingBox = new Rectangle(0, 0, Width, Height);
        }

        #endregion

        #region Protected members

        // BeforeDraw
        protected override void BeforeDraw(GameTime gameTime)
        {
            if (BackgroundColor != Color.Transparent)
            {
                Game.GraphicsDevice.Clear(BackgroundColor);
            }
        }

        // Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            if (disposing)
            {
                Content?.Dispose();
            }

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

        // OnInvalidate
        protected virtual void OnInvalidate()
        {
        }

        // OnLoadContent
        protected virtual void OnLoadContent()
        {
        }

        // OnUnloadContent
        protected virtual void OnUnloadContent()
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

        #endregion

        #region Internal members

        // Activate
        internal void Activate() => OnActivate();

        // ContinuousUpdate
        internal void ContinuousUpdate(GameTime gameTime)
        {
            OnContinuousUpdate(gameTime);
        }

        // Deactivate
        internal void Deactivate() => OnDeactivate();

        #endregion

        // BackgroundColor
        public Color BackgroundColor { get; set; } = Color.Transparent;

        // BoundingBox
        public Rectangle BoundingBox { get; private set; }

        // Content
        public ContentManager? Content { get; private set; }

        // Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

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
            get => height;
            protected set
            {
                height = value;
                InvalidateBoundingBox();
            }
        }

        // Initialize
        public void Initialize() => OnInitialize();

        // Invalidate
        public void Invalidate() => OnInvalidate();

        // IsContentLoaded
        public bool IsContentLoaded { get; private set; }

        // IsCurrentScene
        public bool IsCurrentScene => Game.SceneManager.CurrentScene == this;

        // IsMouseVisible
        public bool IsMouseVisible { get; set; } = true;

        // IsPaused
        public bool IsPaused => pauseCount > 0;

        // LoadContent
        public void LoadContent()
        {
            if (!IsContentLoaded)
            {
                Content = Game.GetNewContentManager();
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
                for (var i = 0; i < SoundInstance.RunningInstances.Count; i++)
                {
                    var soundInstance = SoundInstance.RunningInstances[i];
                    if (soundInstance.Scene == this && soundInstance.PauseAware)
                    {
                        soundInstance.Pause();
                    }
                }

                OnPause();
            }
        }

        // Resume
        public void Resume()
        {
            if (pauseCount > 0)
            {
                pauseCount--;

                if (pauseCount == 0)
                {
                    for (var i = 0; i < SoundInstance.RunningInstances.Count; i++)
                    {
                        var soundInstance = SoundInstance.RunningInstances[i];
                        if (soundInstance.Scene == this)
                        {
                            soundInstance.Resume();
                        }
                    }

                    OnResume();
                }
            }
        }

        // SceneController
        public SceneController SceneController { get; }

        // SceneName
        public string SceneName { get; }

        // SceneSettings
        public SceneSettings SceneSettings { get; set; }

        // UnloadContent
        public void UnloadContent()
        {
            if (IsContentLoaded)
            {
                OnUnloadContent();
                Content?.Dispose();
                Content = null;
                IsContentLoaded = false;
            }
        }

        // Width
        public int Width
        {
            get => width;
            protected set
            {
                width = value;
                InvalidateBoundingBox();
            }
        }
    }
}
