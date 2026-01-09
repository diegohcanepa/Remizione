using Adberration;
using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;
using ScaryCastle.Procedural;
using ScaryCastle.Scripting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace ScaryCastle
{
    /// <summary>
    /// GameSession
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public sealed partial class GameSession : Session
    {
        #region Private fields

        private readonly ScriptConsole? console;
        private readonly List<GameThing> declaredThings = [];
        private readonly Dictionary<string, GameThing> declaredThingsDict = [];
        private readonly EchoScene echoScene;
        private readonly Dictionary<string, MetaItem[]> friendlyItems = [];
        private readonly UIInteractPrompt interactPrompt;
        private readonly InventoryScene inventoryScene;
        private Vector2? playerPosition;
        private readonly RoomEditor? roomEditor;
        private readonly UseKeyItemScene useKeyItemScene;

        #endregion

        // Static constructor
        static GameSession()
        {
            RegisterAotTypes();
        }

        #region Constructor

        // Constructor
        public GameSession(ScaryCastleGame game, int slotNumber)
            : base(game, new ScaryCastlePersistenceModel(), ContentManagerExtension.EncodePath(game.Content, ContentFolder.System, "ScriptLibrary.esl"), slotNumber)
        {
            this.Game = game;
            this.Inventory = new(this);
            this.Environment = new Environment(this);
            this.HUD = new HUD(this);
            this.DeclaredThings = new(declaredThings);
            this.IsMouseVisible = false;
            this.Random = new Random(Seed);

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

                console = new(this, InputBindings.Console, consoleText, new RectangleF(0, 240, 480, 30))
                {
                    TextErrorColor = ColorPalette.Text.Terra
                };

                console.CommandList.Add("add-item Coin");
                console.CommandList.Add("add-item MasterLockpick");

                roomEditor = new RoomEditor(this);
            }

            this.echoScene = new(Game);

            LocalizationSource = LocalizationSource.Script;

            this.inventoryScene = new InventoryScene(this);
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

            if (FindEntity<Hub>(nameof(Hub)) is Hub hubRoom)
                hubRoom.Unload();

            IsHUDVisible = false;
            Inventory.Clear();
            Tickets = 0;
            Player?.Reheal();
            RunManager.Clear();
            CleanUpRuntimeEntities();
            Seed = 0;
            Save();
        }

        // RegisterAotTypes
        private static void RegisterAotTypes()
        {
            AotTypeRegistry.Register(typeof(Actor));
            AotTypeRegistry.Register(typeof(ArenaRoom));
            AotTypeRegistry.Register(typeof(Zabul));
            AotTypeRegistry.Register(typeof(BloodyEye));
            AotTypeRegistry.Register(typeof(BreakableProp));
            AotTypeRegistry.Register(typeof(CloseUpRoom));
            AotTypeRegistry.Register(typeof(CreditsRoom));
            AotTypeRegistry.Register(typeof(VendingMachine));
            AotTypeRegistry.Register(typeof(GameRoom));
            AotTypeRegistry.Register(typeof(HellGoat));
            AotTypeRegistry.Register(typeof(Hub));
            AotTypeRegistry.Register(typeof(Monitor));
            AotTypeRegistry.Register(typeof(EnviousEye));
            AotTypeRegistry.Register(typeof(NumberSix));
            AotTypeRegistry.Register(typeof(Pickup));
            AotTypeRegistry.Register(typeof(PostClock));
            AotTypeRegistry.Register(typeof(Pottery));
            AotTypeRegistry.Register(typeof(Prop));
            AotTypeRegistry.Register(typeof(RideCar));
            AotTypeRegistry.Register(typeof(RideDoor));
            AotTypeRegistry.Register(typeof(SaintPeregrine));
            AotTypeRegistry.Register(typeof(SpearTrap));
            AotTypeRegistry.Register(typeof(Tombstone));
            AotTypeRegistry.Register(typeof(Trunk));
            AotTypeRegistry.Register(typeof(WaterPuddle));

            AotTypeRegistry.Register("add-dialog-option", typeof(AddDialogOptionCommand));
            AotTypeRegistry.Register("add-hole", typeof(AddHoleCommand));
            AotTypeRegistry.Register("add-item", typeof(AddItemCommand));
            AotTypeRegistry.Register("add-light", typeof(AddLightCommand));
            AotTypeRegistry.Register("add-resistance", typeof(AddResistanceCommand));
            AotTypeRegistry.Register("add-trigger-area", typeof(AddTriggerAreaCommand));
            AotTypeRegistry.Register("add-walk-area", typeof(AddWalkAreaCommand));
            AotTypeRegistry.Register("animate-actor", typeof(AnimateActorCommand));
            AotTypeRegistry.Register("attach-light", typeof(AttachLightCommand));
            AotTypeRegistry.Register("await-credits", typeof(AwaitCreditsCommand));
            AotTypeRegistry.Register("await-dialog-block", typeof(AwaitDialogBlockCommand));
            AotTypeRegistry.Register("await-player-approach", typeof(AwaitPlayerApproachCommand));
            AotTypeRegistry.Register("await-popup", typeof(AwaitPopupCommand));
            AotTypeRegistry.Register("begin-resistance-table", typeof(BeginResistanceTableCommand));
            AotTypeRegistry.Register("create-dialog-block", typeof(CreateDialogBlockCommand));
            AotTypeRegistry.Register("echo", typeof(EchoCommand));
            AotTypeRegistry.Register("empty-pilgrim-sack", typeof(EmptyPilgrimSackCommand));
            AotTypeRegistry.Register("end-resistance-table", typeof(EndResistanceTableCommand));
            AotTypeRegistry.Register("ensure-session-scene", typeof(EnsureSessionSceneCommand));
            AotTypeRegistry.Register("exit-session", typeof(ExitSessionCommand));
            AotTypeRegistry.Register("say", typeof(SayCommand));
            AotTypeRegistry.Register("select-walk-area", typeof(SelectWalkAreaCommand));
            AotTypeRegistry.Register("set-light", typeof(SetLightCommand));
            AotTypeRegistry.Register("show-log-message", typeof(ShowLogMessageCommand));
            AotTypeRegistry.Register("show-message", typeof(ShowMessageCommand));
            AotTypeRegistry.Register("terminate-dialog-block", typeof(TerminateDialogBlockCommand));
            AotTypeRegistry.Register("test-skill-chance", typeof(TestSkillChanceCommand));
            AotTypeRegistry.Register("use-key-item", typeof(UseKeyItemCommand));
            AotTypeRegistry.Register("use-item", typeof(UseItemCommand));
            AotTypeRegistry.Register("vibrate", typeof(VibrateCommand));
            AotTypeRegistry.Register("x-tween", typeof(XTweenCommand));
            AotTypeRegistry.Register("y-tween", typeof(YTweenCommand));
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

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            if (GameplayMode == GameplayMode.Adventure)
                HUD.Draw(gameTime);
            else if (IsHUDVisible && IsCurrentScene)
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
            GameplayMode = room is ProceduralRoom or Hub ? GameplayMode.Action : GameplayMode.Adventure;

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
        protected override void OnOutcomeCompleted(Thing target)
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

            // Player
            if (sessionNode.Attributes[nameof(Player)]?.Value is string player)
                Player = FindEntity<Actor>(player);

            // Player position
            if (sessionNode.Attributes[nameof(playerPosition)]?.Value is string playerPositionValue)
                playerPosition = DataConverter.ToVector2(playerPositionValue);

            // CompletedRuns
            if (sessionNode.Attributes[nameof(CompletedRuns)]?.Value is string completedRuns)
                this.CompletedRuns = XmlConvert.ToInt32(completedRuns);

            // FailedRuns
            if (sessionNode.Attributes[nameof(FailedRuns)]?.Value is string failedRuns)
                this.FailedRuns = XmlConvert.ToInt32(failedRuns);

            // Tickets
            if (sessionNode.Attributes[nameof(Tickets)]?.Value is string tickets)
                this.Tickets = XmlConvert.ToInt32(tickets);

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

        // OnStarted
        protected override void OnStarted()
        {
            Config.ValidateConfigurations(this);

            var keyItems = MetaItem.GetItems(ItemCategory.KeyItem);

            var metaItems = new List<MetaItem>();
            foreach (var entity in Entities)
            {
                if (entity is not GameThing thing)
                    continue;

                if (thing.InstanceKind == InstanceKind.Declared)
                {
                    declaredThings.Add(thing);
                    declaredThingsDict.Add(thing.DeclaredName, thing);

                    // Collect friendly items
                    metaItems.Clear();
                    for (var i = 0; i < keyItems.Count; i++)
                    {
                        var routineName = $"{thing.DeclaredName}-With-{keyItems[i].Name}";
                        if (ScriptLibrary.FindRoutine(routineName) != null)
                            metaItems.Add(keyItems[i]);
                    }

                    if (metaItems.Count > 0)
                        this.friendlyItems[thing.DeclaredName] = [.. metaItems];
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
            if (GameplayMode == GameplayMode.Action)
            {
                if (!IsAwaiting)
                {
                    if (Player?.IsDead == true)
                    {
                        FailedRuns++;
                        AwaitRoutine(RoutineNames.GameOver);
                    }
                }
            }
        }

        // OnWrite
        protected override void OnWrite(XmlWriter output)
        {
            // Player
            if (Player != null)
                output.WriteAttributeString(nameof(Player), Player.Name);

            // PlayerPosition
            if (playerPosition.HasValue)
                output.WriteAttributeString(nameof(playerPosition), DataConverter.ToString(playerPosition.Value));

            // CompletedRuns
            output.WriteAttributeString(nameof(CompletedRuns), XmlConvert.ToString(CompletedRuns));

            // FailedRuns
            output.WriteAttributeString(nameof(FailedRuns), XmlConvert.ToString(FailedRuns));

            // Tickets
            output.WriteAttributeString(nameof(Tickets), XmlConvert.ToString(Tickets));

            // Inventory
            if (Inventory.GetSerializationData() is string inventoryData)
                output.WriteAttributeString(nameof(Inventory), inventoryData);
        }

        #endregion

        [ScriptMethod]
        public void BeginRun()
        {
            if (RunManager.HasContent)
                throw new InvalidOperationException("A run is already in progress.");

            if (Seed == 0)
                Seed = System.Environment.TickCount;

            RunManager.Generate(this, Tags.EmptyList, 12);

            Player?.Reheal();
        }

        // CancelRun
        [ScriptMethod]
        public void CancelRun()
        {
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

        // DeclaredThings
        public NamedObjectReadOnlyCollection<GameThing> DeclaredThings { get; }

        // DialogOptionId
        [ScriptProperty]
        public int DialogOptionId { get; set; }

        // Environment
        public Environment Environment { get; }

        // FailedRuns
        [ScriptProperty]
        public int FailedRuns { get; set; }

        // FindDeclaredThing
        public GameThing? FindDeclaredThing(string name)
        {
            return declaredThingsDict.TryGetValue(name, out var result) ? result : null;
        }

        // Game
        public new ScaryCastleGame Game { get; }

        // GameplayMode
        [ScriptProperty]
        public GameplayMode GameplayMode { get; private set; }

        // GetFriendlyItems
        public MetaItem[] GetFriendlyItems(string declaredName)
        {
            return friendlyItems.TryGetValue(declaredName, out var items) ? items : [];
        }

        // HasFriendlyItems
        public bool HasFriendlyItems(string declaredName)
        {
            return friendlyItems.ContainsKey(declaredName);
        }

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

        // NextRoom
        [ScriptProperty]
        public new GameRoom? NextRoom => (GameRoom?)base.NextRoom;

        // ObjectPools
        public ObjectPools ObjectPools { get; }

        // Pickup
        [ScriptProperty]
        public Pickup? Pickup => OutcomeTarget as Pickup;

        // Player
        [ScriptProperty]
        public Actor? Player
        {
            get;
            set
            {
                if (value != field)
                {
                    field?.StopMoving();
                    field = value;
                    HUD.Reset();
                    if (value != null)
                        Camera.FollowTarget(value);
                }
            }
        }

        // PreviousRoom
        [ScriptProperty]
        public new GameRoom? PreviousRoom => (GameRoom?)base.PreviousRoom;

        // Random
        public Random Random { get; private set; }

        // RideDoor
        [ScriptProperty]
        public RideDoor? RideDoor => OutcomeTarget as RideDoor;

        // Room
        [ScriptProperty]
        public new GameRoom? Room => (GameRoom?)base.Room;

        // Seed
        [ScriptProperty]
        public int Seed
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Random = new Random(field);
                }
            }
        }

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
            inventoryScene.SceneController.Push();
        }

        // Tickets
        [ScriptProperty]
        public int Tickets { get; set; }

        // TotalRuns
        [ScriptProperty]
        public int TotalRuns => CompletedRuns + FailedRuns;
    }
}