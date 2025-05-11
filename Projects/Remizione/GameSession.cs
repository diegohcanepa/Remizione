using Engendro;
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
        private readonly ScriptConsole? console;
        private readonly ContextMenuScene contextMenuScene;
        private readonly InventoryScene inventoryScene;
        private Actor? player;
        private Vector2? playerPosition;
        private readonly RoomEditor? roomEditor;
        private readonly Dictionary<PlacementPhase, List<GameThing>> staticThings = [];

        #endregion

        #region Constructor

        // Constructor
        public GameSession(RemizioneGame game, int slotNumber)
            : base(game, new RemizionePersistenceModel(), ContentHelper.EncodePath(game.Content, ContentFolder.System, "ScriptLibrary.esl"), slotNumber)
        {
            this.Game = game;
            this.HUD = new HUD(this);
            this.Environment = new Environment(this);
            this.RandomSeed = 10000;// RandomSeed = System.Environment.TickCount;
            this.CombatManager = new CombatManager(this);

            ObjectPools = new ObjectPools(this);
            OverlayTexts = new OverlayTextManager(game);

            Camera.SmoothSpeed = GameSettings.CameraSmoothSpeed;

            BackgroundColor = ColorPalette.BackgroundColor;

            if (EngendroGame.DebugMode)
            {
                TextSprite consoleText = new(game, Fonts.Common)
                {
                    Color = ColorPalette.HighlightedText,
                    PivotOrigin = RectanglePoint.LeftBottom,
                    Position = Screen.SafeArea.GetPoint(RectanglePoint.LeftBottom),
                    Scale = ScaleInfo.Text.Medium,
                };

                console = new ScriptConsole(this, InputBindings.Console, consoleText, new RectangleF(0, 240, 480, 30)) { TextErrorColor = Color.DarkRed };
                roomEditor = new RoomEditor(this);
            }

            this.contextMenuScene = new(this);
            this.inventoryScene = new(this.Game);

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
            // No active player
            if (Player == null)
            {
                MouseCursor.Instance.State = MouseCursorState.Cross;
                return;
            }

            // Wait
            if (Player.TurnState != CombatTurnState.WaitingInput && CombatManager.TurnList.Count >= 2)
            {
                MouseCursor.Instance.State = MouseCursorState.Wait;
                return;
            }

            if (TargetMode)
            {
                if (Player.InteractiveTarget != null && Player.InteractiveTarget.CanBeTargeted)
                    MouseCursor.Instance.State = MouseCursorState.TargetOn;
                else
                    MouseCursor.Instance.State = MouseCursorState.Target;
            }
            else
                MouseCursor.Instance.State = Player.InteractiveTarget == null ? MouseCursorState.Cross : MouseCursorState.CrossOn;
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
            scriptRegistry.RegisterEntity(typeof(PickupItem));
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
            scriptRegistry.RegisterStatement("await-credits", typeof(AwaitCreditsCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("await-dialog-block", typeof(AwaitDialogBlockCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("await-player-approach", typeof(AwaitPlayerApproachCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("await-popup", typeof(AwaitPopupCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("create-dialog-block", typeof(CreateDialogBlockCommand));
            scriptRegistry.RegisterStatement("damage-bonus-effect", typeof(DamageBonusEffectCommand), CodingContext.Declaration);
            scriptRegistry.RegisterStatement("echo", typeof(EchoCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("ensure-session-scene", typeof(EnsureSessionSceneCommand));
            scriptRegistry.RegisterStatement("exit-session", typeof(ExitSessionCommand));
            scriptRegistry.RegisterStatement("hide-overlay-text", typeof(HideOverlayTextCommand));
            scriptRegistry.RegisterStatement("placement-condition", typeof(PlacementConditionCommand), CodingContext.EntityDeclaration);
            scriptRegistry.RegisterStatement("meta-item", typeof(MetaItemCommand), CodingContext.Declaration);
            scriptRegistry.RegisterStatement("pickup", typeof(PickupCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("say", typeof(SayCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("select-walk-area", typeof(SelectWalkAreaCommand));
            scriptRegistry.RegisterStatement("set-light", typeof(SetLightCommand), CodingContext.Execution);
            scriptRegistry.RegisterStatement("set-thing-light", typeof(SetThingLightCommand), CodingContext.EntityDeclaration);
            scriptRegistry.RegisterStatement("terminate-dialog-block", typeof(TerminateDialogBlockCommand));
            scriptRegistry.RegisterStatement("verbs", typeof(VerbsCommand), CodingContext.EntityDeclaration);
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
            /*
            if (room is CommonRoom commonRoom)
            {
                Environment.EnterRoom(commonRoom);
            }
            */
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
            {
                Player.SuspendInteraction(500);
                Player.EndTurn();
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
                throw new InvalidOperationException("Session node attributes not found");

            // FullHUD
            if (sessionNode.Attributes[nameof(FullHUD)]?.Value is string fullHUDValue)
                FullHUD = XmlConvert.ToBoolean(fullHUDValue);

            // Player
            if (sessionNode.Attributes[nameof(Player)]?.Value is string player)
                Player = GetEntity<Actor>(player);

            // Player position
            if (sessionNode.Attributes[nameof(playerPosition)]?.Value is string playerPositionValue)
                playerPosition = XmlConverterExtension.ToVector2(playerPositionValue);

            // RandomSeed
            if (sessionNode.Attributes[AttributeName.RandomSeed.ToString()]?.Value is string randomSeedValue)
                RandomSeed = XmlConvert.ToInt32(randomSeedValue);

            // WorldVersion
            if (sessionNode.Attributes[AttributeName.WorldVersion.ToString()]?.Value is string worldVersionValue)
                RandomSeed = XmlConvert.ToInt32(worldVersionValue);
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
            if (GetEntity<ProceduralRoom>("Purgatory") is ProceduralRoom purgatory)
            {
                if (IsNewSession)
                    Environment.BeginCycle(Cycle.Indulgence);

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

            if (IsCurrentScene)
                UpdateMouseCursor();

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
            // FullHUD
            output.WriteAttributeString(nameof(FullHUD), XmlConvert.ToString(FullHUD));

            // Player
            if (Player != null)
                output.WriteAttributeString(nameof(Player), Player.Name);

            // PlayerPosition
            if (playerPosition.HasValue)
                output.WriteAttributeString(nameof(playerPosition), XmlConverterExtension.ToString(playerPosition.Value));

            // RandomSeed
            output.WriteAttributeString(AttributeName.RandomSeed.ToString(), XmlConvert.ToString(RandomSeed));

            // WorldVersion
            output.WriteAttributeString(AttributeName.WorldVersion.ToString(), XmlConvert.ToString(WorldVersion));
        }

        #endregion

        // ClearOverlayTexts
        [ScriptMethod(CodingContext.Any)]
        public void ClearOverlayTexts() => OverlayTexts.Clear();

        // CombatManager
        public CombatManager CombatManager { get; }

        // CycleCount
        [ScriptProperty]
        public int CycleCount { get; set; }

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

        // FullHUD
        [ScriptProperty]
        public bool FullHUD { get; set; } = true;

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

        // LightingSystem
        [ScriptProperty]
        public bool LightingSystem { get; set; } = true;

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

        // ShowContextMenu
        public void ShowContextMenu(GameThing thing)
        {
            if (player == null)
                return;

            contextMenuScene.Target = thing;
            Game.SceneManager.Push(contextMenuScene);
        }

        // ShowInventory
        [ScriptMethod]
        public void ShowInventory()
        {
            if (Player == null)
                return;

            inventoryScene.Actor = Player;
            Game.SceneManager.Push(inventoryScene);
        }

        // TargetMode
        public bool TargetMode { get; set; }

        // WorldVersion
        public int WorldVersion { get; set; } = 1;
    }
}