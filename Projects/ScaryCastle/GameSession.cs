using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Engendro.Collections;
using Microsoft.Xna.Framework;
using ScaryCastle.Props;
using ScaryCastle.Scripting;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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
        private InventoryScene? inventoryScene;
        private Vector2? playerPosition;
        private FrozenDictionary<string, GameThing>? proceduralCatalog;
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
            : base(game, new ScaryCastlePersistenceModel(), Path.Combine("Content", ContentFolder.System.ToString(), "ScriptLibrary.esl"), slotNumber)
        {
            this.Game = game;
            this.Environment = new Environment();
            this.InteractionContext = new(this);
            this.InteractionData = new(this);

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

                console = new(this, InputBindings.Console, consoleText, new RectangleF(0, 115, 240, 20))
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

        // PreparePlayerForRun
        private void PreparePlayerForRun()
        {
            if (Player == null || CurrentRun == null)
                throw new InvalidOperationException("No player and/or run is available.");

            if (RunIndex == 0)
            {
                CurrentRun.PlayerInventory.Add(ItemNames.Apple);
                CurrentRun.PlayerInventory.Add(ItemNames.GooBottle);
                CurrentRun.PlayerInventory.Add(ItemNames.ServantCross);
                CurrentRun.Traits.Add(TraitType.Luck);
                CurrentRun.Traits.Add(TraitType.Lockpicking);
            }

            CurrentRun.PlayerInventory.Capacity = GameSettings.PlayerDefaults.InventoryCapacity;
            Player.MaxHP = GameSettings.PlayerDefaults.MaxHP;
            Player.MaxEnergy = GameSettings.PlayerDefaults.MaxEnergy;
            Player.MaxStamina = GameSettings.PlayerDefaults.MaxStamina;

            if (RunIndex == 0)
                Player.Energy = 0;
            else
                Player.Recharge();
        }

        // RegisterAotTypes
        private static void RegisterAotTypes()
        {
            AotTypeRegistry.Register(typeof(Actor));
            AotTypeRegistry.Register(typeof(BloodyEye));
            AotTypeRegistry.Register(typeof(Pill));
            AotTypeRegistry.Register(typeof(BronzeKey));
            AotTypeRegistry.Register(typeof(CloseUpRoom));
            AotTypeRegistry.Register(typeof(Coin));
            AotTypeRegistry.Register(typeof(CreditsRoom));
            AotTypeRegistry.Register(typeof(Decoration));
            AotTypeRegistry.Register(typeof(Door));
            AotTypeRegistry.Register(typeof(EnviousEye));
            AotTypeRegistry.Register(typeof(Firecracker));
            AotTypeRegistry.Register(typeof(FlyingSkull));
            AotTypeRegistry.Register(typeof(GameRoom));
            AotTypeRegistry.Register(typeof(GateLever));
            AotTypeRegistry.Register(typeof(GoldenKey));
            AotTypeRegistry.Register(typeof(Goo));
            AotTypeRegistry.Register(typeof(HellGoat));
            AotTypeRegistry.Register(typeof(Monitor));
            AotTypeRegistry.Register(typeof(NumberSix));
            AotTypeRegistry.Register(typeof(Penitent));
            AotTypeRegistry.Register(typeof(Pottery));
            AotTypeRegistry.Register(typeof(Prop));
            AotTypeRegistry.Register(typeof(Rat));
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
            AotTypeRegistry.Register("await-credits", typeof(AwaitCreditsCommand));
            AotTypeRegistry.Register("await-devil-hand", typeof(AwaitDevilHandCommand));
            AotTypeRegistry.Register("await-dialog-block", typeof(AwaitDialogBlockCommand));
            AotTypeRegistry.Register("await-examine-item", typeof(AwaitExamineItemCommand));
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
            if (Room is ProceduralRoom proceduralRoom)
            {
                var tag = proceduralRoom.RoomNode.Definition.MusicTag;

                if (string.IsNullOrWhiteSpace(tag))
                    tag = MusicTag.Castle;

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

            if (CurrentRun != null)
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

            Game.RenderTargets.Swap();
            Game.SpriteBatch.Begin(effect: ScaryCastleGame.Effects.CRT.Effect);
            Game.SpriteBatch.Draw(Game.RenderTargets.PreviousTarget, Vector2.Zero, Color.White);
            Game.SpriteBatch.End();

            if (Player != null && !Player.IsDead)
                RunHUD?.Draw(gameTime);

            SpeechText.DrawSpeechTexts(gameTime);

            roomEditor?.Draw(gameTime);
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
            RunHUD?.Reset();
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

            else if (RunHUD?.HandleInput() == HandleInputResult.Handled)
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
            if (sessionNode.Attributes[nameof(RunIndex)]?.Value is string runCountValue)
                RunIndex = XmlConvert.ToInt32(runCountValue);
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
                    if (GameData.Items.Find(script.OverloadName) == null)
                        throw new InvalidOperationException($"The item definition supplied in [{script.Name}] does not exist.");
                }
            }
        }

        // OnStarted
        protected override void OnStarted()
        {
            Dictionary<string, GameThing> dict = [];

            // Collect all things that has a data-driven definition
            foreach (var entity in Entities)
            {
                if (entity is not GameThing thing)
                    continue;

                if (thing.Definition == null)
                    continue;

                if (thing.InstanceKind == EntityInstanceKind.Declared)
                    dict.Add(thing.DeclaredName, thing);
            }

            proceduralCatalog = dict.ToFrozenDictionary();

            foreach (var actorDef in GameData.Actors)
            {
                actorDef.AssertScriptDeclaration(this);
            }

            foreach (var propDef in GameData.Props)
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
                CurrentRun.Update(gameTime);

                RunHUD?.Update(gameTime);

                if (!IsAwaiting)
                {
                    if (Player?.IsDead == true)
                    {
                        Player?.StopMoving();
                        AwaitRoutine(RoutineNames.DeathByHealth);
                    }
                }
            }

            if (IsCurrentScene)
            {
                if (Room == null || !Room.ControlMouseCursor)
                    InteractionContext.Refresh();
            }

            /*
            if (Player != null && Player.ActiveThrowable == null)
            {
                if (!IsAwaiting && IsCurrentScene && RunInProgress)
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
            output.WriteAttributeString(nameof(RunIndex), XmlConvert.ToString(RunIndex));
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

        // AdvanceToNextFloor
        [ScriptMethod]
        public void AdvanceToNextFloor()
        {
            if (CurrentRun == null)
                return;

            if (CurrentRun.FloorDescriptor != null)
                CleanUpRuntimeEntities();

            if (CurrentRun.TryGenerateNextFloor(out ProceduralRoom? startRoom) && startRoom != null)
            {
                if (Player != null)
                {
                    startRoom.Children.Add(Player);
                    if (startRoom.WalkArea != null)
                        Player.Position = startRoom.WalkArea.Polygon.BoundingRectangleF.Center;

                    Camera.Follow(Player, true);
                    EnterRoom(startRoom);
                }
            }
            else
            {
                CompleteRun();
            }
        }

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

            var runSeed = seed ?? System.Environment.TickCount;

            // 1. Get descriptor
            var runDefinition = GameData.Runs.Get($"Run{RunIndex}");

            // 2. Create run
            CurrentRun = new Run(this, runSeed, runDefinition);

            RunHUD = new(CurrentRun);
            inventoryScene = new(CurrentRun.PlayerInventory);

            PreparePlayerForRun();
            AdvanceToNextFloor();
        }

        // ComicTextPool
        public ObjectPool<ComicText> ComicTextPool { get; }

        // CompleteRun
        [ScriptMethod]
        public void CompleteRun()
        {
            RunIndex++;
            EndRun();
        }

        // CurrentRun
        public Run? CurrentRun { get; private set; }

        // DangerousTarget
        [ScriptProperty]
        public GameThing? DangerousTarget { get; set; }

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
            inventoryScene = null;
            CleanUpRuntimeEntities();

            InteractionContext.Reset();
            InteractionContext.HeldItem = null;

            if (Player != null)
            {
                Player.StatusManager.Clear();
                Player.Reheal();
            }

            Save();
        }

        // Environment
        public Environment Environment { get; }

        // FindProceduralThing
        public GameThing? FindProceduralThing(string name)
        {
            if (proceduralCatalog == null)
                return null;

            return proceduralCatalog.TryGetValue(name, out var result) ? result : null;
        }

        // Game
        public new ScaryCastleGame Game { get; }

        // GetProceduralThing
        public GameThing GetProceduralThing(string name)
        {
            return proceduralCatalog == null ? throw new InvalidOperationException() : proceduralCatalog[name];
        }

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

        // HoveredItem
        public Item? HoveredItem { get; set; }

        // InteractionContext
        public InteractionContext InteractionContext { get; }

        // InteractionData
        public InteractionData InteractionData { get; }

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
                    RunHUD?.Reset();
                    InteractionData.Clear();
                    if (value != null)
                        Camera.Follow(value);
                }
            }
        }

        // PreviousRoom
        [ScriptProperty]
        public new GameRoom? PreviousRoom => (GameRoom?)base.PreviousRoom;

        // ProcessTurn
        public void ProcessTurn(int turnPenalty = 0)
        {
            if (Room is not ProceduralRoom room)
                return;

            if (Player?.IsDead == true)
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

        // RunHUD
        public RunHUD? RunHUD { get; private set; }

        // RunIndex
        [ScriptProperty]
        public int RunIndex { get; set; }

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
            if (inventoryScene != null)
                Game.SceneManager.Push(inventoryScene);
        }
    }
}