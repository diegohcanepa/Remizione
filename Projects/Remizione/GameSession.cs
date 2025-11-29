using Adberration;
using Adberration.Scripting;
using Adberration.Scripting.Core;
using Engendro;
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
        private readonly UIInteractPrompt interactPrompt;
        private readonly InventoryScene inventoryScene;
        private Vector2? playerPosition;
        private readonly RoomEditor? roomEditor;
        private readonly List<GameThing> staticThings = [];
        private readonly Dictionary<string, GameThing> staticThingsDict = [];
        private readonly UseKeyItemScene useKeyItemScene;

        #endregion

        #region Constructor

        // Constructor
        public GameSession(RemizioneGame game, int slotNumber)
            : base(game, new RemizionePersistenceModel(), ContentHelper.EncodePath(game.Content, ContentFolder.System, "ScriptLibrary.esl"), slotNumber)
        {
            this.Game = game;
            this.Inventory = new(this);
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
                    MaximumWidth = Screen.NativeWidth - 20,
                    PivotOrigin = RectanglePoint.LeftBottom,
                    Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom),
                    Scale = ScaleInfo.Text.VeryLarge,
                };

                console = new ScriptConsole(this, InputBindings.Console, consoleText, new RectangleF(0, 240, 480, 30))
                {
                    TextErrorColor = ColorPalette.Text.Terra
                };

                console.CommandList.Add("add-item Coin");
                console.CommandList.Add("add-item MasterLockpick");

                roomEditor = new RoomEditor(this);
            }

            this.echoScene = new(Game);

            LocalizationSource = LocalizationSource.Script;

            this.MetaItemPool = new();
            this.inventoryScene = new InventoryScene(Inventory);
            this.useKeyItemScene = new UseKeyItemScene(Inventory);

            // Prompt
            this.interactPrompt = new(this);
        }

        #endregion

        #region Private members

        // EndRun
        private void EndRun()
        {
            if (!RunManager.HasContent)
                return;

            if (GetEntity<Hub>(nameof(Hub)) is Hub hubRoom)
                hubRoom.Unload();

            IsHUDVisible = false;
            Inventory.Clear();
            GameplayMode = GameplayMode.Adventure;
            Player?.Reheal();
            RunManager.Clear();
            CleanUpRuntimeEntities();
            Seed = 0;
            Save();
        }

        #endregion

        #region Protected members

        // CanHandleRoomInput
        protected override bool CanHandleRoomInput
        {
            get
            {
                if (console?.IsActive == true)
                    return false;

                if (roomEditor?.IsActive == true)
                    return false;

                return base.CanHandleRoomInput;
            }
        }

        // ExtendScriptRegistry
        protected override void ExtendScriptRegistry(ScriptRegistry scriptRegistry)
        {
            scriptRegistry.RegisterEntity(typeof(Actor));
            scriptRegistry.RegisterEntity(typeof(ArenaRoom));
            scriptRegistry.RegisterEntity(typeof(Zabul));
            scriptRegistry.RegisterEntity(typeof(BloodyEye));
            scriptRegistry.RegisterEntity(typeof(BreakableProp));
            scriptRegistry.RegisterEntity(typeof(CloseUpRoom));
            scriptRegistry.RegisterEntity(typeof(DepositMachine));
            scriptRegistry.RegisterEntity(typeof(CreditsRoom));
            scriptRegistry.RegisterEntity(typeof(GameRoom));
            scriptRegistry.RegisterEntity(typeof(HellGoat));
            scriptRegistry.RegisterEntity(typeof(Hub));
            scriptRegistry.RegisterEntity(typeof(Monitor));
            scriptRegistry.RegisterEntity(typeof(NosyHemorrhoid));
            scriptRegistry.RegisterEntity(typeof(NumberSix));
            scriptRegistry.RegisterEntity(typeof(PostClock));
            scriptRegistry.RegisterEntity(typeof(Pottery));
            scriptRegistry.RegisterEntity(typeof(Prop));
            scriptRegistry.RegisterEntity(typeof(RideCar));
            scriptRegistry.RegisterEntity(typeof(RideDoor));
            scriptRegistry.RegisterEntity(typeof(SaintPeregrine));
            scriptRegistry.RegisterEntity(typeof(SpearTrap));
            scriptRegistry.RegisterEntity(typeof(Tombstone));
            scriptRegistry.RegisterEntity(typeof(Trunk));
            scriptRegistry.RegisterEntity(typeof(WaterPuddle));

            scriptRegistry.RegisterStatement("add-dialog-option", typeof(AddDialogOptionCommand));
            scriptRegistry.RegisterStatement("add-hole", typeof(AddHoleCommand), CodingContext.EntityDeclaration);
            scriptRegistry.RegisterStatement("add-item", typeof(AddItemCommand), CodingContext.Any);
            scriptRegistry.RegisterStatement("add-light", typeof(AddLightCommand), CodingContext.EntityDeclaration);
            scriptRegistry.RegisterStatement("add-resistance", typeof(AddResistanceCommand), CodingContext.Initialization);
            scriptRegistry.RegisterStatement("add-trigger-area", typeof(AddTriggerAreaCommand), CodingContext.EntityDeclaration);
            scriptRegistry.RegisterStatement("add-walk-area", typeof(AddWalkAreaCommand), CodingContext.EntityDeclaration);
            scriptRegistry.RegisterStatement("animate-actor", typeof(AnimateActorCommand));
            scriptRegistry.RegisterStatement("attach-light", typeof(AttachLightCommand), CodingContext.EntityDeclaration);
            scriptRegistry.RegisterStatement("await-credits", typeof(AwaitCreditsCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("await-dialog-block", typeof(AwaitDialogBlockCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("await-player-approach", typeof(AwaitPlayerApproachCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("await-popup", typeof(AwaitPopupCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("begin-resistance-table", typeof(BeginResistanceTableCommand), CodingContext.Initialization);
            scriptRegistry.RegisterStatement("create-dialog-block", typeof(CreateDialogBlockCommand));
            scriptRegistry.RegisterStatement("echo", typeof(EchoCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("empty-pilgrim-sack", typeof(EmptyPilgrimSackCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("end-resistance-table", typeof(EndResistanceTableCommand), CodingContext.Initialization);
            scriptRegistry.RegisterStatement("ensure-session-scene", typeof(EnsureSessionSceneCommand));
            scriptRegistry.RegisterStatement("exit-session", typeof(ExitSessionCommand));
            scriptRegistry.RegisterStatement("say", typeof(SayCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("select-walk-area", typeof(SelectWalkAreaCommand));
            scriptRegistry.RegisterStatement("set-light", typeof(SetLightCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("show-log-message", typeof(ShowLogMessageCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("show-message", typeof(ShowMessageCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("terminate-dialog-block", typeof(TerminateDialogBlockCommand));
            scriptRegistry.RegisterStatement("test-skill-chance", typeof(TestSkillChanceCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("use-key-item", typeof(UseKeyItemCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("use-item", typeof(UseItemCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("vibrate", typeof(VibrateCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("x-tween", typeof(XTweenCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("y-tween", typeof(YTweenCommand), CodingContext.Execution);
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            if (GameplayMode == GameplayMode.Action && IsHUDVisible && IsCurrentScene)
                HUD.Draw(gameTime);

            if (!IsAwaiting)
                interactPrompt.Draw(gameTime);

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
            var width = room.Width == 0 ? room.CustomWidth : room.Width;
            var height = room.Height == 0 ? room.CustomHeight : room.Height;
            Camera.Setup(width, height, room.ScrollLock, room.Zoom);

            // Follow player
            if (Player != null && Player.InCurrentRoom)
                Camera.FollowTarget(Player, true);
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
            if (roomEditor?.HandleInput() == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            else if (HUD.HandleInput(gameTime) == HandleInputResult.Handled)
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

            // CompletedRuns
            if (sessionNode.Attributes[nameof(CompletedRuns)]?.Value is string completedRuns)
                this.CompletedRuns = XmlConvert.ToInt32(completedRuns);

            // FailedRuns
            if (sessionNode.Attributes[nameof(FailedRuns)]?.Value is string failedRuns)
                this.FailedRuns = XmlConvert.ToInt32(failedRuns);

            // Tickets
            if (sessionNode.Attributes[nameof(Tickets)]?.Value is string tickets)
                this.Tickets = XmlConvert.ToInt32(tickets);

            // MetaItemPool
            if (sessionNode.Attributes[nameof(MetaItemPool)]?.Value is string metaItemPoolData)
                MetaItemPool.Deserialize(metaItemPoolData);

            // Inventory
            if (sessionNode.Attributes[nameof(Inventory)]?.Value is string inventoryData)
                Inventory.SetSerializationData(inventoryData);
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

        // OnScriptLibraryLoaded
        protected override void OnScriptLibraryLoaded()
        {
            MetaItemPool.InitializeDefaults();
        }

        // OnStart
        protected override void OnStart()
        {
            var keyItems = MetaItem.GetItems(ItemCategory.KeyItem);

            var metaItems = new List<MetaItem>();
            foreach (var entity in Entities)
            {
                if (entity is not GameThing thing)
                    continue;

                if (thing.InstanceKind == InstanceKind.Static)
                {
                    staticThings.Add(thing);
                    staticThingsDict.Add(thing.StaticName, thing);

                    // Collect friendly items
                    metaItems.Clear();
                    for (var i = 0; i < keyItems.Count; i++)
                    {
                        var routineName = $"{thing.StaticName}-With-{keyItems[i].Name}";
                        if (ScriptLibrary.GetRoutine(routineName) != null)
                            metaItems.Add(keyItems[i]);
                    }

                    if (metaItems.Count > 0)
                        this.friendlyItems[thing.StaticName] = metaItems.ToArray();
                }
            }
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

            Environment.Update(gameTime);
            interactPrompt.Update(gameTime);

            if (GameplayMode == GameplayMode.Action && IsHUDVisible)
                HUD.Update(gameTime);

            // Check game over condition
            if (!IsAwaiting)
            {
                if (Player?.IsDead == true)
                {
                    FailedRuns++;
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

            // CompletedRuns
            output.WriteAttributeString(nameof(CompletedRuns), XmlConvert.ToString(CompletedRuns));

            // FailedRuns
            output.WriteAttributeString(nameof(FailedRuns), XmlConvert.ToString(FailedRuns));

            // Tickets
            output.WriteAttributeString(nameof(Tickets), XmlConvert.ToString(Tickets));

            // MetaItemPool
            output.WriteAttributeString(nameof(MetaItemPool), MetaItemPool.Serialize());

            // Inventory
            if (Inventory.GetSerializationData() is string inventoryData)
                output.WriteAttributeString(nameof(Inventory), inventoryData);
        }

        #endregion

        // BeginRun
        [ScriptMethod]
        public void BeginRun()
        {
            if (RunManager.HasContent)
                throw new InvalidOperationException("A run is already in progress.");

            if (Seed == 0)
                Seed = System.Environment.TickCount;

            RunManager.Generate(this);
        }

        // CancelRun
        [ScriptMethod]
        public void CancelRun()
        {
            Tickets = 0;
            FailedRuns++;
            EndRun();
        }

        // ChooseKeyItem
        public bool ChooseKeyItem(string text)
        {
            if (Player == null)
                return false;

            Player.Stand();
            if (OutcomeTarget is Prop prop)
            {
                KeyItemTarget = prop;
                useKeyItemScene.Text = text;
                useKeyItemScene.SceneController.Push();
                return true;
            }

            return false;
        }

        // CompleteRun
        [ScriptMethod]
        public void CompleteRun()
        {
            CompletedRuns++;
            EndRun();
        }

        // CompletedRuns
        [ScriptProperty]
        public int CompletedRuns { get; set; }

        // DialogOptionId
        [ScriptProperty]
        public int DialogOptionId { get; set; }

        // Environment
        public Environment Environment { get; }

        // FailedRuns
        [ScriptProperty]
        public int FailedRuns { get; set; }

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
            return staticThingsDict.TryGetValue(name, out var result) ? result : null;
        }

        // HasFriendlyItems
        public bool HasFriendlyItems(string staticName) => friendlyItems.ContainsKey(staticName);

        // HUD
        public HUD HUD { get; }

        // ImpactWordPool
        public ObjectPool<ImpactWord> ImpactWordPool { get; }

        // Inventory
        public Inventory Inventory { get; }

        // IsConsoleVisible
        public bool IsConsoleVisible => console?.IsActive ?? false;

        // IsHUDVisible
        [ScriptProperty]
        public bool IsHUDVisible { get; set; } = true;

        // KeyItemTarget
        [ScriptProperty]
        public Prop? KeyItemTarget { get; set; }

        // KillEnemies
        [ScriptMethod]
        public void KillEnemies()
        {
            if (Room == null)
                return;

            for (int i = Room.Children.Count - 1; i >= 0; i--)
            {
                if (Room.Children[i] is Actor actor && !actor.IsDead && actor != Player)
                    actor.Die();
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

        // LightingSystem
        [ScriptProperty]
        public bool LightingSystem { get; set; } = true;

        // MetaItemPool
        public MetaItemPool MetaItemPool { get; }

        // NextRoom
        [ScriptProperty]
        public new GameRoom? NextRoom => (GameRoom?)base.NextRoom;

        // ObjectPools
        public ObjectPools ObjectPools { get; }

        // Player
        [ScriptProperty]
        public Actor? Player
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    HUD.Reset();
                }
            }
        }

        // PreviousRoom
        [ScriptProperty]
        public new GameRoom? PreviousRoom => (GameRoom?)base.PreviousRoom;

        // Room
        [ScriptProperty]
        public new GameRoom? Room => (GameRoom?)base.Room;

        // Seed
        [ScriptProperty]
        public int Seed { get; set; }

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

        // ShowInventory
        [ScriptMethod]
        public void ShowInventory()
        {
            if (Player == null)
                return;

            Player.Stand();
            if (Camera.Target == Player)
                Camera.FocusTarget();

            inventoryScene.SceneController.Push();
        }

        // StaticThings
        public NamedObjectReadOnlyCollection<GameThing> StaticThings { get; }

        // Tickets
        public int Tickets { get; set; }

        // TotalRuns
        [ScriptProperty]
        public int TotalRuns => CompletedRuns + FailedRuns;
    }
}