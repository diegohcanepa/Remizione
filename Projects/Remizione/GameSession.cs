using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Remizione.Props;
using Remizione.Scripting;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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

        private const string AngerMusicTag = "Anger";
        private int combatMoodTimer = -1;
        private readonly ScriptConsole? console;
        private readonly EchoScene echoScene;
        private readonly InventoryScene inventoryScene;
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
        public GameSession(RemizioneGame game, int slotNumber)
            : base(game, new RemizionePersistenceModel(), Path.Combine("Content", ContentFolder.System.ToString(), "ScriptLibrary.esl"), slotNumber)
        {
            this.Game = game;
            this.Environment = new Environment();
            this.InteractionContext = new(this);
            this.InteractionData = new(this);
            this.echoScene = new EchoScene(this);
            this.PlayerData = new(this);
            this.inventoryScene = new(PlayerData.Inventory);
            this.HUD = new(this);
            this.LootGenerator = new(this);

            ObjectPools = new ObjectPools(this);

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

                console.CommandList.Add("add-item Apple");

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
                Player.Energy = 0;
            else
                Player.Recharge();
        }

        // RegisterAotTypes
        private static void RegisterAotTypes()
        {
            AotTypeRegistry.Register(typeof(Actor));
            AotTypeRegistry.Register(typeof(Bonfire));
            AotTypeRegistry.Register(typeof(BronzeKey));
            AotTypeRegistry.Register(typeof(CloseUpRoom));
            AotTypeRegistry.Register(typeof(Coin));
            AotTypeRegistry.Register(typeof(CreditsRoom));
            AotTypeRegistry.Register(typeof(Decoration));
            AotTypeRegistry.Register(typeof(Door));
            AotTypeRegistry.Register(typeof(EnviousEye));
            AotTypeRegistry.Register(typeof(GameRoom));
            AotTypeRegistry.Register(typeof(GoldenKey));
            AotTypeRegistry.Register(typeof(LootOrb));
            AotTypeRegistry.Register(typeof(Pottery));
            AotTypeRegistry.Register(typeof(Prop));
            AotTypeRegistry.Register(typeof(Rat));
            AotTypeRegistry.Register(typeof(SpearTrap));
            AotTypeRegistry.Register(typeof(Torch));
            AotTypeRegistry.Register(typeof(TrapdoorKey));
            AotTypeRegistry.Register(typeof(Trunk));

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
            AotTypeRegistry.Register("await-dialog-block", typeof(AwaitDialogBlockCommand));
            AotTypeRegistry.Register("await-input", typeof(AwaitInputCommand));
            AotTypeRegistry.Register("await-npc-turn", typeof(AwaitNPCTurnCommand));
            AotTypeRegistry.Register("await-popup", typeof(AwaitPopupCommand));
            AotTypeRegistry.Register("consume-item", typeof(ConsumeItemCommand));
            AotTypeRegistry.Register("create-dialog-block", typeof(CreateDialogBlockCommand));
            AotTypeRegistry.Register("echo", typeof(EchoCommand));
            AotTypeRegistry.Register("ensure-session-scene", typeof(EnsureSessionSceneCommand));
            AotTypeRegistry.Register("exit-session", typeof(ExitSessionCommand));
            AotTypeRegistry.Register("if-can-pickup", typeof(IfCanPickUpStatement));
            AotTypeRegistry.Register("if-test-skill", typeof(IfTestSkillStatement));
            AotTypeRegistry.Register("pickup", typeof(PickUpCommand));
            AotTypeRegistry.Register("place-item", typeof(PlaceItemCommand));
            AotTypeRegistry.Register("say", typeof(SayCommand));
            AotTypeRegistry.Register("select-walk-area", typeof(SelectWalkAreaCommand));
            AotTypeRegistry.Register("set-item-reward", typeof(SetItemRewardCommand), CodingContext.Any);
            AotTypeRegistry.Register("set-light", typeof(SetLightCommand));
            AotTypeRegistry.Register("show-message", typeof(ShowMessageCommand));
            AotTypeRegistry.Register("terminate-dialog-block", typeof(TerminateDialogBlockCommand));
            AotTypeRegistry.Register("x-tween", typeof(XTweenCommand));
            AotTypeRegistry.Register("y-tween", typeof(YTweenCommand));
        }

        // SyncProceduralMusic
        private void SyncProceduralMusic()
        {
            /*
            if (Room is ProceduralRoom proceduralRoom)
            {
                var tag = proceduralRoom.RoomNode.Definition.MusicTag;

                if (string.IsNullOrWhiteSpace(tag))
                    tag = MusicTag.Castle;

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

            /*
            Game.RenderTargets.Swap();
            Game.SpriteBatch.Begin(effect: RemizioneGame.Effects.CRT.Effect);
            Game.SpriteBatch.Draw(Game.RenderTargets.PreviousTarget, Vector2.Zero, Color.White);
            Game.SpriteBatch.End();
            */

            if (Player != null && !Player.IsDead)
                HUD?.Draw(gameTime);

            console?.Draw(gameTime);

            SpeechText.DrawSpeechTexts(gameTime);

            roomEditor?.Draw(gameTime);
        }

        // OnEnterRoom
        protected override void OnEnterRoom(Room room)
        {
            Game.SceneManager.PopUntil(this);
            MouseCursor.Reset();
            SyncProceduralMusic();
        }

        // OnExitRoom
        protected override void OnExitRoom(Room currentRoom, Room nextRoom)
        {
            HUD?.Reset();
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

            else if (HUD?.HandleInput() == HandleInputResult.Handled)
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

            // DisplayHPMeter
            if (sessionNode.Attributes[nameof(DisplayHPMeter)]?.Value is string displayHPMeterValue)
                DisplayHPMeter = XmlConvert.ToBoolean(displayHPMeterValue);

            // Inventory
            if (sessionNode.Attributes[nameof(PlayerData.Inventory)]?.Value is string inventoryValue)
                PlayerData.Inventory.Deserialize(inventoryValue);

            // InventoryEnabled
            if (sessionNode.Attributes[nameof(InventoryEnabled)]?.Value is string inventoryEnabledValue)
                InventoryEnabled = XmlConvert.ToBoolean(inventoryEnabledValue);

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
            GameDataValidator.Validate(ScriptLibrary);
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

            if (combatMoodTimer >= 0)
            {
                combatMoodTimer -= gameTime.ElapsedGameTime.Milliseconds;
                if (combatMoodTimer < 0)
                {
                    if (Room != null)
                    {
                        var tag = Room.MusicTag;
                        if (string.IsNullOrWhiteSpace(tag))
                            tag = "Idle";

                        AudioManager.Music.PlayTag(tag, 10000);
                    }
                }
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

                if (!IsAwaiting)
                {
                    if (Player?.IsDead == true)
                    {
                        Player?.StopMoving();
                        AwaitRoutine(RoutineNames.DeathByHealth);
                    }
                }
            }

            HUD.Update(gameTime);

            if (IsCurrentScene)
            {
                if (Room == null || !Room.ControlMouseCursor)
                    InteractionContext.Refresh();
            }

            if (PlayerData.Inventory.Count > 0)
            {
                if (Player != null && Player.ActiveThrowable == null)
                {
                    if (!IsAwaiting && IsCurrentScene)
                    {
                        if (InputManager.DefaultPlayer.Mouse.VirtualPosition.Y > 130)
                        {
                            ShowInventory();
                            return;
                        }
                    }
                }
            }
        }

        // OnWrite
        protected override void OnWrite(XmlWriter output)
        {
            // DisplayHPMeter
            output.WriteAttributeString(nameof(DisplayHPMeter), XmlConvert.ToString(DisplayHPMeter));

            // InventoryEnabled
            output.WriteAttributeString(nameof(InventoryEnabled), XmlConvert.ToString(InventoryEnabled));

            // Inventory
            output.WriteAttributeString(nameof(PlayerData.Inventory), PlayerData.Inventory.Serialize());

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

        // BeginCombatMood
        public void BeginCombatMood()
        {
            if (AudioManager.Music.CurrentTag != AngerMusicTag)
                AudioManager.Music.PlayTag(AngerMusicTag);

            combatMoodTimer = 10000;
        }

        // BeginRun
        [ScriptMethod]
        public void BeginRun()
        {
            if (CurrentRun != null)
                throw new InvalidOperationException("A run is already in progress.");

            var runSeed = Seed == 0 ? System.Environment.TickCount : Seed;

            // 1. Get descriptor
            var runDefinition = GameData.Runs[RunIndex];

            // 2. Create run
            CurrentRun = new Run(this, runSeed, runDefinition);

            //inventoryScene = new(CurrentRun.PlayerInventory);

            PreparePlayerForRun();
            AdvanceToNextFloor();
        }

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

        // DisplayHPMeter
        [ScriptProperty]
        public bool DisplayHPMeter { get; set; } = true;

        // EndRun
        [ScriptMethod]
        public void EndRun()
        {
            if (CurrentRun == null)
                return;

            CurrentRun = null;
            CleanUpRuntimeEntities();
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
        public new RemizioneGame Game { get; }

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

        // HUD
        public HUD HUD { get; }

        // InteractionContext
        public InteractionContext InteractionContext { get; }

        // InteractionData
        public InteractionData InteractionData { get; }

        // InventoryEnabled
        [ScriptProperty]
        public bool InventoryEnabled { get; set; }

        // IsConsoleVisible
        public bool IsConsoleVisible => console?.IsActive ?? false;

        // IsGameplayActive
        public bool IsGameplayActive
        {
            get
            {
                if (TransitionManager.CurrentTransition.IsRunning)
                    return false;

                if (TransitionManager.CurrentTransition.TransitionState == TransitionState.In)
                    return false;

                // Modal speech text active
                if (SpeechText.ModalInstance != null)
                    return false;

                // Session is awaiting script
                if (IsAwaiting)
                    return false;

                // Inventory is active
                if (!IsCurrentScene)
                    return false;

                return true;
            }
        }

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
        public Random MasterRunRng { get; } = new();

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
                    HUD?.Reset();
                    InteractionData.Clear();
                    if (value != null)
                        Camera.Follow(value);
                }
            }
        }

        // PlayerData
        public PlayerData PlayerData { get; }

        // PreviousRoom
        [ScriptProperty]
        public new GameRoom? PreviousRoom => (GameRoom?)base.PreviousRoom;

        // ProcessTurn
        public void ProcessTurn(int turnPenalty = 0)
        {
            if (Room is not GameRoom room)
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

        // RespawnWorld
        [ScriptMethod]
        public void RespawnWorld()
        {
        }

        // Room
        [ScriptProperty]
        public new GameRoom? Room => (GameRoom?)base.Room;

        // RunIndex
        [ScriptProperty]
        public int RunIndex { get; set; }

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

        // ShowDialogMenu
        public void ShowDialogMenu(DialogBlock dialogBlock)
        {
            DialogOptionId = -1;
            var scene = new DialogBlockScene(this, dialogBlock);
            Game.SceneManager.Push(scene);
        }

        // ShowEcho
        public void ShowEcho(string text, string? soundName)
        {
            if (Game.SceneManager.CurrentScene is not EchoScene)
                Game.SceneManager.Push(echoScene);

            echoScene.Show(text, soundName);
        }

        // ShowInventory
        public void ShowInventory()
        {
            if (inventoryScene != null)
                Game.SceneManager.Push(inventoryScene);
        }
    }
}