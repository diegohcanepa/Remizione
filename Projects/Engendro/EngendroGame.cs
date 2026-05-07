using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engendro
{
    /// <summary>
    /// EngendroGame
    /// </summary>
    public partial class EngendroGame : Game
    {
        #region Private fields

        private bool disposed;
        private static float fpsElapsedTime;
        private static int fpsFrames;
        private bool isPaused;
        private Renderer2D? renderTargets;

        #endregion

        #region Static constructor

        // EngendroGame
        static EngendroGame()
        {
#if DEBUG
            DebugMode = true;
#else
            DebugMode = false;
#endif
        }

        #endregion

        #region Constructor

        // Constructor
        public EngendroGame(string title, string contentRootDirectory, int nativeWidth, int nativeHeight, RunningPlatform platform)
            : base()
        {
            CodeContract.NotEmpty(contentRootDirectory, nameof(contentRootDirectory));

            Instance = Instance != null ? throw new InvalidOperationException("This class cannot be instantiated twice.") : this;

            RunningPlatform = platform;

            Graphics = new GraphicsDeviceManager(this)
            {
                GraphicsProfile = GraphicsProfile.HiDef
            };

            Content.RootDirectory = contentRootDirectory;

            this.ViewportAdapter = new ViewportAdapter(nativeWidth, nativeHeight);
            this.Camera = new Camera("UI", nativeWidth, nativeHeight);
            this.SceneManager = new();
            this.Shapes = new();

            Window.Title = title;
            Window.ClientSizeChanged += Window_ClientSizeChanged;
        }

        #endregion

        #region Private members

        // Pause
        private void Pause()
        {
            if (!isPaused)
            {
                isPaused = true;
                AudioManager.Pause();
            }
        }

        // Resume
        private void Resume()
        {
            if (isPaused)
            {
                isPaused = false;
                AudioManager.Resume();
            }
        }

        // SwitchDisplayMode
        private void SwitchDisplayMode(int width, int height, bool isFullScreen)
        {
            if (Graphics == null)
                throw new InvalidOperationException("GraphicsDeviceManager not initialized.");

            Graphics.PreferredBackBufferWidth = width;
            Graphics.PreferredBackBufferHeight = height;
            Graphics.IsFullScreen = isFullScreen;
            Graphics.ApplyChanges();
            ViewportAdapter.SetDisplaySize(width, height);

            if (renderTargets != null)
            {
                renderTargets.Dispose();
                renderTargets = new();
            }
        }

        // Window_ClientSizeChanged
        private void Window_ClientSizeChanged(object? sender, EventArgs e)
        {
            if (Window.ClientBounds.Width > 0 && Window.ClientBounds.Height > 0)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        #endregion

        #region Protected members

        // Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                SpriteBatch.Dispose();
                renderTargets?.Dispose();
            }

            foreach (var player in InputManager.Players)
            {
                player.GamePad.StopVibration();
            }

            disposed = true;

            base.Dispose(disposing);
        }

        // Draw
        protected sealed override void Draw(GameTime gameTime)
        {
            if (!IsActive)
                return;

            RenderTargets.Swap();
            SceneManager.Draw(gameTime);
            GraphicsDevice.SetRenderTarget(null);
            OnDraw(gameTime);
        }

        // Graphics
        protected GraphicsDeviceManager Graphics { get; }

        // Initialize
        protected sealed override void Initialize()
        {
            base.Initialize();
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            OnInitialize();
        }

        // OnActivated
        protected override void OnActivated(object sender, EventArgs args)
        {
            base.OnActivated(sender, args);
            Resume();
        }

        // OnDeactivated
        protected override void OnDeactivated(object sender, EventArgs args)
        {
            base.OnDeactivated(sender, args);
            Pause();
        }

        // OnDraw
        protected virtual void OnDraw(GameTime gameTime)
        {
        }

        // OnInitialize
        protected virtual void OnInitialize()
        {
        }

        // OnUpdate
        protected virtual void OnUpdate(GameTime gameTime)
        {
        }

        // Update
        protected sealed override void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;

            InputManager.Update(gameTime);
            AudioManager.Update(gameTime);
            SceneManager.Update(gameTime);
            Camera.Update(gameTime);

            fpsFrames++;
            fpsElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (fpsElapsedTime > 1)
            {
                FPS = fpsFrames;
                fpsFrames = 0;
                fpsElapsedTime = 0f;
            }

            OnUpdate(gameTime);
        }

        #endregion

        // Camera
        public Camera Camera { get; }

        // CreateRenderTarget2D
        public RenderTarget2D CreateRenderTarget2D()
        {
            return new RenderTarget2D(GraphicsDevice, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight);
        }

        // DebugMode
        public static bool DebugMode { get; }

        // FPS
        public static int FPS { get; private set; }

        // GetNewContentManager
        public ContentManager GetNewContentManager()
        {
            return new(Content.ServiceProvider, Content.RootDirectory);
        }

        // Instance
        public static EngendroGame Instance { get; private set; } = null!;

        // IsDemo
        public bool IsDemo { get; protected set; }

        // IsFullScreen
        public bool IsFullScreen => Graphics.IsFullScreen;

        // RendererTargets
        public Renderer2D RenderTargets => renderTargets ??= new();

        // RunningPlatform
        public static RunningPlatform RunningPlatform
        {
            get => field == RunningPlatform.Unknown ? throw new InvalidOperationException() : (field);
            private set;
        }

        // SceneManager
        public SceneManager SceneManager { get; }

        // Shapes
        public Shapes Shapes { get; }

        // SpriteBatch
        public SpriteBatch SpriteBatch { get; private set; } = null!;

        // SwitchToFullScreen
        public void SwitchToFullScreen()
        {
            var dm = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            SwitchDisplayMode(dm.Width, dm.Height, true);
        }

        // SwitchToWindowedMode
        public void SwitchToWindowedMode()
        {
            var width = (int)(GraphicsDevice.Adapter.CurrentDisplayMode.Width * .9f);
            var height = (int)(GraphicsDevice.Adapter.CurrentDisplayMode.Height * .9f);

            SwitchToWindowedMode(width, height);
        }

        // SwitchToWindowedMode
        public void SwitchToWindowedMode(int width, int height)
        {
            SwitchDisplayMode(width, height, false);
        }

        // Title
        public string Title
        {
            get => Window.Title;
            set => Window.Title = value;
        }

        // ViewportAdapter
        public ViewportAdapter ViewportAdapter { get; }
    }
}