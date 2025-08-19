using Adberration;
using Adberration.Scripting;
using Adberration.Scripting.Core;
using Engendro;
using Microsoft.Xna.Framework;
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

        private enum AttributeName { RandomSeed, WorldVersion }
        private readonly ScriptConsole? console;
        private readonly EchoScene echoScene;
        private int energy;
        private readonly Dictionary<string, MetaItem[]> friendlyItems = [];
        private Actor? player;
        private Vector2? playerPosition;
        private int rainRemainingTime;
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
            //this.RandomSeed = 10000;
            this.RandomSeed = System.Environment.TickCount;
            this.StaticThings = new ReadOnlyCollection<GameThing>(staticThings);
            this.IsMouseVisible = false;

            ObjectPools = new ObjectPools(this);
            ImpactWordPool = new ObjectPool<ImpactWord>(() => new ImpactWord(game), 100);
            OverlayTexts = new OverlayTextManager(game);

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

                console = new ScriptConsole(this, InputBindings.Console, consoleText, new RectangleF(0, 240, 480, 30)) { TextErrorColor = ColorPalette.Text.Terra };
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
            scriptRegistry.RegisterEntity(typeof(LootBag));
            scriptRegistry.RegisterEntity(typeof(OcculusMinion));
            scriptRegistry.RegisterEntity(typeof(OutgoingGhostCar));
            scriptRegistry.RegisterEntity(typeof(Pickup));
            scriptRegistry.RegisterEntity(typeof(Pottery));
            scriptRegistry.RegisterEntity(typeof(ProceduralRoom));
            scriptRegistry.RegisterEntity(typeof(Prop));
            scriptRegistry.RegisterEntity(typeof(RoomConnector));
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
            scriptRegistry.RegisterStatement("await-pickup", typeof(AwaitPickUpCommand), CodingContext.Execution);
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
            scriptRegistry.RegisterStatement("hide-overlay-text", typeof(HideOverlayTextCommand));
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
            RemainingTime = GameSettings.CountdownMaximum;
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

            // Stage
            if (sessionNode.Attributes[nameof(Stage)]?.Value is string stage)
                this.Stage = XmlConvert.ToInt32(stage);

            // Player
            if (sessionNode.Attributes[nameof(Player)]?.Value is string player)
                Player = GetEntity<Actor>(player);

            // Player position
            if (sessionNode.Attributes[nameof(playerPosition)]?.Value is string playerPositionValue)
                playerPosition = XmlConverterExtension.ToVector2(playerPositionValue);

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
                if (entity is not GameThing thing)
                    continue;

                if (thing.InstanceKind == InstanceKind.Static)
                {
                    staticThings.Add(thing);
                    staticThingsDict.Add(thing.StaticName, thing);
                }
            }
        }

        // OnContinuousUpdate
        protected override void OnContinuousUpdate(GameTime gameTime)
        {
            base.OnContinuousUpdate(gameTime);

            if (GameplayMode == GameplayMode.Survival && RemainingTime >= 0)
                RemainingTime -= gameTime.ElapsedGameTime.Milliseconds;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

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
        }

        // OnWrite
        protected override void OnWrite(XmlWriter output)
        {
            // GameplayMode
            output.WriteAttributeString(nameof(GameplayMode), XmlConvert.ToString((int)GameplayMode));

            // Stage
            output.WriteAttributeString(nameof(Stage), XmlConvert.ToString(Stage));

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

            // WorldVersion
            output.WriteAttributeString(AttributeName.WorldVersion.ToString(), XmlConvert.ToString(WorldVersion));
        }

        #endregion

        // ChanceSuccess
        [ScriptProperty]
        public bool ChanceSuccess => HUD.ChanceRoll.Success;

        // ClearOverlayTexts
        [ScriptMethod(CodingContext.Any)]
        public void ClearOverlayTexts() => OverlayTexts.Clear();

        // DialogOptionId
        [ScriptProperty]
        public int DialogOptionId { get; set; }

        // Energy
        [ScriptProperty]
        public int Energy
        {
            get => energy;
            set
            {
                if (value != energy )
                {
                    energy = value;
                    if (Room is ProceduralRoom room)
                        energy = Math.Min(energy, room.RequiredEnergy);
                }
            }
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
                return Array.Empty<MetaItem>();
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

        // IsTimeCritical
        public bool IsTimeCritical => RemainingTime <= GameSettings.TimeCritical;

        // LightingSystem
        [ScriptProperty]
        public bool LightingSystem { get; set; } = true;

        // NextStage
        [ScriptMethod]
        public void NextStage()
        {
            Energy = 0;
            Stage++;
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

        // RegisterFriendlyItems
        public void RegisterFriendlyItems(string staticName, params MetaItem[] metaItems)
        {
            friendlyItems[staticName] = metaItems;
        }

        // RemainingTime
        [ScriptProperty]
        public int RemainingTime { get; set; } = int.MaxValue;

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

        // Stage
        [ScriptProperty]
        public int Stage { get; private set; }

        // StaticThings
        public ReadOnlyCollection<GameThing> StaticThings { get; }

        // WorldVersion
        public int WorldVersion { get; set; } = 1;
    }
}