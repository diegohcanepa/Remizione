using Engendro;
using Engendro.Input;
using EngendroAdventure;
using EngendroAdventure.Scripting;
using EngendroAdventure.Scripting.Core;
using Microsoft.Xna.Framework;
using Remizione.Creatures;
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

        private enum AttributeName { RandomSeed, WorldVersion }
        private readonly CharacterSheetScene characterSheetScene;
        private readonly ScriptConsole? console;
        private readonly EchoScene echoScene;
        private bool inGameMenuLocked;
        private readonly InGameMenuScene inGameMenuScene;
        private readonly LootScene lootScene;
        private Actor? player;
        private Vector2? playerPosition;
        private int rainRemainingTime;
        private readonly RoomEditor? roomEditor;
        private readonly Dictionary<PlacementPhase, List<GameThing>> staticThings = [];

        #endregion

        #region Constructor

        // Constructor
        public GameSession(RemizioneGame game, int slotNumber)
            : base(game, new RemizionePersistenceModel(), ContentHelper.EncodePath(game.Content, ContentFolder.System, "ScriptLibrary.esl"), slotNumber)
        {
            this.Game = game;
            this.Environment = new Environment(this);
            this.HUD = new HUD(this);
            this.RandomSeed = 10000;// RandomSeed = System.Environment.TickCount;
            //this.RandomSeed = System.Environment.TickCount;

            ObjectPools = new ObjectPools(this);
            ImpactWordPool = new ObjectPool<ImpactWord>(() => new ImpactWord(game), 100);
            OverlayTexts = new OverlayTextManager(game);

            Camera.SmoothSpeed = GameSettings.CameraSmoothSpeed;

            BackgroundColor = ColorPalette.BackgroundColor;

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

            this.inGameMenuScene = new(this);
            this.characterSheetScene = new(Game);
            this.echoScene = new(Game);
            this.lootScene = new(this);

            LocalizationSource = LocalizationSource.Script;
        }

        #endregion

        #region Private members

        // ExpandRoom
        private bool ExpandRoom(Direction direction)
        {
            if (Player == null || Room is not ProceduralRoom proceduralRoom)
                return false;

            return proceduralRoom.Expand(Player.Position, direction);
        }

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

        protected override void OnLoadContent()
        {
            base.OnLoadContent();
            InputManager.Reset();
        }

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
            scriptRegistry.RegisterEntity(typeof(IsometricProp));
            scriptRegistry.RegisterEntity(typeof(Orb));
            scriptRegistry.RegisterEntity(typeof(Pickup));
            scriptRegistry.RegisterEntity(typeof(Prop));
            scriptRegistry.RegisterEntity(typeof(GameRoom));
            scriptRegistry.RegisterEntity(typeof(CreditsRoom));
            scriptRegistry.RegisterEntity(typeof(ProceduralRoom));
            scriptRegistry.RegisterEntity(typeof(Unredeemed));
            scriptRegistry.RegisterEntity(typeof(Zabul));

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
            scriptRegistry.RegisterStatement("placement-condition", typeof(PlacementConditionCommand), CodingContext.EntityDeclaration);
            scriptRegistry.RegisterStatement("meta-item", typeof(MetaItemCommand), CodingContext.Declaration);
            scriptRegistry.RegisterStatement("place-dynamic-prop", typeof(PlaceDynamicPropCommand), CodingContext.Execution);
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
                Environment.EnterRoom(gameRoom);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (HUD.HandleInput(gameTime) == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            if (inGameMenuLocked && InputBindings.InGameMenu.IsKeyUp())
                inGameMenuLocked = false;

            if (!inGameMenuLocked && !InputManager.DefaultPlayer.Keyboard.IsShiftDown() && InputBindings.InGameMenu.IsPressed(PlayerIndex.One))
            {
                ShowInGameMenu();
                return HandleInputResult.Handled;
            }
            else
                return base.OnHandleInput(gameTime);
        }

        // OnInitializeEntities
        protected override void OnInitializeEntities()
        {
            // Add dictionary entries
            if (staticThings.Count == 0)
            {
                foreach (var phase in Enum.GetValues<PlacementPhase>())
                {
                    staticThings.Add(phase, []);
                }
            }

            // Distribute entities
            foreach (var entity in Entities)
            {
                if (entity is GameThing thing)
                    staticThings[thing.PlacementPhase].Add(thing);
            }
        }

        // OnOutcomeCompleted
        protected override void OnOutcomeCompleted(Thing thing)
        {
            if (Player != null)
                Player.SuspendInteraction(500);
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

            // Cycle
            if (sessionNode.Attributes[nameof(Environment.Cycle)]?.Value is string cycleValue)
                this.Environment.Cycle = Enum.Parse<Cycle>(cycleValue);

            // CycleCooldown
            if (sessionNode.Attributes[nameof(Environment.CycleCooldown)]?.Value is string cycleCooldownValue)
                this.Environment.CycleCooldown = XmlConvert.ToInt32(cycleCooldownValue);

            // CycleCount
            if (sessionNode.Attributes[nameof(Environment.CycleCount)]?.Value is string cycleCountValue)
                this.Environment.CycleCount = XmlConvert.ToInt32(cycleCountValue);

            // InventoryEnabled
            if (sessionNode.Attributes[nameof(InventoryEnabled)]?.Value is string inventoryEnabledValue)
                InventoryEnabled = XmlConvert.ToBoolean(inventoryEnabledValue);

            // Player
            if (sessionNode.Attributes[nameof(Player)]?.Value is string player)
                Player = GetEntity<Actor>(player);

            // Player position
            if (sessionNode.Attributes[nameof(playerPosition)]?.Value is string playerPositionValue)
                playerPosition = XmlConverterExtension.ToVector2(playerPositionValue);

            // PurgatoryMode
            if (sessionNode.Attributes[nameof(PurgatoryMode)]?.Value is string purgatoryModeValue)
                PurgatoryMode = XmlConvert.ToBoolean(purgatoryModeValue);

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
            if (GetEntity<ProceduralRoom>("Purgatory") is ProceduralRoom purgatory)
            {
                if (Player != null)
                {
                    if (IsNewSession)
                        Player.Position = purgatory.WorldManager.Blocks[0].BoundingBox.Center;

                    Camera.FollowTarget(Player, true);
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
            // Cycle
            output.WriteAttributeString(nameof(Environment.Cycle), Environment.Cycle.ToString());

            // CycleCooldown
            output.WriteAttributeString(nameof(Environment.CycleCooldown), XmlConvert.ToString(Environment.CycleCooldown));

            // CycleCount
            output.WriteAttributeString(nameof(Environment.CycleCount), XmlConvert.ToString(Environment.CycleCount));

            // InventoryEnabled
            output.WriteAttributeString(nameof(InventoryEnabled), XmlConvert.ToString(InventoryEnabled));

            // NextRainCooldown
            output.WriteAttributeString(nameof(NextRainCooldown), XmlConvert.ToString(NextRainCooldown));

            // Player
            if (Player != null)
                output.WriteAttributeString(nameof(Player), Player.Name);

            // PlayerPosition
            if (playerPosition.HasValue)
                output.WriteAttributeString(nameof(playerPosition), XmlConverterExtension.ToString(playerPosition.Value));

            // PurgatoryMode
            output.WriteAttributeString(nameof(PurgatoryMode), XmlConvert.ToString(PurgatoryMode));

            // RainRemainingTime
            output.WriteAttributeString(nameof(Environment.Rain.RemainingTime), XmlConvert.ToString(Environment.Rain.RemainingTime));

            // RandomSeed
            output.WriteAttributeString(AttributeName.RandomSeed.ToString(), XmlConvert.ToString(RandomSeed));

            // WorldVersion
            output.WriteAttributeString(AttributeName.WorldVersion.ToString(), XmlConvert.ToString(WorldVersion));
        }

        #endregion

        // ClearOverlayTexts
        [ScriptMethod(CodingContext.Any)]
        public void ClearOverlayTexts() => OverlayTexts.Clear();

        // DialogOptionId
        [ScriptProperty]
        public int DialogOptionId { get; set; }

        // Environment
        public Environment Environment { get; }

        // ExpandDown
        [ScriptMethod]
        public void ExpandDown() => ExpandRoom(Direction.Down);

        // ExpandLeft
        [ScriptMethod]
        public void ExpandLeft() => ExpandRoom(Direction.Left);

        // ExpandRight
        [ScriptMethod]
        public void ExpandRight() => ExpandRoom(Direction.Right);

        // ExpandUp
        [ScriptMethod]
        public void ExpandUp() => ExpandRoom(Direction.Up);

        // Game
        public new RemizioneGame Game { get; }

        // GetStaticThings
        public IEnumerable<GameThing> GetStaticThings(PlacementPhase phase)
        {
            foreach (var thing in staticThings[phase])
            {
                yield return thing;
            }
        }

        // HUD
        public HUD HUD { get; }

        // ImpactWordPool
        public ObjectPool<ImpactWord> ImpactWordPool { get; }

        // InventoryEnabled
        [ScriptProperty]
        public bool InventoryEnabled { get; set; }

        // IsConsoleVisible
        public bool IsConsoleVisible => console?.IsActive ?? false;

        // LightingSystem
        [ScriptProperty]
        public bool LightingSystem { get; set; } = true;

        // NextRainCooldown
        [ScriptProperty(CodingContext.Any)]
        public int NextRainCooldown { get; private set; }

        // NextRoom
        [ScriptProperty]
        public new GameRoom? NextRoom => (GameRoom?)base.NextRoom;

        // ObjectPools
        public ObjectPools ObjectPools { get; }

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

        // PurgatoryMode
        [ScriptProperty]
        public bool PurgatoryMode { get; set; }

        // PreviousRoom
        [ScriptProperty]
        public new GameRoom? PreviousRoom => (GameRoom?)base.PreviousRoom;

        // RandomSeed
        public int RandomSeed { get; private set; }

        // RestorePlayerPosition
        [ScriptMethod]
        public void RestorePlayerPosition()
        {
            if (Player != null && playerPosition.HasValue)
                Player.Position = playerPosition.Value;
        }

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

        // ShowCharacterSheet
        public void ShowCharacterSheet()
        {
            if (Player == null)
                return;

            characterSheetScene.Actor = Player;
            Game.SceneManager.Push(characterSheetScene);
        }

        // ShowEcho
        public void ShowEcho(string text)
        {
            echoScene.Text = text;
            Game.SceneManager.Push(echoScene);
        }

        // ShowInGameMenu
        public void ShowInGameMenu()
        {
            inGameMenuLocked = true;
            HUD.Log.Hide();
            Game.SceneManager.Push(inGameMenuScene);
        }

        // ShowLootScene
        [ScriptMethod]
        public void ShowLootScene()
        {
            if (Player?.InteractiveTarget == null)
                return;

            lootScene.Target = Player.InteractiveTarget;
            Game.SceneManager.Push(lootScene);
        }

        // WorldVersion
        public int WorldVersion { get; set; } = 1;
    }
}