using Engendro;
using EngendroAdventure;
using EngendroAdventure.Scripting;
using EngendroAdventure.Scripting.Core;
using Microsoft.Xna.Framework;
using Remizione.Scenes;
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

        private enum AttributeName { RandomSeed }
        private readonly ScriptConsole? console;
        private readonly InventoryScene inventoryScene;
        private Actor? player;
        private Vector2? playerPosition;
        private readonly RoomEditor? roomEditor;
        private readonly Dictionary<PlacementPhase, List<Prop>> staticThings = [];

        #endregion

        #region Constructor

        // Constructor
        public GameSession(RemizioneGame game, int slotNumber)
            : base(game, new RemizionePersistenceModel(), ContentHelper.EncodePath(game.Content, ContentFolder.System, "ScriptLibrary.esl"), slotNumber)
        {
            this.Game = game;
            this.HUD = new HUD(this);
            this.Environment = new Environment();
            this.RandomSeed = RandomSeed = System.Environment.TickCount;
            this.Random = new Random(RandomSeed);

            ObjectPools = new ObjectPools(this);
            OverlayTexts = new OverlayTextManager(game);

            Camera.SmoothSpeed = GameSettings.CameraSmoothSpeed;

            BackgroundColor = ColorPalette.BackgroundColor;

            if (EngendroGame.DebugMode)
            {
                TextSprite consoleText = new(game, Fonts.Speech)
                {
                    Color = ColorPalette.HighlightedText,
                    PivotOrigin = RectanglePoint.LeftBottom,
                    Position = Screen.SafeArea.GetPoint(RectanglePoint.LeftBottom),
                    Scale = ScaleInfo.Text.Medium,
                };

                console = new ScriptConsole(this, InputBindings.Console, consoleText, new RectangleF(0, 240, 480, 30)) { TextErrorColor = Color.DarkRed };
                roomEditor = new RoomEditor(this);
            }

            this.inventoryScene = new(this);

            LocalizationSource = LocalizationSource.Script;
        }

        #endregion

        #region Private members

        // ComparePropSizeDescending
        private static int ComparePropSizeDescending(Prop? a, Prop? b)
        {
            if (a == null && b == null) return 0;
            if (a == null) return 1;
            if (b == null) return -1;

            Size sizeA = a.GetRequiredGridSpace(WorldBlockGrid.CellSize);
            Size sizeB = b.GetRequiredGridSpace(WorldBlockGrid.CellSize);
            int areaA = sizeA.Width * sizeA.Height;
            int areaB = sizeB.Width * sizeB.Height;

            return areaB - areaA;
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
            scriptRegistry.RegisterEntity(typeof(Prop));
            scriptRegistry.RegisterEntity(typeof(GameRoom));
            scriptRegistry.RegisterEntity(typeof(CreditsRoom));
            scriptRegistry.RegisterEntity(typeof(ProceduralRoom));
            scriptRegistry.RegisterEntity(typeof(Zabul));

            scriptRegistry.RegisterStatement("act", typeof(ActCommand));
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
            scriptRegistry.RegisterStatement("meta-item", typeof(MetaItemCommand), CodingContext.Declaration);
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

            if (Game.RenderTargets != null)
            {
                Game.RenderTargets.Swap();
                Game.SpriteBatch.Begin(effect: RemizioneGame.Effects.CRT.Effect);
                Game.SpriteBatch.Draw(Game.RenderTargets.PreviousTarget, Vector2.Zero, Color.White);
                Game.SpriteBatch.End();
            }

            if (IsPaused)
            {
                Game.SpriteBatch.Begin(Game.Camera);
                Game.Shapes.DrawRectangle(Screen.Area, ColorPalette.ScenePausedShade);
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
                    staticThings.Add(phase, new());
                }
            }

            // Distribute entities
            foreach (var entity in Entities)
            {
                if (entity is Prop prop)
                    staticThings[prop.PlacementPhase].Add(prop);
            }

            foreach (var phase in Enum.GetValues<PlacementPhase>())
            {
                staticThings[phase].Sort(ComparePropSizeDescending);
            }
        }

        // OnOutcomeCompleted
        protected override void OnOutcomeCompleted(Thing thing)
        {
            player?.SuspendInteraction(500);
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
            {
                RandomSeed = XmlConvert.ToInt32(randomSeedValue);
                Random = new Random(RandomSeed);
            }

            // SelectedItemCategory
            if (sessionNode.Attributes[nameof(SelectedItemCategory)]?.Value is string selectedItemCategoryValue)
                SelectedItemCategory = Enum.Parse<ItemCategory>(selectedItemCategoryValue);
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
                {
                    Environment.BeginCycle(Cycle.Indulgence);
                    Player = GetEntity<Actor>("Sinner");
                }

                if (Player != null)
                {
                    purgatory.Children.Add(Player);

                    if (IsNewSession)
                        Player.Position = purgatory.WorldManager.Blocks[0].BoundingBox.Center;

                    Camera.FollowTarget(Player, true);
                }

                EnterRoom(purgatory);
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

            // SelectedItemCategory
            output.WriteAttributeString(nameof(SelectedItemCategory), XmlConvert.ToString((int)SelectedItemCategory));
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

        // PreservePlayerPosition
        [ScriptMethod]
        public void PreservePlayerPosition()
        {
            if (Player != null)
                this.playerPosition = Player.Position;
        }

        // PreviousRoom
        [ScriptProperty]
        public new GameRoom? PreviousRoom => (GameRoom?)base.PreviousRoom;

        // Random
        public Random Random { get; private set; }

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

        // SelectedItemCategory
        public ItemCategory SelectedItemCategory { get; set; } = ItemCategory.KeyItems;

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

        // ShowInventory
        public void ShowInventory()
        {
            inventoryScene.Actor = player;
            Game.SceneManager.Push(inventoryScene);
            Camera.FocusTarget();
        }
    }
}