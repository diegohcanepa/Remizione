using Adberration;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScaryCastle.Menus;
using ScaryCastle.UI;

namespace ScaryCastle
{
    /// <summary>
    /// ScaryCastleGame
    /// </summary>
    public partial class ScaryCastleGame : AdventureGame
    {
        #region Constructor

        // Constructor
        public ScaryCastleGame(RunningPlatform platform, PlatformBridge platformBridge)
            : base(GameSettings.Title, GameSettings.ContentRootDirectory, Screen.NativeWidth, Screen.NativeHeight, platform, platformBridge)
        {
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
            CombatBehavior.Behaviors.Load(ContentManagerExtension.EncodePath(Content, ContentFolder.System, "CombatBehaviors.json"));
            ItemDefinition.Definitions.Load(ContentManagerExtension.EncodePath(Content, ContentFolder.System, "Items.json"));
            RoomDefinition.Definitions.Load(ContentManagerExtension.EncodePath(Content, ContentFolder.System, "Rooms.json"));
            ActorDefinition.Definitions.Load(ContentManagerExtension.EncodePath(Content, ContentFolder.System, "Actors.json"));
            PropDefinition.Definitions.Load(ContentManagerExtension.EncodePath(Content, ContentFolder.System, "Props.json"));
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
                var scene = new PauseMenuScene(CurrentSession);
                SceneManager.Push(scene);
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (SceneManager.CurrentScene != null && SceneManager.CurrentScene.TransitionAware)
                Effects.ColorReduction.SetColor(1 - TransitionManager.CurrentTransition.VisibleRatio);
            else
                Effects.ColorReduction.SetColor(1);

            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, effect: Effects.ColorReduction.Effect);
            SpriteBatch.Draw(RenderTargets.CurrentTarget, ViewportAdapter.DestinationRectangle, Color.White);
            SpriteBatch.End();

            MouseCursor.Draw(gameTime);
        }

        // OnScenesDrawn
        protected override void OnScenesDrawn(GameTime gameTime)
        {
            RenderTargets.Swap();
            SpriteBatch.Begin(effect: Effects.CRT.Effect);
            SpriteBatch.Draw(RenderTargets.PreviousTarget, Vector2.Zero, Color.White);
            SpriteBatch.End();

            // Draw speech bubbles
            SpeechBubble.DrawSpeechBubbles(gameTime);

            MonitorFrame.Draw(gameTime);
        }

        // OnInitialize
        protected override void OnInitialize()
        {
            base.OnInitialize();

            Effects = new();
            LocalizationManager.Initialize(this);
            AudioManager.Load(ContentManagerExtension.EncodePath(Content, ContentFolder.System, "Sounds.xml"));

            Fonts.Initialize(Content);
            UserSettingsData userSettings = UserSettingsData.Load(this);

            // TODO: Dev only
            userSettings.LanguageTag = LocalizationManager.SpanishLatinAmerica;

            UserSettingsData.Apply(this, userSettings);

            InitializeProceduralContent();

#if QUICK_START
            QuickStart();
#else
            DefaultStart();
#endif
        }

        // OUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
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
            SceneManager.Push(CurrentSession);
            CurrentSession.Run();
        }

        #endregion

        // CurrentSession
        public GameSession? CurrentSession { get; private set; }

        // Effects
        public static GameEffects Effects { get; private set; } = null!;

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