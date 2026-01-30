using Adberration;
using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;
using ScaryCastle.Procedural;
using ScaryCastle.Scripting;
using System;
using System.Collections.Generic;
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
        private int playerHPBeforeOutcome;
        private Vector2? playerPosition;
        private readonly RoomEditor? roomEditor;

        #endregion

        #region Static constructor

        // Static constructor
        static GameSession()
        {
            RegisterAotTypes();
        }

        #endregion

        #region Constructor

        // Constructor
        public GameSession(ScaryCastleGame game, int slotNumber)
            : base(game, new ScaryCastlePersistenceModel(), ContentManagerExtension.EncodePath(game.Content, ContentFolder.System, "ScriptLibrary.esl"), slotNumber)
        {
            this.Game = game;
            this.Deck = new Deck(this);
            this.Inventory = new(this);
            this.Environment = new Environment(this);
            this.LootGenerator = new(this);
            this.HUD = new HUD(this);
            this.InteractionContext = new(this);
            this.DeclaredThings = new(declaredThings);
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
        }

        #endregion

        #region Private members

        // RegisterAotTypes
        private static void RegisterAotTypes()
        {
            AotTypeRegistry.Register(typeof(Actor));
            AotTypeRegistry.Register(typeof(Arena));
            AotTypeRegistry.Register(typeof(Zabul));
            AotTypeRegistry.Register(typeof(BloodyEye));
            AotTypeRegistry.Register(typeof(BreakableProp));
            AotTypeRegistry.Register(typeof(CloseUpRoom));
            AotTypeRegistry.Register(typeof(Coin));
            AotTypeRegistry.Register(typeof(CreditsRoom));
            AotTypeRegistry.Register(typeof(Card));
            AotTypeRegistry.Register(typeof(Dice));
            AotTypeRegistry.Register(typeof(GameRoom));
            AotTypeRegistry.Register(typeof(HellGoat));
            AotTypeRegistry.Register(typeof(Hub));
            AotTypeRegistry.Register(typeof(Monitor));
            AotTypeRegistry.Register(typeof(EnviousEye));
            AotTypeRegistry.Register(typeof(NumberSix));
            AotTypeRegistry.Register(typeof(PostClock));
            AotTypeRegistry.Register(typeof(Pottery));
            AotTypeRegistry.Register(typeof(Prop));
            AotTypeRegistry.Register(typeof(RideCar));
            AotTypeRegistry.Register(typeof(RideDoor));
            AotTypeRegistry.Register(typeof(Sack));
            AotTypeRegistry.Register(typeof(SpearTrap));
            AotTypeRegistry.Register(typeof(Tombstone));
            AotTypeRegistry.Register(typeof(Torch));
            AotTypeRegistry.Register(typeof(Trunk));
            AotTypeRegistry.Register(typeof(WaterPuddle));

            AotTypeRegistry.Register("add-dialog-option", typeof(AddDialogOptionCommand));
            AotTypeRegistry.Register("add-hole", typeof(AddHoleCommand));
            AotTypeRegistry.Register("add-item", typeof(AddItemCommand));
            AotTypeRegistry.Register("add-light", typeof(AddLightCommand));
            AotTypeRegistry.Register("add-trigger-area", typeof(AddTriggerAreaCommand));
            AotTypeRegistry.Register("add-walk-area", typeof(AddWalkAreaCommand));
            AotTypeRegistry.Register("animate-actor", typeof(AnimateActorCommand));
            AotTypeRegistry.Register("attach-light", typeof(AttachLightCommand));
            AotTypeRegistry.Register("await-credits", typeof(AwaitCreditsCommand));
            AotTypeRegistry.Register("await-dialog-block", typeof(AwaitDialogBlockCommand));
            AotTypeRegistry.Register("await-input", typeof(AwaitInputCommand));
            AotTypeRegistry.Register("await-monitor-text", typeof(AwaitMonitorTextCommand));
            AotTypeRegistry.Register("await-player-approach", typeof(AwaitPlayerApproachCommand));
            AotTypeRegistry.Register("await-popup", typeof(AwaitPopupCommand));
            AotTypeRegistry.Register("create-dialog-block", typeof(CreateDialogBlockCommand));
            AotTypeRegistry.Register("echo", typeof(EchoCommand));
            AotTypeRegistry.Register("empty-pilgrim-sack", typeof(EmptyPilgrimSackCommand));
            AotTypeRegistry.Register("ensure-session-scene", typeof(EnsureSessionSceneCommand));
            AotTypeRegistry.Register("exit-session", typeof(ExitSessionCommand));
            AotTypeRegistry.Register("if-can-pickup-loot", typeof(IfCanPickUpLootStatement));
            AotTypeRegistry.Register("if-test-skill", typeof(IfTestSkillStatement));
            AotTypeRegistry.Register("pickup-loot", typeof(PickUpLootCommand));
            AotTypeRegistry.Register("say", typeof(SayCommand));
            AotTypeRegistry.Register("select-walk-area", typeof(SelectWalkAreaCommand));
            AotTypeRegistry.Register("set-light", typeof(SetLightCommand));
            AotTypeRegistry.Register("show-log-message", typeof(ShowLogMessageCommand));
            AotTypeRegistry.Register("show-message", typeof(ShowMessageCommand));
            AotTypeRegistry.Register("take-damage", typeof(TakeDamageCommand));
            AotTypeRegistry.Register("terminate-dialog-block", typeof(TerminateDialogBlockCommand));
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

        // OnOutcome
        protected override void OnOutcome(Thing target)
        {
            playerHPBeforeOutcome = Player?.HP ?? 0;
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            if (HUDVisible && IsCurrentScene && !IsInArena)
                HUD.Draw(gameTime);

            // Draw speech bubbles
            SpeechBubble.DrawSpeechBubbles(gameTime);

            /*
            if (IsPaused)
            {
                Game.SpriteBatch.Begin(Game.Camera);
                Game.Shapes.DrawRectangle(Screen.Area, ColorPalette.SceneShade);
                Game.SpriteBatch.End();
            }
            */

            console?.Draw(gameTime);
            roomEditor?.Draw(gameTime);
        }

        // OnEnterRoom
        protected override void OnEnterRoom(Room room)
        {
            IsInArena = room is Arena;
            InteractionContext.Reset();
            MouseCursor.Reset();

            var width = room.Width == 0 ? room.CustomWidth : room.Width;
            var height = room.Height == 0 ? room.CustomHeight : room.Height;
            Camera.Setup(width, height, room.ScrollLock, room.Zoom);

            // Follow player
            if (Player != null && Player.IsInCurrentRoom)
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

            // FloorIndex
            if (sessionNode.Attributes[nameof(FloorIndex)]?.Value is string floorIndex)
                FloorIndex = XmlConvert.ToInt32(floorIndex);

            // Player
            if (sessionNode.Attributes[nameof(Player)]?.Value is string player)
                Player = FindEntity<Actor>(player);

            // Player position
            if (sessionNode.Attributes[nameof(playerPosition)]?.Value is string playerPositionValue)
                playerPosition = DataConvert.ToVector2(playerPositionValue);

            // Coins
            if (sessionNode.Attributes[nameof(Coins)]?.Value is string coins)
                this.Coins = XmlConvert.ToInt32(coins);

            // Deck
            if (sessionNode.Attributes[nameof(Deck)]?.Value is string deckData)
                Inventory.LoadState(deckData);

            // Inventory
            if (sessionNode.Attributes[nameof(Inventory)]?.Value is string inventoryData)
                Inventory.LoadState(inventoryData);
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
            // Check overloads
            foreach (var script in ScriptLibrary.AllScripts)
            {
                if (script.ScriptType == ScriptType.Outcome && script.OverloadName.Length > 0)
                {
                    if (ItemDefinition.Find(script.OverloadName) == null)
                        throw new InvalidOperationException($"The item definition supplied in [{script.Name}] does not exist.");
                }
            }
        }

        // OnStarted
        protected override void OnStarted()
        {
            EntityDefinition.ValidateIntegrity(this);

            foreach (var entity in Entities)
            {
                if (entity is not GameThing thing)
                    continue;

                if (thing.InstanceKind == EntityInstanceKind.Declared)
                {
                    declaredThings.Add(thing);
                    declaredThingsDict.Add(thing.DeclaredName, thing);
                }
            }

            if (FloorIndex > 0)
                BeginRun();
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

            if (HUDVisible && !IsInArena)
            {
                HUD.Update(gameTime);

                if (!IsAwaiting)
                {
                    if (Player?.IsDead == true)
                        AwaitRoutine(RoutineNames.GameOver);
                }
            }

            if (!IsInArena)
                InteractionContext.Update();
        }

        // OnWrite
        protected override void OnWrite(XmlWriter output)
        {
            output.WriteAttributeString(nameof(FloorIndex), XmlConvert.ToString(FloorIndex));

            // Player
            if (Player != null)
                output.WriteAttributeString(nameof(Player), Player.Name);

            // PlayerPosition
            if (playerPosition.HasValue)
                output.WriteAttributeString(nameof(playerPosition), DataConvert.ToString(playerPosition.Value));

            // Coins
            output.WriteAttributeString(nameof(Coins), XmlConvert.ToString(Coins));

            // Deck
            if (Deck.SaveState() is string deckData)
                output.WriteAttributeString(nameof(Deck), deckData);

            // Inventory
            if (Inventory.SaveState() is string inventoryData)
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

            RunManager.Generate(this, Tags.EmptyList, FloorIndex);

            if (Player != null && RunManager.Rooms[0].RideRoom is RideRoom rideRoom)
            {
                Player.Reheal();
                HUDVisible = true;
                rideRoom.Children.Add(Player);
                if (rideRoom.WalkArea != null)
                    Player.Position = rideRoom.WalkArea.Polygon.BoundingRectangleF.Center;
                Camera.FollowTarget(Player, true);
                EnterRoom(rideRoom);
            }
        }

        // Coins
        [ScriptProperty]
        public int Coins { get; set; }

        // CompleteRun
        [ScriptMethod]
        public void CompleteRun()
        {
            EndRun();
        }

        // Deck
        public Deck Deck { get; }

        // DeclaredThings
        public NamedObjectReadOnlyCollection<GameThing> DeclaredThings { get; }

        // DialogOptionId
        [ScriptProperty]
        public int DialogOptionId { get; set; }

        // EndRun
        [ScriptMethod]
        public void EndRun()
        {
            if (!RunManager.HasContent)
                return;

            if (FindEntity<Hub>(nameof(Hub)) is Hub hubRoom)
                hubRoom.Unload();

            HUDVisible = false;
            Inventory.Clear();
            Coins = 0;
            RunManager.Clear();
            CleanUpRuntimeEntities();
            Seed = 0;

            // 1. Force an immediate collection of all generations (0, 1, and 2).
            // 'Forced' tells the GC to ignore its internal heuristics and run immediately.
            // 'true' makes the call blocking (execution halts until the GC finishes).
            GC.Collect(2, GCCollectionMode.Forced, true);

            // 2. Wait for objects with finalizers (destructors) to finish their cleanup logic.
            GC.WaitForPendingFinalizers();

            // 3. Collect again.
            // This is necessary because objects finalized in step 2 are now officially
            // marked as "garbage" and can finally be released from memory in this pass.
            GC.Collect(2, GCCollectionMode.Forced, true);
        }

        // Enemy
        [ScriptProperty]
        public Actor? Enemy { get; set; }

        // Environment
        public Environment Environment { get; }

        // FindDeclaredThing
        public GameThing? FindDeclaredThing(string name)
        {
            return declaredThingsDict.TryGetValue(name, out var result) ? result : null;
        }

        // FloorIndex
        [ScriptProperty]
        public int FloorIndex { get; set; } = 0;

        // Game
        public new ScaryCastleGame Game { get; }

        // HUD
        public HUD HUD { get; }

        // HUDVisible
        [ScriptProperty]
        public bool HUDVisible { get; set; }

        // ImpactWordPool
        public ObjectPool<ImpactWord> ImpactWordPool { get; }

        // InteractionContext
        public InteractionContext InteractionContext { get; }

        // Inventory
        public Inventory Inventory { get; }

        // IsConsoleVisible
        public bool IsConsoleVisible => console?.IsActive ?? false;

        // IsInArena
        public bool IsInArena { get; private set; }

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

        // LightingSystem
        [ScriptProperty]
        public bool LightingSystem { get; set; } = true;

        // LootGenerator
        public LootGenerator LootGenerator { get; }

        // NextRoom
        [ScriptProperty]
        public new GameRoom? NextRoom => (GameRoom?)base.NextRoom;

        // ObjectPools
        public ObjectPools ObjectPools { get; }

        // OutcomeProp
        [ScriptProperty]
        public Prop? OutcomeProp => OutcomeTarget as Prop;

        // OutcomeDoor
        [ScriptProperty]
        public RideDoor? OutcomeDoor => OutcomeTarget as RideDoor;

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

        // PlayerTookDamage
        [ScriptProperty]
        public bool PlayerTookDamage => Player != null && Player.HP < playerHPBeforeOutcome;

        // PreviousRoom
        [ScriptProperty]
        public new GameRoom? PreviousRoom => (GameRoom?)base.PreviousRoom;

        // Random
        public Random Random { get; private set; }

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
        public void ShowEcho(string text, AtlasImage? image = null)
        {
            echoScene.Show(text, image);
            Game.SceneManager.Push(echoScene);
        }
    }
}