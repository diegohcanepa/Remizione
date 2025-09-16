using Adberration;
using Adberration.Scripting;
using Adberration.Scripting.Core;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using Remizione.Scripting;
using System;
using System.Collections.Generic;
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

        private readonly ScriptConsole? console;
        private readonly EchoScene echoScene;
        private readonly Dictionary<string, MetaItem[]> friendlyItems = [];
        private Actor? player;
        private Vector2? playerPosition;
        private int power;
        private readonly List<RideRoom> rideRooms = [];
        private readonly RoomEditor? roomEditor;
        private readonly List<GameThing> staticThings = [];
        private readonly Dictionary<string, GameThing> staticThingsDict = [];

        #endregion

        #region Constructor

        // Constructor
        public GameSession(RemizioneGame game, int slotNumber)
            : base(game, new RemizionePersistenceModel(), ContentHelper.EncodePath(game.Content, ContentFolder.System, "ScriptLibrary.esl"), slotNumber)
        {
            this.Game = game;
            this.Environment = new Environment(this);
            this.HUD = new HUD(this);
            this.StaticThings = new(staticThings);
            this.IsMouseVisible = false;

            ObjectPools = new ObjectPools(this);
            ImpactWordPool = new ObjectPool<ImpactWord>(() => new ImpactWord(game), 100);

            BackgroundColor = ColorPalette.BackgroundColor;
            Camera.SmoothSpeed = GameSettings.CameraSmoothSpeed;

            if (EngendroGame.DebugMode)
            {
                TextSprite consoleText = new(game, Fonts.CommonOutline)
                {
                    Color = ColorPalette.HighlightedText,
                    PivotOrigin = RectanglePoint.LeftBottom,
                    Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom),
                    Scale = ScaleInfo.Text.VeryLarge,
                };

                console = new ScriptConsole(this, InputBindings.Console, consoleText, new RectangleF(0, 240, 480, 30), "=>> $BeginRun()", "=>> $AdvanceRun()")
                {
                    TextErrorColor = ColorPalette.Text.Terra
                };

                roomEditor = new RoomEditor(this);
            }

            this.echoScene = new(Game);

            LocalizationSource = LocalizationSource.Script;
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
            scriptRegistry.RegisterEntity(typeof(BloodyEye));
            scriptRegistry.RegisterEntity(typeof(BreakableProp));
            scriptRegistry.RegisterEntity(typeof(CreditsRoom));
            scriptRegistry.RegisterEntity(typeof(GameRoom));
            scriptRegistry.RegisterEntity(typeof(HellGoat));
            scriptRegistry.RegisterEntity(typeof(IsometricProp));
            scriptRegistry.RegisterEntity(typeof(OcculusMinion));
            scriptRegistry.RegisterEntity(typeof(OutgoingRideCar));
            scriptRegistry.RegisterEntity(typeof(PostClock));
            scriptRegistry.RegisterEntity(typeof(Pottery));
            scriptRegistry.RegisterEntity(typeof(Prop));
            scriptRegistry.RegisterEntity(typeof(RideRoom));
            scriptRegistry.RegisterEntity(typeof(SpearTrap));
            scriptRegistry.RegisterEntity(typeof(Tower));
            scriptRegistry.RegisterEntity(typeof(Trunk));
            scriptRegistry.RegisterEntity(typeof(WaterPuddle));

            scriptRegistry.RegisterStatement("add-dialog-option", typeof(AddDialogOptionCommand));
            scriptRegistry.RegisterStatement("add-hole", typeof(AddHoleCommand), CodingContext.EntityDeclaration);
            scriptRegistry.RegisterStatement("add-item", typeof(AddItemCommand), CodingContext.Any);
            scriptRegistry.RegisterStatement("add-light", typeof(AddLightCommand), CodingContext.EntityDeclaration);
            scriptRegistry.RegisterStatement("add-loot-item", typeof(AddLootItemCommand), CodingContext.Initialization);
            scriptRegistry.RegisterStatement("add-trigger-area", typeof(AddTriggerAreaCommand), CodingContext.EntityDeclaration);
            scriptRegistry.RegisterStatement("add-walk-area", typeof(AddWalkAreaCommand), CodingContext.EntityDeclaration);
            scriptRegistry.RegisterStatement("animate-actor", typeof(AnimateActorCommand));
            scriptRegistry.RegisterStatement("await-chance-roll", typeof(AwaitChanceRollCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("await-credits", typeof(AwaitCreditsCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("await-dialog-block", typeof(AwaitDialogBlockCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("await-player-approach", typeof(AwaitPlayerApproachCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("await-popup", typeof(AwaitPopupCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("begin-loot-table", typeof(BeginLootTableCommand), CodingContext.Initialization);
            scriptRegistry.RegisterStatement("begin-rain", typeof(BeginRainCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("create-dialog-block", typeof(CreateDialogBlockCommand));
            scriptRegistry.RegisterStatement("echo", typeof(EchoCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("end-loot-table", typeof(EndLootTableCommand), CodingContext.Initialization);
            scriptRegistry.RegisterStatement("ensure-session-scene", typeof(EnsureSessionSceneCommand));
            scriptRegistry.RegisterStatement("exit-session", typeof(ExitSessionCommand));
            scriptRegistry.RegisterStatement("friendly-items", typeof(FriendlyItemsCommand));
            scriptRegistry.RegisterStatement("meta-item", typeof(MetaItemCommand), CodingContext.Declaration);
            scriptRegistry.RegisterStatement("placement-data", typeof(PlacementDataCommand), CodingContext.EntityDeclaration);
            scriptRegistry.RegisterStatement("say", typeof(SayCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("select-walk-area", typeof(SelectWalkAreaCommand));
            scriptRegistry.RegisterStatement("set-light", typeof(SetLightCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("set-thing-light", typeof(SetThingLightCommand), CodingContext.EntityDeclaration);
            scriptRegistry.RegisterStatement("terminate-dialog-block", typeof(TerminateDialogBlockCommand));
            scriptRegistry.RegisterStatement("use-friendly-item", typeof(UseFriendlyItemCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("use-item", typeof(UseItemCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("vibrate", typeof(VibrateCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("x-tween", typeof(XTweenCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("y-tween", typeof(YTweenCommand), CodingContext.Execution);
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

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
            IsPowerRestored = false;
            RemainingTime = GameSettings.CountdownMaximum;
            RequiredPower = Room is ProceduralRoom proceduralRoom ? proceduralRoom.RequiredPower : 0;
            player?.Inventory.NotifyRoomChanged();
            Environment.EnterRoom();
        }

        // OnExitRoom
        protected override void OnExitRoom(Room currentRoom, Room nextRoom)
        {
            Environment.ExitRoom();
            HUD.Reset();
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

            // GameplayMode
            if (sessionNode.Attributes[nameof(GameplayMode)]?.Value is string gameplayMode)
                GameplayMode = Enum.Parse<GameplayMode>(gameplayMode);

            // Player
            if (sessionNode.Attributes[nameof(Player)]?.Value is string player)
                Player = GetEntity<Actor>(player);

            // Player position
            if (sessionNode.Attributes[nameof(playerPosition)]?.Value is string playerPositionValue)
                playerPosition = XmlConverterExtension.ToVector2(playerPositionValue);

            ////////////////
            // Stats
            ////////////////

            // Deaths
            if (sessionNode.Attributes[nameof(Stats.Deaths)]?.Value is string deaths)
                this.Stats.Deaths = XmlConvert.ToInt32(deaths);

            // Runs
            if (sessionNode.Attributes[nameof(Stats.Runs)]?.Value is string runs)
                this.Stats.Runs = XmlConvert.ToInt32(runs);
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
                if (entity is not GameThing thing)
                    continue;

                if (thing.InstanceKind == InstanceKind.Static)
                {
                    staticThings.Add(thing);
                    staticThingsDict.Add(thing.StaticName, thing);
                }
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (IsCurrentScene && GameplayMode == GameplayMode.Run && RemainingTime >= 0)
                RemainingTime -= gameTime.ElapsedGameTime.Milliseconds;

            if (console != null)
            {
                if (console.IsActive && roomEditor != null)
                    roomEditor.IsActive = false;

                console?.Update(gameTime);
            }

            roomEditor?.HandleInput();

            Environment.Update(gameTime);
            HUD.Update(gameTime);

            // Check game over condition
            if (!IsAwaiting)
            {
                if (RemainingTime <= 0 || Player?.IsDead == true)
                {
                    Stats.Deaths++;
                    AwaitRoutine(RoutineNames.GameOver);
                }
            }
        }

        // OnWrite
        protected override void OnWrite(XmlWriter output)
        {
            // GameplayMode
            output.WriteAttributeString(nameof(GameplayMode), XmlConvert.ToString((int)GameplayMode));

            // Player
            if (Player != null)
                output.WriteAttributeString(nameof(Player), Player.Name);

            // PlayerPosition
            if (playerPosition.HasValue)
                output.WriteAttributeString(nameof(playerPosition), XmlConverterExtension.ToString(playerPosition.Value));

            ////////////////
            // Stats
            ////////////////

            // Deaths
            output.WriteAttributeString(nameof(SessionStats.Deaths), XmlConvert.ToString(Stats.Deaths));

            // Runs
            output.WriteAttributeString(nameof(SessionStats.Runs), XmlConvert.ToString(Stats.Runs));
        }

        #endregion

        // AdvanceRun
        [ScriptMethod]
        public void AdvanceRun()
        {
            if (!IsRunInProgress)
                throw new InvalidOperationException("No run in progress.");

            RunProgress++;

            if (RunProgress == rideRooms.Count)
            {
                Stats.Runs++;
                EndRun();
            }
            else
            {
                EnterRoom(rideRooms[RunProgress]);
            }
        }

        // BeginRun
        [ScriptMethod]
        public void BeginRun()
        {
            if (IsRunInProgress)
                throw new InvalidOperationException("A run is already in progress.");

            IsRunInProgress = true;

            this.RandomSeed = System.Environment.TickCount;

            for (var i = 0; i < RunLength; i++)
            {
                var room = CreateRuntimeRoomClone("RideRoom", $"RideRoom*{i}") as RideRoom ?? throw new InvalidOperationException($"Failed to create runtime clone from RideRoom.");
                rideRooms.Add(room);
            }

            AdvanceRun();
        }

        // DialogOptionId
        [ScriptProperty]
        public int DialogOptionId { get; set; }

        // EndRun
        [ScriptMethod]
        public void EndRun()
        {
            if (!IsRunInProgress)
                return;

            CleanUpRuntimeEntities();
            rideRooms.Clear();
            IsRunInProgress = false;
            RunProgress = -1;
        }

        // Environment
        public Environment Environment { get; }

        // FriendlyItemTarget
        [ScriptProperty]
        public Prop? FriendlyItemTarget { get; set; }

        // Game
        public new RemizioneGame Game { get; }

        // GameplayMode
        [ScriptProperty]
        public GameplayMode GameplayMode { get; set; }

        // GetFriendlyItems
        public MetaItem[] GetFriendlyItems(string staticName)
        {
            if (friendlyItems.TryGetValue(staticName, out var items))
                return items;
            else
                return [];
        }

        // GetStaticThing
        public GameThing? GetStaticThing(string name)
        {
            return staticThingsDict.TryGetValue(name, out var thing) ? thing : null;
        }

        // HasFriendlyItems
        public bool HasFriendlyItems(string staticName) => friendlyItems.ContainsKey(staticName);

        // HUD
        public HUD HUD { get; }

        // ImpactWordPool
        public ObjectPool<ImpactWord> ImpactWordPool { get; }

        // IsConsoleVisible
        public bool IsConsoleVisible => console?.IsActive ?? false;

        // IsHUDVisible
        [ScriptProperty]
        public bool IsHUDVisible { get; set; } = true;

        // IsPowerRestored
        public bool IsPowerRestored { get; private set; }

        // IsRunInProgress
        [ScriptProperty]
        public bool IsRunInProgress { get; private set; }

        // IsTimeCritical
        public bool IsTimeCritical => RemainingTime <= GameSettings.TimeCritical;

        // LightingSystem
        [ScriptProperty]
        public bool LightingSystem { get; set; } = true;

        // NextRoom
        [ScriptProperty]
        public new GameRoom? NextRoom => (GameRoom?)base.NextRoom;

        // ObjectPools
        public ObjectPools ObjectPools { get; }

        // OutgoingRideCar
        [ScriptProperty]
        public OutgoingRideCar? OutgoingRideCar => OutcomeTarget as OutgoingRideCar;

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

        // KillPlayer
        [ScriptMethod]
        public void KillPlayer()
        {
            if (Player != null && !Player.IsDead)
            {
                Environment.Lightning.Show(Player.Position);
                Player.Die();
            }
        }

        // Power
        [ScriptProperty]
        public int Power
        {
            get => power;
            set
            {
                if (value != power)
                {
                    power = Math.Min(value, RequiredPower);

                    if (power == RequiredPower && !IsPowerRestored)
                    {
                        IsPowerRestored = true;
                        Sound.Play(SoundNames.PowerRestored);
                        HUD.Message.Show(HUDMessageKind.PowerRestored);
                    }
                }
            }
        }

        // PreviousRoom
        [ScriptProperty]
        public new GameRoom? PreviousRoom => (GameRoom?)base.PreviousRoom;

        // RandomSeed
        public int RandomSeed { get; private set; }

        // RegisterFriendlyItems
        public void RegisterFriendlyItems(string staticName, params MetaItem[] metaItems)
        {
            friendlyItems[staticName] = metaItems;
        }

        // RemainingTime
        [ScriptProperty]
        public int RemainingTime { get; set; } = int.MaxValue;

        // RequiredPower
        [ScriptProperty]
        public int RequiredPower { get; private set; }

        // Room
        [ScriptProperty]
        public new GameRoom? Room => (GameRoom?)base.Room;

        // RunLength
        public int RunLength { get; set; } = 3;

        // RunProgress
        [ScriptProperty]
        public int RunProgress { get; private set; } = -1;

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
        public NamedObjectReadOnlyCollection<GameThing> StaticThings { get; }

        // Stats
        public SessionStats Stats { get; } = new();
    }
}