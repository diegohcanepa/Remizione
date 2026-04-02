using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
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
        private readonly EchoScene echoScene;
        private readonly InventoryScene inventoryScene;
        private Vector2? playerPosition;
        private readonly List<GameThing> proceduralThings = [];
        private readonly Dictionary<string, GameThing> proceduralThingsDict = [];
        private readonly RoomEditor? roomEditor;
        private readonly Sprite savingIcon;

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
            this.Inventory = new(this);
            this.Environment = new Environment(this);
            this.LootGenerator = new(this);
            this.HUD = new HUD(this);
            this.InteractionContext = new(this);
            this.InteractionData = new(this);
            this.DeclaredThings = new(proceduralThings);
            this.Random = new Random(Seed);
            this.inventoryScene = new(Inventory);

            ObjectPools = new ObjectPools(this);
            ImpactWordPool = new ObjectPool<ImpactWord>(() => new ImpactWord(), 100);

            BackgroundColor = ColorPalette.BackgroundColor;
            Camera.SmoothSpeed = GameSettings.CameraSmoothSpeed;

            if (EngendroGame.DebugMode)
            {
                TextSprite consoleText = new(Fonts.CommonOutline)
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

            // Saving icon
            this.savingIcon = new Sprite(Atlases.UI.SavingIcon)
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.Area.GetPoint(RectanglePoint.RightTop, -6, 4)
            };

            LocalizationSource = LocalizationSource.Script;
        }

        #endregion

        #region Private members

        // RegisterAotTypes
        private static void RegisterAotTypes()
        {
            AotTypeRegistry.Register(typeof(Actor));
            AotTypeRegistry.Register(typeof(Zabul));
            AotTypeRegistry.Register(typeof(BloodyEye));
            AotTypeRegistry.Register(typeof(BreakableProp));
            AotTypeRegistry.Register(typeof(CloseUpRoom));
            AotTypeRegistry.Register(typeof(Coin));
            AotTypeRegistry.Register(typeof(CreditsRoom));
            AotTypeRegistry.Register(typeof(Dice));
            AotTypeRegistry.Register(typeof(Firecracker));
            AotTypeRegistry.Register(typeof(GameRoom));
            AotTypeRegistry.Register(typeof(GameThing));
            AotTypeRegistry.Register(typeof(HellGoat));
            AotTypeRegistry.Register(typeof(Hub));
            AotTypeRegistry.Register(typeof(Monitor));
            AotTypeRegistry.Register(typeof(EnviousEye));
            AotTypeRegistry.Register(typeof(NumberSix));
            AotTypeRegistry.Register(typeof(Pottery));
            AotTypeRegistry.Register(typeof(Prop));
            AotTypeRegistry.Register(typeof(Rat));
            AotTypeRegistry.Register(typeof(RideCar));
            AotTypeRegistry.Register(typeof(RideDoor));
            AotTypeRegistry.Register(typeof(Sack));
            AotTypeRegistry.Register(typeof(Skeleton));
            AotTypeRegistry.Register(typeof(SpearTrap));
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
            AotTypeRegistry.Register("apply-enemy-combat-intent", typeof(ApplyEnemyCombatIntentCommand));
            AotTypeRegistry.Register("attach-light", typeof(AttachLightCommand));
            AotTypeRegistry.Register("await-approach", typeof(AwaitApproachCommand));
            AotTypeRegistry.Register("await-credits", typeof(AwaitCreditsCommand));
            AotTypeRegistry.Register("await-dialog-block", typeof(AwaitDialogBlockCommand));
            AotTypeRegistry.Register("await-input", typeof(AwaitInputCommand));
            AotTypeRegistry.Register("await-monitor-text", typeof(AwaitMonitorTextCommand));
            AotTypeRegistry.Register("await-npc-attack", typeof(AwaitNPCAttackCommand));
            AotTypeRegistry.Register("await-player-attack", typeof(AwaitPlayerAttackCommand));
            AotTypeRegistry.Register("await-popup", typeof(AwaitPopupCommand));
            AotTypeRegistry.Register("cast-lightning", typeof(CastLightningCommand));
            AotTypeRegistry.Register("create-dialog-block", typeof(CreateDialogBlockCommand));
            AotTypeRegistry.Register("echo", typeof(EchoCommand));
            AotTypeRegistry.Register("ensure-session-scene", typeof(EnsureSessionSceneCommand));
            AotTypeRegistry.Register("exit-session", typeof(ExitSessionCommand));
            AotTypeRegistry.Register("if-can-pickup-loot", typeof(IfCanPickUpLootStatement));
            AotTypeRegistry.Register("if-test-skill", typeof(IfTestSkillStatement));
            AotTypeRegistry.Register("pickup-loot", typeof(PickUpLootCommand));
            AotTypeRegistry.Register("place-item", typeof(PlaceItemCommand));
            AotTypeRegistry.Register("say", typeof(SayCommand));
            AotTypeRegistry.Register("select-walk-area", typeof(SelectWalkAreaCommand));
            AotTypeRegistry.Register("set-light", typeof(SetLightCommand));
            AotTypeRegistry.Register("show-message", typeof(ShowMessageCommand));
            AotTypeRegistry.Register("terminate-dialog-block", typeof(TerminateDialogBlockCommand));
            AotTypeRegistry.Register("use-item", typeof(UseItemCommand));
            AotTypeRegistry.Register("vibrate", typeof(VibrateCommand));
            AotTypeRegistry.Register("x-tween", typeof(XTweenCommand));
            AotTypeRegistry.Register("y-tween", typeof(YTweenCommand));
        }

        // SyncProceduralMusic
        private void SyncProceduralMusic()
        {
            if (Room is ProceduralRoom proceduralRoom)
            {
                var tag = proceduralRoom.RoomNode.Definition.MusicTag;

                if (string.IsNullOrWhiteSpace(tag))
                    tag = MusicTag.Ride;

                AudioManager.Music.PlayTag(tag);
            }
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

            if (HUDVisible && IsCurrentScene)
                HUD.Draw(gameTime);

            // Draw speech bubbles
            SpeechBubble.DrawSpeechBubbles(gameTime);

            if (CurrentRun == null)
            {
                if (savingIcon.Tweens.IsTweening)
                {
                    Game.SpriteBatch.Begin(Game.Camera);
                    savingIcon.Draw(gameTime);
                    Game.SpriteBatch.End();
                }
            }

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
            InteractionContext.Reset();
            MouseCursor.Reset();
            SyncProceduralMusic();
        }

        // OnExitRoom
        protected override void OnExitRoom(Room currentRoom, Room nextRoom)
        {
            HUD.Reset();
            ImpactWordPool.ReturnAll();
            ObjectPools.FloatingTexts.ReturnAll();

            for (var i = currentRoom.Children.Count - 1; i >= 0; i--)
            {
                if (currentRoom.Children[i] is ThrownObject thrownObject)
                    thrownObject.Unparent();
            }
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput()
        {
            if (roomEditor?.HandleInput() == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            else if (HUD.HandleInput() == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            else
                return base.OnHandleInput();
        }

        // OnOutcome
        protected override void OnOutcome(Thing target)
        {
            base.OnOutcome(target);
            InteractionContext.HeldItem = null;
        }

        // OnOutcomeCompleted
        protected override void OnOutcomeCompleted(Thing target)
        {
            if (target is Actor actor && actor.Reaction != null)
            {
                actor.Reaction = null;
            }
            else
            {
                ReactiveActor?.React();
                ReactiveActor = null;
            }
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
                throw new InvalidOperationException("Session node attributes not found.");

            // Player
            if (sessionNode.Attributes[nameof(Player)]?.Value is string player)
                Player = FindEntity<Actor>(player);

            // Player position
            if (sessionNode.Attributes[nameof(playerPosition)]?.Value is string playerPositionValue)
                playerPosition = DataConvert.ToVector2(playerPositionValue);

            // Inventory
            if (sessionNode.Attributes[nameof(Inventory)]?.Value is string inventoryData)
                Inventory.LoadState(inventoryData);

            // RunCount
            if (sessionNode.Attributes[nameof(RunCount)]?.Value is string runCountValue)
                RunCount = XmlConvert.ToInt32(runCountValue);
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
            savingIcon.Tweens.OpacityTween = FloatTween.Create(TweenStyle.QuadraticInOut, 1, .8f, 300, 10);
        }

        // OnScriptLibraryLoaded
        protected override void OnScriptLibraryLoaded()
        {
            // Check overloads
            foreach (var script in ScriptLibrary.AllScripts)
            {
                if (script.ScriptType == ScriptType.Outcome && script.OverloadName.Length > 0)
                {
                    if (ItemDefinition.Definitions.Find(script.OverloadName) == null)
                        throw new InvalidOperationException($"The item definition supplied in [{script.Name}] does not exist.");
                }
            }
        }

        // OnStarted
        protected override void OnStarted()
        {
            foreach (var entity in Entities)
            {
                if (entity is not GameThing thing)
                    continue;

                if (thing is not IThingDefinition)
                    continue;

                if (thing.InstanceKind == EntityInstanceKind.Declared)
                {
                    proceduralThings.Add(thing);
                    proceduralThingsDict.Add(thing.DeclaredName, thing);
                }
            }

            foreach (var actorDef in ActorDefinition.Definitions.All)
            {
                actorDef.AssertScriptDeclaration(this);
            }

            foreach (var propDef in PropDefinition.Definitions.All)
            {
                propDef.AssertScriptDeclaration(this);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            savingIcon.Update(gameTime);

            if (console != null)
            {
                if (console.IsActive && roomEditor != null)
                    roomEditor.IsActive = false;

                console?.Update(gameTime);
            }

            if (HUDVisible)
            {
                HUD.Update(gameTime);

                if (!IsAwaiting)
                {
                    if (Player?.IsDead == true)
                    {
                        Player?.StopMoving();
                        AwaitRoutine(RoutineNames.DeathByHealth);
                    }
                }
            }

            InteractionContext.Refresh();

            if (!IsAwaiting && IsCurrentScene && CurrentRun != null)
            {
                if (InputManager.DefaultPlayer.Mouse.VirtualPosition.Y > 130)
                {
                    Game.SceneManager.Push(inventoryScene);
                    return;
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
                output.WriteAttributeString(nameof(playerPosition), DataConvert.ToString(playerPosition.Value));

            // Inventory
            if (Inventory.SaveState() is string inventoryData)
                output.WriteAttributeString(nameof(Inventory), inventoryData);

            // RunCount
            output.WriteAttributeString(nameof(RunCount), XmlConvert.ToString(RunCount));
        }

        #endregion

        // CurrentRun
        public Run? CurrentRun { get; private set; }

        // BeginRun
        [ScriptMethod]
        public void BeginRun()
        {
            if (CurrentRun != null)
                throw new InvalidOperationException("A run is already in progress.");

            if (Seed == 0)
                Seed = System.Environment.TickCount;

            CurrentRun = new Run(Seed, 6);

            LoadNextCorridor();
        }

        // CompleteRun
        [ScriptMethod]
        public void CompleteRun()
        {
            EndRun();
            RunCount++;
        }

        // DeclaredThings
        public NamedObjectReadOnlyCollection<GameThing> DeclaredThings { get; }

        // DialogOptionId
        [ScriptProperty]
        public int DialogOptionId { get; set; }

        // EndRun
        [ScriptMethod]
        public void EndRun()
        {
            if (CurrentRun == null)
                return;

            CurrentRun = null;

            if (FindEntity<Hub>(nameof(Hub)) is Hub hubRoom)
                hubRoom.Unload();

            HUDVisible = false;
            Inventory.Clear();
            Player?.Reheal();
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

        // Environment
        public Environment Environment { get; }

        // FindDeclaredThing
        public GameThing? FindDeclaredThing(string name)
        {
            return proceduralThingsDict.TryGetValue(name, out var result) ? result : null;
        }

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

        // InteractionData
        public InteractionData InteractionData { get; }

        // Inventory
        public Inventory Inventory { get; }

        // IsConsoleVisible
        public bool IsConsoleVisible => console?.IsActive ?? false;

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

        // LoadNextCorridor
        [ScriptMethod]
        public void LoadNextCorridor()
        {
            if (CurrentRun == null)
                return;

            CleanUpRuntimeEntities();

            if (!CurrentRun.NextCorridor(this))
            {
                CompleteRun();
                return;
            }

            if (CurrentRun.CurrentCorridor == null)
                throw new InvalidOperationException("Corridor generation failed.");

            if (Player != null)
            {
                var rideRoom = CurrentRun.CurrentCorridor.RideRoom;
                Player.Reheal();
                HUDVisible = true;
                rideRoom.Children.Add(Player);
                if (rideRoom.WalkArea != null)
                    Player.Position = rideRoom.WalkArea.Polygon.BoundingRectangleF.Center;
                Camera.Follow(Player, true);
                EnterRoom(rideRoom);
            }
        }

        // LootGenerator
        public LootGenerator LootGenerator { get; }

        // NextRoom
        [ScriptProperty]
        public new GameRoom? NextRoom => (GameRoom?)base.NextRoom;

        // ObjectPools
        public ObjectPools ObjectPools { get; }

        // OutcomeDoor
        [ScriptProperty]
        public RideDoor? OutcomeDoor => OutcomeTarget as RideDoor;

        // OutcomeTarget
        [ScriptProperty]
        public override GameThing? OutcomeTarget => base.OutcomeTarget as GameThing;

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
                    InteractionData.Clear();
                    if (value != null)
                        Camera.Follow(value);
                }
            }
        }

        // PreviousRoom
        [ScriptProperty]
        public new GameRoom? PreviousRoom => (GameRoom?)base.PreviousRoom;

        // Random
        public Random Random { get; private set; }

        // ReactiveActor
        public Actor? ReactiveActor { get; set; }

        // Room
        [ScriptProperty]
        public new GameRoom? Room => (GameRoom?)base.Room;

        // RunCount
        [ScriptProperty]
        public int RunCount { get; private set; }

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
        public void ShowEcho(string text, bool allowTyping, AtlasImage? image = null)
        {
            echoScene.Show(text, allowTyping, image);
            Game.SceneManager.Push(echoScene);
        }
    }
}