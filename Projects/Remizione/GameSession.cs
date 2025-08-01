using Engendro;
using Engendro.Audio;
using Engendro.Input;
using EngendroAdventure;
using EngendroAdventure.Scripting;
using EngendroAdventure.Scripting.Core;
using Microsoft.Xna.Framework;
using Remizione.Creatures;
using Remizione.Scripting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace Remizione
{
    /// <summary>
    /// GameSession
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public sealed partial class GameSession : Session
    {
        #region Private fields

        private enum AttributeName { ProcStates, RandomSeed, WorldVersion }
        private readonly ScriptConsole? console;
        private readonly EchoScene echoScene;
        private readonly SoundInstance exitAlarmSound;
        private Actor? player;
        private Vector2? playerPosition;
        private readonly Dictionary<string, int> proceduralThingStates = [];
        private int rainRemainingTime;
        private readonly RoomEditor? roomEditor;
        private readonly List<GameThing> staticThings = [];

        #endregion

        #region Constructor

        // Constructor
        public GameSession(RemizioneGame game, int slotNumber)
            : base(game, new RemizionePersistenceModel(), ContentHelper.EncodePath(game.Content, ContentFolder.System, "ScriptLibrary.esl"), slotNumber)
        {
            this.Game = game;
            this.Environment = new Environment(this);
            this.HUD = new HUD(this);
            //this.RandomSeed = 10000;
            this.RandomSeed = System.Environment.TickCount;
            this.StaticThings = new ReadOnlyCollection<GameThing>(staticThings);

            ObjectPools = new ObjectPools(this);
            ImpactWordPool = new ObjectPool<ImpactWord>(() => new ImpactWord(game), 100);
            OverlayTexts = new OverlayTextManager(game);

            BackgroundColor = ColorPalette.BackgroundColor;
            Camera.SmoothSpeed = GameSettings.CameraSmoothSpeed;
            exitAlarmSound = Sound.FindNotNull(SoundNames.ExitAlarm).PopInstance() ?? throw new InvalidOperationException($"Sound '{SoundNames.ExitAlarm}' not found.");

            if (EngendroGame.DebugMode)
            {
                TextSprite consoleText = new(game, Fonts.CommonOutline)
                {
                    Color = ColorPalette.HighlightedText,
                    PivotOrigin = RectanglePoint.LeftBottom,
                    Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom),
                    Scale = ScaleInfo.Text.VeryLarge,
                };

                console = new ScriptConsole(this, InputBindings.Console, consoleText, new RectangleF(0, 240, 480, 30)) { TextErrorColor = ColorPalette.Text.Terra };
                roomEditor = new RoomEditor(this);
            }

            this.echoScene = new(Game);

            LocalizationSource = LocalizationSource.Script;

            // Global light
            this.GlobalLight ??= new Light(Game, "GlobalLight")
            {
                Color = Color.White,
                LightKind = LightKind.Global,
                ImageName = "GlobalLight",
                PivotOrigin = RectanglePoint.Middle,
                Position = Screen.Center,
            };
            
            this.GlobalLight.Prepare(Atlases.Environment);
        }

        #endregion

        #region Private members

        // UpdateMouseCursor
        private void UpdateMouseCursor()
        {
            if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.GamePad)
            {
                MouseCursor.Instance.State = MouseCursorState.None;
                return;
            }

            // No active player
            if (AwaitingScript?.CurrentStatement is SayCommand)
            {
                MouseCursor.Instance.State = MouseCursorState.Arrow;
                return;
            }

            if (IsAwaiting)
            {
                MouseCursor.Instance.State = MouseCursorState.Wait;
                return;
            }

            MouseCursor.Instance.State = Player?.InteractiveTarget == null ? MouseCursorState.Cross : MouseCursorState.CrossOn;
        }

        #endregion

        #region Protected members

        // CanHandleRoomInput
        protected override bool CanHandleRoomInput
        {
            get
            {
                if (!EngendroGame.DebugMode)
                    return base.CanHandleRoomInput;

                if (console != null && console.IsActive)
                    return false;

                if (roomEditor != null && roomEditor.IsActive)
                    return false;

                return true;
            }
        }

        // ExtendScriptRegistry
        protected override void ExtendScriptRegistry(ScriptRegistry scriptRegistry)
        {
            scriptRegistry.RegisterEntity(typeof(Actor));
            scriptRegistry.RegisterEntity(typeof(Baal));
            scriptRegistry.RegisterEntity(typeof(OutgoingGhostCar));
            scriptRegistry.RegisterEntity(typeof(IsometricProp));
            scriptRegistry.RegisterEntity(typeof(ProceduralRoom));
            scriptRegistry.RegisterEntity(typeof(Orb));
            scriptRegistry.RegisterEntity(typeof(Pickup));
            scriptRegistry.RegisterEntity(typeof(Prop));
            scriptRegistry.RegisterEntity(typeof(GameRoom));
            scriptRegistry.RegisterEntity(typeof(CreditsRoom));
            scriptRegistry.RegisterEntity(typeof(Snail));
            scriptRegistry.RegisterEntity(typeof(Unredeemed));
            scriptRegistry.RegisterEntity(typeof(Zabul));
            scriptRegistry.RegisterEntity(typeof(RoomConnector));

            scriptRegistry.RegisterStatement("add-dialog-option", typeof(AddDialogOptionCommand));
            scriptRegistry.RegisterStatement("add-hole", typeof(AddHoleCommand), CodingContext.EntityDeclaration);
            scriptRegistry.RegisterStatement("add-item", typeof(AddItemCommand), CodingContext.Any);
            scriptRegistry.RegisterStatement("add-light", typeof(AddLightCommand), CodingContext.EntityDeclaration);
            scriptRegistry.RegisterStatement("add-loot-item", typeof(AddLootItemCommand), CodingContext.EntityDeclaration);
            scriptRegistry.RegisterStatement("add-trigger-area", typeof(AddTriggerAreaCommand), CodingContext.EntityDeclaration);
            scriptRegistry.RegisterStatement("add-walk-area", typeof(AddWalkAreaCommand), CodingContext.EntityDeclaration);
            scriptRegistry.RegisterStatement("animate", typeof(AnimateCommand));
            scriptRegistry.RegisterStatement("await-credits", typeof(AwaitCreditsCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("await-dialog-block", typeof(AwaitDialogBlockCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("await-pickup", typeof(AwaitPickUpCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("await-player-approach", typeof(AwaitPlayerApproachCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("await-popup", typeof(AwaitPopupCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("begin-rain", typeof(BeginRainCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("create-dialog-block", typeof(CreateDialogBlockCommand));
            scriptRegistry.RegisterStatement("echo", typeof(EchoCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("ensure-session-scene", typeof(EnsureSessionSceneCommand));
            scriptRegistry.RegisterStatement("exit-session", typeof(ExitSessionCommand));
            scriptRegistry.RegisterStatement("hide-overlay-text", typeof(HideOverlayTextCommand));
            scriptRegistry.RegisterStatement("placement-data", typeof(PlacementDataCommand), CodingContext.EntityDeclaration);
            scriptRegistry.RegisterStatement("meta-item", typeof(MetaItemCommand), CodingContext.Declaration);
            scriptRegistry.RegisterStatement("say", typeof(SayCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("select-walk-area", typeof(SelectWalkAreaCommand));
            scriptRegistry.RegisterStatement("set-light", typeof(SetLightCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("set-thing-light", typeof(SetThingLightCommand), CodingContext.EntityDeclaration);
            scriptRegistry.RegisterStatement("terminate-dialog-block", typeof(TerminateDialogBlockCommand));
            scriptRegistry.RegisterStatement("vibrate", typeof(VibrateCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("x-tween", typeof(XTweenCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("y-tween", typeof(YTweenCommand), CodingContext.Execution);
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            OverlayTexts.Draw(gameTime);

            HUD.Draw(gameTime);

            if (IsPaused)
            {
                Game.SpriteBatch.Begin(Game.Camera);
                Game.Shapes.DrawRectangle(Screen.Area, ColorPalette.SceneShade);
                Game.SpriteBatch.End();
            }

            console?.Draw(gameTime);
            roomEditor?.Draw(gameTime);
        }

        // OnEnterRoom
        protected override void OnEnterRoom(Room room)
        {
            if (room is GameRoom gameRoom)
            {
                player?.Inventory.NotifyRoomChanged();
                Environment.EnterRoom(gameRoom);
            }
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (HUD.HandleInput(gameTime) == HandleInputResult.Handled)
                return HandleInputResult.Handled;
            else
                return base.OnHandleInput(gameTime);
        }

        // OnOutcomeCompleted
        protected override void OnOutcomeCompleted(Thing thing)
        {
            Player?.SuspendInteraction(250);
        }

        // OnPause
        protected override void OnPause()
        {
            base.OnPause();

            if (Room != null)
            {
                for (var i = 0; i < Room.Children.Count; i++)
                {
                    Room.Children[i].Pause();
                }
            }
        }

        // OnRead
        protected override void OnRead(XmlNode sessionNode)
        {
            if (sessionNode == null || sessionNode.Attributes == null)
                throw new InvalidOperationException("Session node attributes not found");

            // Countdown
            if (sessionNode.Attributes[nameof(Countdown)]?.Value is string countdown)
                this.Countdown = XmlConvert.ToInt32(countdown);

            // GameplayMode
            if (sessionNode.Attributes[nameof(GameplayMode)]?.Value is string gameplayMode)
                GameplayMode = Enum.Parse<GameplayMode>(gameplayMode);

            // Player
            if (sessionNode.Attributes[nameof(Player)]?.Value is string player)
                Player = GetEntity<Actor>(player);

            // Player position
            if (sessionNode.Attributes[nameof(playerPosition)]?.Value is string playerPositionValue)
                playerPosition = XmlConverterExtension.ToVector2(playerPositionValue);

            // Procedural thing states
            if (sessionNode.Attributes[AttributeName.ProcStates.ToString()]?.Value is string procStates)
            {
                var states = procStates.Split(',');
                foreach (var state in states)
                {
                    var values = state.Split('=');
                    proceduralThingStates[values[0]] = int.Parse(values[1]);
                }
            }

            // NextRainCooldown
            if (sessionNode.Attributes[nameof(NextRainCooldown)]?.Value is string nextRainCooldown)
                this.NextRainCooldown = XmlConvert.ToInt32(nextRainCooldown);

            // RainRemainingTime
            if (sessionNode.Attributes[nameof(Environment.Rain.RemainingTime)]?.Value is string rainRemainingTime)
                this.rainRemainingTime = XmlConvert.ToInt32(rainRemainingTime);

            // RandomSeed
            if (sessionNode.Attributes[AttributeName.RandomSeed.ToString()]?.Value is string randomSeedValue)
                RandomSeed = XmlConvert.ToInt32(randomSeedValue);

            // WorldVersion
            if (sessionNode.Attributes[AttributeName.WorldVersion.ToString()]?.Value is string worldVersionValue)
                WorldVersion = XmlConvert.ToInt32(worldVersionValue);
        }

        // OnResume
        protected override void OnResume()
        {
            base.OnResume();

            if (Room != null)
            {
                for (var i = 0; i < Room.Children.Count; i++)
                {
                    Room.Children[i].Resume();
                }
            }
        }

        // OnRun
        protected override void OnRun()
        {
            base.OnRun();

            if (rainRemainingTime > 0)
                Environment.Rain.Begin(rainRemainingTime, true);
        }

        // OnSave
        protected override void OnSave()
        {
            base.OnSave();

            if (IsCurrentScene)
                HUD.ShowSavingIcon();
        }

        // OnStart
        protected override void OnStart()
        {
            base.OnStart();

            foreach (var entity in Entities)
            {
                if (entity is GameThing thing && thing.EntityKind == EntityKind.Static)
                    staticThings.Add(thing);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (GameplayMode == GameplayMode.Survival && Countdown >= 0)
            {
                Countdown -= gameTime.ElapsedGameTime.Milliseconds;

                if (IsCountdownActive && !exitAlarmSound.IsPlaying)
                    exitAlarmSound.Play();

                /*
                if (Countdown <= 0)
                    Game.SceneManager.Push(new GameOverScene(Game, GameOverReason.TimeOut));
                */
            }

            if (console != null)
            {
                if (console.IsActive && roomEditor != null)
                    roomEditor.IsActive = false;

                console?.Update(gameTime);
            }

            roomEditor?.HandleInput();

            Environment.Update(gameTime);
            HUD.Update(gameTime);

            OverlayTexts.Update(gameTime);

            if (IsCurrentScene || (Game.SceneManager.CurrentScene != null && !Game.SceneManager.CurrentScene.HasMouseControl))
                UpdateMouseCursor();
        }

        // OnWrite
        protected override void OnWrite(XmlWriter output)
        {
            // Countdown
            output.WriteAttributeString(nameof(Countdown), XmlConvert.ToString(Countdown));

            // GameplayMode
            output.WriteAttributeString(nameof(GameplayMode), XmlConvert.ToString((int)GameplayMode));

            // NextRainCooldown
            output.WriteAttributeString(nameof(NextRainCooldown), XmlConvert.ToString(NextRainCooldown));

            // Player
            if (Player != null)
                output.WriteAttributeString(nameof(Player), Player.Name);

            // PlayerPosition
            if (playerPosition.HasValue)
                output.WriteAttributeString(nameof(playerPosition), XmlConverterExtension.ToString(playerPosition.Value));

            // RainRemainingTime
            output.WriteAttributeString(nameof(Environment.Rain.RemainingTime), XmlConvert.ToString(Environment.Rain.RemainingTime));

            // RandomSeed
            output.WriteAttributeString(AttributeName.RandomSeed.ToString(), XmlConvert.ToString(RandomSeed));

            // Collect state data for procedural things
            if (Room is ProceduralRoom proceduralRoom)
            {
                var stateData = new List<string>();

                foreach (var thing in proceduralRoom.ProceduralThings)
                {
                    if (thing.StateID != 0)
                        stateData.Add($"{thing.Name}={thing.StateID}");
                }

                if (stateData.Count > 0)
                {
                    var value = string.Join(",", stateData);
                    output.WriteAttributeString(AttributeName.ProcStates.ToString(), value);
                }
            }

            // WorldVersion
            output.WriteAttributeString(AttributeName.WorldVersion.ToString(), XmlConvert.ToString(WorldVersion));
        }

        #endregion

        // Countdown
        [ScriptProperty]
        public int Countdown { get; set; } = -1;

        // ClearOverlayTexts
        [ScriptMethod(CodingContext.Any)]
        public void ClearOverlayTexts() => OverlayTexts.Clear();

        // DialogOptionId
        [ScriptProperty]
        public int DialogOptionId { get; set; }

        // Environment
        public Environment Environment { get; }

        // Game
        public new RemizioneGame Game { get; }

        // GameplayMode
        [ScriptProperty]
        public GameplayMode GameplayMode { get; set; }

        // GetProceduralThingState
        public int GetProceduralThingState(string name)
        {
            if (proceduralThingStates.TryGetValue(name, out int state))
                return state;
            
            return 0;
        }

        // GlobalLight
        public Light GlobalLight { get; }

        // GlobalLightSize
        [ScriptProperty]
        public Vector2 GlobalLightSize
        {
            get => GlobalLight.Scale;
            set => GlobalLight.Scale = value;
        }

        // HUD
        public HUD HUD { get; }

        // ImpactWordPool
        public ObjectPool<ImpactWord> ImpactWordPool { get; }

        // IsConsoleVisible
        public bool IsConsoleVisible => console?.IsActive ?? false;

        // IsCountdownActive
        public bool IsCountdownActive => Countdown.IsBetween(0, GameSettings.CountdownAlert) && GameplayMode == GameplayMode.Survival;

        // IsHUDVisible
        [ScriptProperty]
        public bool IsHUDVisible { get; set; } = true;

        // Level
        [ScriptProperty]
        public int Level { get; private set; }

        // LightingSystem
        [ScriptProperty]
        public bool LightingSystem { get; set; } = true;

        // NextLevel
        [ScriptMethod]
        public void NextLevel()
        {
            exitAlarmSound.Stop(3000);
            Countdown = Randomizer.Next(GameSettings.CountdownMinimum, GameSettings.CountdownMaximum);
            Level++;
        }

        // NextRainCooldown
        [ScriptProperty(CodingContext.Any)]
        public int NextRainCooldown { get; private set; }

        // NextRoom
        [ScriptProperty]
        public new GameRoom? NextRoom => (GameRoom?)base.NextRoom;

        // ObjectPools
        public ObjectPools ObjectPools { get; }

        // OutgoingGhostCar
        [ScriptProperty]
        public OutgoingGhostCar? OutgoingGhostCar => OutcomeTarget as OutgoingGhostCar;

        // OverlayTexts
        public OverlayTextManager OverlayTexts { get; }

        // Player
        [ScriptProperty]
        public Actor? Player
        {
            get => player;
            set
            {
                if (value != player)
                {
                    this.player = value;
                    HUD.Reset();
                }
            }
        }

        // PreviousRoom
        [ScriptProperty]
        public new GameRoom? PreviousRoom => (GameRoom?)base.PreviousRoom;

        // RandomSeed
        public int RandomSeed { get; private set; }

        // Room
        [ScriptProperty]
        public new GameRoom? Room => (GameRoom?)base.Room;

        // ShakeCamera
        public void ShakeCamera(ImpactType impactType)
        {
            // Shake camera
            if (impactType == ImpactType.Low)
                Camera.Shake(TweenStyle.Linear, new Vector2(.7f), 70, 2);

            else if (impactType == ImpactType.Medium)
                Camera.Shake(TweenStyle.Linear, new Vector2(1.2f), 70, 2);

            else
                Camera.Shake(TweenStyle.Linear, new Vector2(3.4f), 50, 4);
        }

        // ShowEcho
        public void ShowEcho(string text)
        {
            echoScene.Text = text;
            Game.SceneManager.Push(echoScene);
        }

        // StaticThings
        public ReadOnlyCollection<GameThing> StaticThings { get; }

        // WorldVersion
        public int WorldVersion { get; set; } = 1;
    }
}