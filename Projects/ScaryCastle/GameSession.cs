using Adberration;
using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;
using ScaryCastle.Props;
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
        private readonly FloatTween chromaticAberrationTween = new();
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
            this.RunModifiers = new(this);
            this.PlayerInventory = new(this);
            this.Environment = new Environment();
            this.LootGenerator = new(this);
            this.PocketItemManager = new(this);
            this.HUD = new HUD(this);
            this.InteractionContext = new(this);
            this.InteractionData = new(this);
            this.DeclaredThings = new(proceduralThings);
            this.inventoryScene = new(PlayerInventory);

            ObjectPools = new ObjectPools(this);
            ComicTextPool = new ObjectPool<ComicText>(() => new ComicText(), 100);

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

                console.CommandList.Add("put PotteryA into $Room #at:120,75");
                console.CommandList.Add("put StinkyRat into $Room");
                console.CommandList.Add("put GoldenTrunk into $Room #at:77,77");
                console.CommandList.Add("add-item Lockpick");

                roomEditor = new RoomEditor(this);
            }

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
            AotTypeRegistry.Register(typeof(BloodyEye));
            AotTypeRegistry.Register(typeof(BluePill));
            AotTypeRegistry.Register(typeof(BronzeKey));
            AotTypeRegistry.Register(typeof(CloseUpRoom));
            AotTypeRegistry.Register(typeof(Coin));
            AotTypeRegistry.Register(typeof(CreditsRoom));
            AotTypeRegistry.Register(typeof(Decoration));
            AotTypeRegistry.Register(typeof(Firecracker));
            AotTypeRegistry.Register(typeof(FlyingSkull));
            AotTypeRegistry.Register(typeof(GameRoom));
            AotTypeRegistry.Register(typeof(GameThing));
            AotTypeRegistry.Register(typeof(GateLever));
            AotTypeRegistry.Register(typeof(GoldenKey));
            AotTypeRegistry.Register(typeof(Goo));
            AotTypeRegistry.Register(typeof(HellGoat));
            AotTypeRegistry.Register(typeof(Monitor));
            AotTypeRegistry.Register(typeof(EnviousEye));
            AotTypeRegistry.Register(typeof(NumberSix));
            AotTypeRegistry.Register(typeof(Penitent));
            AotTypeRegistry.Register(typeof(Pottery));
            AotTypeRegistry.Register(typeof(Prop));
            AotTypeRegistry.Register(typeof(Rat));
            AotTypeRegistry.Register(typeof(RedPill));
            AotTypeRegistry.Register(typeof(Door));
            AotTypeRegistry.Register(typeof(Sack));
            AotTypeRegistry.Register(typeof(Skeleton));
            AotTypeRegistry.Register(typeof(SpearTrap));
            AotTypeRegistry.Register(typeof(StinkyRat));
            AotTypeRegistry.Register(typeof(Torch));
            AotTypeRegistry.Register(typeof(TrapDoor));
            AotTypeRegistry.Register(typeof(Trunk));
            AotTypeRegistry.Register(typeof(WreckingBall));

            AotTypeRegistry.Register("add-dialog-option", typeof(AddDialogOptionCommand));
            AotTypeRegistry.Register("add-hole", typeof(AddHoleCommand), CodingContext.EntityDeclaration);
            AotTypeRegistry.Register("add-item", typeof(AddItemCommand));
            AotTypeRegistry.Register("add-light", typeof(AddLightCommand), CodingContext.EntityDeclaration);
            AotTypeRegistry.Register("add-trigger-area", typeof(AddTriggerAreaCommand), CodingContext.EntityDeclaration);
            AotTypeRegistry.Register("add-walk-area", typeof(AddWalkAreaCommand), CodingContext.EntityDeclaration);
            AotTypeRegistry.Register("animate-actor", typeof(AnimateActorCommand));
            AotTypeRegistry.Register("attach-light", typeof(AttachLightCommand), CodingContext.EntityDeclaration);
            AotTypeRegistry.Register("await-approach", typeof(AwaitApproachCommand));
            AotTypeRegistry.Register("await-boss-intro", typeof(AwaitBossIntroCommand));
            AotTypeRegistry.Register("await-credits", typeof(AwaitCreditsCommand));
            AotTypeRegistry.Register("await-devil-hand", typeof(AwaitDevilHandCommand));
            AotTypeRegistry.Register("await-dialog-block", typeof(AwaitDialogBlockCommand));
            AotTypeRegistry.Register("await-input", typeof(AwaitInputCommand));
            AotTypeRegistry.Register("await-monitor-text", typeof(AwaitMonitorTextCommand));
            AotTypeRegistry.Register("await-npc-turn", typeof(AwaitNPCTurnCommand));
            AotTypeRegistry.Register("await-popup", typeof(AwaitPopupCommand));
            AotTypeRegistry.Register("consume-item", typeof(ConsumeItemCommand));
            AotTypeRegistry.Register("create-dialog-block", typeof(CreateDialogBlockCommand));
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
            AotTypeRegistry.Register("x-tween", typeof(XTweenCommand));
            AotTypeRegistry.Register("y-tween", typeof(YTweenCommand));
        }

        // SyncProceduralMusic
        private void SyncProceduralMusic()
        {
            //AudioManager.Music.Stop();
            //AudioManager.Music.CurrentTag = string.Empty;

            /*
            if (Room is ProceduralRoom proceduralRoom)
            {
                var tag = proceduralRoom.RoomNode.Definition.MusicTag;

                if (string.IsNullOrWhiteSpace(tag))
                    tag = MusicTag.Ride;

                AudioManager.Music.PlayTag(tag);
            }
            */
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

            Game.RenderTargets.Swap();
            Game.SpriteBatch.Begin(effect: ScaryCastleGame.Effects.CRT.Effect);
            Game.SpriteBatch.Draw(Game.RenderTargets.PreviousTarget, Vector2.Zero, Color.White);
            Game.SpriteBatch.End();

            if (IsHUDVisible)
                HUD.Draw(gameTime);

            SpeechText.DrawSpeechTexts(gameTime);
        }

        // OnEnterRoom
        protected override void OnEnterRoom(Room room)
        {
            Game.SceneManager.PopUntil(this);
            InteractionContext.Reset();
            MouseCursor.Reset();
            if (room is not ProceduralRoom)
                chromaticAberrationTween.Stop();
            SyncProceduralMusic();
        }

        // OnExitRoom
        protected override void OnExitRoom(Room currentRoom, Room nextRoom)
        {
            HUD.Reset();
            ComicTextPool.ReturnAll();
            ObjectPools.FlyOffs.ReturnAll();

            for (var i = currentRoom.Children.Count - 1; i >= 0; i--)
            {
                if (currentRoom.Children[i] is ThrownProp thrownObject)
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
                    if (ItemDefinition.Container.Find(script.OverloadName) == null)
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

            foreach (var actorDef in ActorDefinition.Container.All)
            {
                actorDef.AssertScriptDeclaration(this);
            }

            foreach (var propDef in PropDefinition.Container.All)
            {
                propDef.AssertScriptDeclaration(this);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (chromaticAberrationTween.IsRunning)
            {
                chromaticAberrationTween.Update(gameTime);
                ScaryCastleGame.Effects.CRT.ChromaticAberration = chromaticAberrationTween.CurrentValue;
            }

            savingIcon.Update(gameTime);

            if (console != null)
            {
                if (console.IsActive && roomEditor != null)
                    roomEditor.IsActive = false;

                console?.Update(gameTime);
            }

            if (CurrentRun != null)
            {
                RunModifiers.Update(gameTime);

                if (IsHUDVisible)
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

            //if (IsCurrentScene)
            {
                if (Room == null || !Room.ControlMouseCursor)
                    InteractionContext.Refresh();
            }

            /*
            if (Player != null && Player.ActiveThrowable == null)
            {
                if (!IsAwaiting && IsCurrentScene && CurrentRun != null)
                {
                    if (InputManager.DefaultPlayer.Mouse.VirtualPosition.Y > 130)
                    {
                        ShowInventory();
                        return;
                    }
                }
            }
            */
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

            // RunCount
            output.WriteAttributeString(nameof(RunCount), XmlConvert.ToString(RunCount));
        }

        // OnOutcomeCompleted
        protected override void OnOutcomeCompleted(Script script, Thing target)
        {
            base.OnOutcomeCompleted(script, target);
            ActiveNPC?.CombatDecision = null;
            ProcessTurn();
        }

        #endregion

        // ActiveNPC
        [ScriptProperty]
        public Actor? ActiveNPC { get; private set; }

        // BeginRun
        [ScriptMethod]
        public void BeginRun()
        {
            BeginRun(null);
        }

        // BeginRun
        public void BeginRun(int? seed = null)
        {
            if (CurrentRun != null)
                throw new InvalidOperationException("A run is already in progress.");

            // 1. Establecemos el Seed de la run para la TOPOLOGÍA (fijo o por tiempo)
            RunSeed = seed ?? System.Environment.TickCount;

            // 2. RNG Maestro: DETERMINISTA. Usa el RunSeed para que los cuartos sean iguales.
            MasterRunRng = new Random(RunSeed);

            // 3. RNG Volátil: CAOS TOTAL. Sin parámetros, .NET genera una semilla impredecible.
            VolatileRng = new Random(RunSeed);

            CurrentRun = new Run(this, 15);

            PlayerInventory.Capacity = GameSettings.InitialInventoryCapacity;
            if (RunCount == 0)
            {
                PlayerInventory.Add(ItemNames.Apple);
                PlayerInventory.Add(ItemNames.GooBottle);
                PlayerInventory.Add(ItemNames.ServantCross);
            }

            CurrentRun.NextFloor();

            if (Player != null)
            {
                Player.Reheal();
                Player.Recharge();
                var startRoom = CurrentRun.FloorMap[new(0, 0)].Room;
                startRoom.Children.Add(Player);
                if (startRoom.WalkArea != null)
                    Player.Position = startRoom.WalkArea.Polygon.BoundingRectangleF.Center;
                Camera.Follow(Player, true);
                EnterRoom(startRoom);
            }
        }

        // Boss
        [ScriptProperty]
        public Actor? Boss => Bosses.Count > 0 ? Bosses[0] : null;

        // Bosses
        public List<Actor> Bosses { get; } = [];

        // ComicTextPool
        public ObjectPool<ComicText> ComicTextPool { get; }

        // CompleteRun
        [ScriptMethod]
        public void CompleteRun()
        {
            RunCount++;
            EndRun();
        }

        // CurrentRun
        public Run? CurrentRun { get; private set; }

        // DangerousTarget
        [ScriptProperty]
        public GameThing? DangerousTarget { get; set; }

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

            RunModifiers.Clear();
            Bosses.Clear();
            CleanUpRuntimeEntities();
            PocketItemManager.Reset();
            PlayerInventory.Clear();
            PlayerStats.Reset();
            InteractionContext.Reset();
            InteractionContext.HeldItem = null;

            if (Player != null)
            {
                Player.Reheal();
                Player.Recharge();
                Player.MaxEnergy = 5;
                Player.Energy = 5;
            }

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

            Save();
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

        // GoldenKeys
        [ScriptProperty]
        public int GoldenKeys
        {
            get;
            set
            {
                if (value != field)
                    field = Math.Max(0, value);
            }
        }

        // HasPendingTurns
        [ScriptProperty]
        public bool HasPendingTurns
        {
            get
            {
                if (Room is ProceduralRoom)
                {
                    for (var i = 0; i < Room.Children.Count; i++)
                    {
                        if (Room.Children[i] is Actor actor && !actor.IsPlayer && actor.CombatDecision?.Intent != null)
                            return true;
                    }
                }

                return false;
            }
        }

        // HUD
        public HUD HUD { get; }

        // InteractionContext
        public InteractionContext InteractionContext { get; }

        // InteractionData
        public InteractionData InteractionData { get; }

        // IsConsoleVisible
        public bool IsConsoleVisible => console?.IsActive ?? false;

        // IsHUDVisible
        [ScriptProperty]
        public bool IsHUDVisible => Room?.IsProcedural == true;

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

        // MasterRunRng (RNG supremo de la partida entera. Solo se usa para generar pisos.)
        public Random MasterRunRng { get; private set; } = new();

        // NextRoom
        [ScriptProperty]
        public new GameRoom? NextRoom => (GameRoom?)base.NextRoom;

        // ObjectPools
        public ObjectPools ObjectPools { get; }

        // OutcomeDoor
        [ScriptProperty]
        public Door? OutcomeDoor => OutcomeTarget as Door;

        // OutcomeTarget
        [ScriptProperty]
        public override GameThing? OutcomeTarget => base.OutcomeTarget as GameThing;

        // PerformChromaticAberration
        public void PerformChromaticAberration()
        {
            ScaryCastleGame.Effects.CRT.Reset();
            chromaticAberrationTween.Start(TweenStyle.Linear, ScaryCastleGame.Effects.CRT.ChromaticAberration, ScaryCastleGame.Effects.CRT.ChromaticAberration + 0.009f, 2500, 2);
        }

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

        // PlayerInventory
        public ItemContainer PlayerInventory { get; }

        // PlayerStats
        public PlayerStats PlayerStats { get; } = new();

        // PocketItemManager
        public PocketItemManager PocketItemManager { get; }

        // PreviousRoom
        [ScriptProperty]
        public new GameRoom? PreviousRoom => (GameRoom?)base.PreviousRoom;

        // ProcessTurn
        public void ProcessTurn(int turnPenalty = 0)
        {
            if (Room is not ProceduralRoom room)
                return;

            if (turnPenalty != 0)
            {
                for (var i = 0; i < room.Children.Count; i++)
                {
                    if (room.Children[i] is Actor actor && !actor.IsPlayer)
                        actor.RemainingTurns -= Math.Abs(turnPenalty);
                }
            }

            if (Player != null)
            {
                for (var i = 0; i < room.Children.Count; i++)
                {
                    if (room.Children[i] is Actor actor && actor != Player && actor.RemainingTurns > 0)
                    {
                        if (actor.DistanceTo(Player) <= 5)
                            actor.RemainingTurns = 0;
                    }
                }
            }

            for (var i = 0; i < room.Children.Count; i++)
            {
                if (room.Children[i] is Actor actor && !actor.IsPlayer && actor.RemainingTurns == 0)
                {
                    if (actor.BeginTurn() is { } script)
                    {
                        ActiveNPC = actor;
                        BeginOutcome(script, actor);
                        return;
                    }
                }
            }

            for (var i = 0; i < room.Children.Count; i++)
            {
                if (room.Children[i] is Actor actor && !actor.IsPlayer)
                {
                    actor.ProcessTurn();
                }
            }

            ActiveNPC = null;
        }

        // Room
        [ScriptProperty]
        public new GameRoom? Room => (GameRoom?)base.Room;

        // RunCount
        [ScriptProperty]
        public int RunCount { get; set; }

        // RunModifiers
        public RunModifierManager RunModifiers { get; }

        // RunSeed
        [ScriptProperty]
        public int RunSeed { get; private set; }

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

        // ShowDialogMenu
        public void ShowDialogMenu(DialogBlock dialogBlock)
        {
            DialogOptionId = -1;
            var scene = new DialogBlockScene(this, dialogBlock);
            Game.SceneManager.Push(scene);
        }

        // ShowInventory
        public void ShowInventory()
        {
            Game.SceneManager.Push(inventoryScene);
        }

        // VolatileRng (Este es el RNG para gameplay (Drops, IA, combate))
        public Random VolatileRng { get; private set; } = new();
    }
}