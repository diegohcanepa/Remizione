using Adberration;
using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remizione.Menus;

namespace Remizione
{
    /// <summary>
    /// RemizioneGame
    /// </summary>
    public partial class RemizioneGame : AdventureGame
    {
        #region Constructor

        // Constructor
        public RemizioneGame(RunningPlatform platform, PlatformBridge platformBridge)
            : base(GameSettings.Title, GameSettings.ContentRootDirectory, Screen.NativeWidth, Screen.NativeHeight, platform, platformBridge)
        {
            this.MouseCursor = new MouseCursor(this);

            AudioManager.AmbienceCategory.ContentPath = ContentManagerExtension.EncodeAudioPath(ContentFolder.Ambience);
            AudioManager.MusicCategory.ContentPath = ContentManagerExtension.EncodeAudioPath(ContentFolder.Music);
            AudioManager.FXCategory.ContentPath = ContentManagerExtension.EncodeAudioPath(ContentFolder.FX);
            AudioManager.VoiceCategory.ContentPath = ContentManagerExtension.EncodeAudioPath(ContentFolder.Voices);

            IsFixedTimeStep = false;
            Graphics.SynchronizeWithVerticalRetrace = true;
            IsMouseVisible = false;

#if RELEASE
            //PauseOnDeactivate = true;
#else
            //PauseOnDeactivate = false;
#endif

#if DEMO
            IsDemo = true;
#endif

            TransitionManager.DefaultTransition.DefaultDuration = GameSettings.DefaultTransitionDuration;

            //InputManager.AllowGamePad = false;
        }

        #endregion

        #region Private members

        // InitializeProceduralContent
        private void InitializeProceduralContent()
        {
            MetaItem.Load(ContentManagerExtension.EncodePath(Content, ContentFolder.System, "MetaItems.json"));
            RoomConfig.Load(ContentManagerExtension.EncodePath(Content, ContentFolder.System, "Rooms.json"));
            ThingConfig.Load(ContentManagerExtension.EncodePath(Content, ContentFolder.System, "Actors.json"),
                             ContentManagerExtension.EncodePath(Content, ContentFolder.System, "Props.json"));
            
            RideRoom.RegisterRideRoom(typeof(CommonRoom));
        }

#if !QUICK_START
        // DefaultStart
        private void DefaultStart()
        {
            //VladUtils.PlayMenuBackgroundSound();

            var publisherSplash = new SplashScene(this, Atlases.Menu.PublisherLogo, Screen.Center - new Vector2(0, 22), .16f);
            var developerSplash = new SplashScene(this, Atlases.Menu.DeveloperLogo, Screen.Center - new Vector2(0, 20), .21f);

            publisherSplash.NextScene = developerSplash;

            developerSplash.NextScene = new PreloadScene(this);

            SceneManager.Push(publisherSplash);
        }
#else
        // QuickStart
        private void QuickStart()
        {
            StartSession(0);
        }
#endif

        // ShowPauseScene
        private void ShowPauseScene()
        {
            if (!SceneManager.Contains<PauseMenuScene>() && CurrentSession?.Room is GameRoom room && room.AllowPauseMenu)
            {
                new PauseMenuScene(CurrentSession).SceneController.Push();
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (SceneManager.CurrentScene != null && SceneManager.CurrentScene.SceneController.TransitionAware)
                Effects.ColorReduction.SetColor(1 - TransitionManager.CurrentTransition.VisibleRatio);
            else
                Effects.ColorReduction.SetColor(1);

            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, effect: Effects.ColorReduction.Effect);
            SpriteBatch.Draw(RenderTargets.CurrentTarget, ViewportAdapter.DestinationRectangle, Color.White);
            SpriteBatch.End();

            if (InputManager.AllowMouse && SceneManager.CurrentScene?.IsMouseVisible == true)
                MouseCursor.Draw(gameTime);
        }

        // OnInitialize
        protected override void OnInitialize()
        {
            base.OnInitialize();

            Effects = new GameEffects(this);
            LocalizationManager.Initialize(this);
            AudioManager.Load(ContentManagerExtension.EncodePath(Content, ContentFolder.System, "SoundData.xml"));
            
            Fonts.Initialize(Content);
            UserSettingsData userSettings = UserSettingsData.Load(this);
            UserSettingsData.Apply(this, userSettings);

            InitializeProceduralContent();

#if QUICK_START
            QuickStart();
#else
            DefaultStart();
#endif
        }

        // OnSceneManagerDrawn
        protected override void OnSceneManagerDrawn(GameTime gameTime)
        {
            RenderTargets.Swap();
            SpriteBatch.Begin(effect: Effects.CRT.Effect);
            SpriteBatch.Draw(RenderTargets.PreviousTarget, Vector2.Zero, Color.White);
            SpriteBatch.End();
        }

        // OUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (InputManager.AllowMouse)
                MouseCursor.Update(gameTime);

            //?
            /*
            if (PlatformBridge.DetectControllerDisconnection)
            {
                if (SceneManager.CurrentScene is ControllerDisconnectedScene)
                {
                    return;
                }
                else if (!InputManager.AllowKeyboard && !InputHelper.IsPlayerGamePadConnected)
                {
                    ControllerDisconnectedScene messageBox = new(this);
                    SceneManager.Push(messageBox);
                    return;
                }
            }
            */

            /*
            if (CurrentSession != null && (CurrentSession.IsCurrentScene || SceneManager.CurrentScene is DialogBoxScene))
            {
                if (InputBindings.PauseMenu.IsPressed(InputHelper.PlayerIndex))
                {
                    ShowPauseScene();
                }
            }
            */
        }

        #endregion

        #region Internal members

        // DisposeSession
        internal void DisposeSession(Scene nextScene)
        {
            if (CurrentSession == null)
            {
                return;
            }

            SceneManager.Clear();

            CurrentSession.Dispose();

            AudioManager.Reset();

            CurrentSession = null;

            SceneManager.Push(nextScene);
        }

        // StartSession
        internal void StartSession(int slotNumber)
        {
            CurrentSession = new GameSession(this, slotNumber);
            CurrentSession.Start();
            CurrentSession.SceneController.Push();
            CurrentSession.Run();
        }

        #endregion

        // CurrentSession
        public GameSession? CurrentSession { get; private set; }

        // Effects
        public static GameEffects Effects { get; private set; } = null!;

        // MouseCursor
        public MouseCursor MouseCursor { get; }

        // Play
        public void Play(int slotNumber)
        {
            TransitionManager.CurrentTransition.Out(1000);

#if QUICK_START
            StartSession(slotNumber);
#else
            SceneManager.Push(new AutoSaveAdviceScene(this, slotNumber));
#endif
        }
    }
}