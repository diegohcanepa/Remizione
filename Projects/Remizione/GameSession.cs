using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Collections;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Remizione.Scripting;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml;

namespace Remizione
{
    /// <summary>
    /// GameSession
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public sealed class GameSession : Session
    {
        #region Private fields

        private readonly ScriptConsole? console;
        private readonly Sprite interactionTargetMarker;
        private readonly List<(ItemDefinition itemDef, int amount, GameRoom room, Vector2 position)> droppedKeyItems = [];
        private string? lastBonfireNameInSaveFile;
        private Vector2? playerPosition;
        private FrozenDictionary<string, GameThing>? proceduralCatalog;
        private readonly RoomEditor? roomEditor;
        private readonly TextSprite gameSavedText;
        private readonly UnlockManager unlockManager;

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
            : base(game, Path.Combine("Content", ContentFolder.System.ToString(), "ScriptLibrary.esl"), slotNumber)
        {
            this.Game = game;
            this.Environment = new Environment();
            this.InteractionContext = new(this);
            this.InteractionData = new();
            this.PlayerData = new(this);
            this.HUD = new(this);
            this.Overlays = new(this);
            this.LootGenerator = new(this);
            this.PersistenceModel = new RemizionePersistenceModel();
            this.RenewSeed();
            this.unlockManager = new(this.PlayerData);

            ObjectPools = new ObjectPools(this);

            BackgroundColor = Colors.BackgroundColor;
            Camera.SmoothSpeed = GameSettings.CameraSmoothSpeed;

            if (EngendroGame.DebugMode)
            {
                TextSprite consoleText = new(Fonts.CommonOutline)
                {
                    Color = Colors.Palette.Text,
                    MaximumWidth = Screen.NativeWidth - 20,
                    PivotOrigin = RectanglePoint.LeftBottom,
                    Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom),
                    Scale = ScaleInfo.Text.VeryLarge,
                };

                console = new(this, InputBindings.Console, consoleText, new RectangleF(0, 115, 240, 20))
                {
                    TextErrorColor = Colors.Palette.Terra
                };
                console.CommandList.Add("put PotteryA into $Room #at:260,130");

                roomEditor = new RoomEditor(this);
            }

            // Game saved text
            this.gameSavedText = new TextSprite(Fonts.CommonOutline)
            {
                Color = Colors.Palette.Green,
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightTop),
                Scale = ScaleInfo.Text.Large,
                Text = TextRepository.GetValue("Misc.GameSaved"),
            };

            LocalizationSource = LocalizationSource.Script;

            this.interactionTargetMarker = new(Atlases.UI.InteractionTarget)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Scale = ScaleInfo.UIElement.Medium,
            };

            interactionTargetMarker.Tweens.OpacityTween = FloatTween.Create(TweenStyle.Linear, .7f, 1, 200, -1);
        }

        #endregion

        #region Private members

        // ApplyGrainEffect
        private void ApplyGrainEffect()
        {
            RemizioneGame.Effects.Grain.CameraOffset.SetValue(Camera.Position);

            Game.RenderTargets.Swap();
            Game.SpriteBatch.Begin(effect: RemizioneGame.Effects.Grain.Effect);
            Game.SpriteBatch.Draw(Game.RenderTargets.PreviousTarget, Vector2.Zero, Color.White);
            Game.SpriteBatch.End();
        }

        // DrawInteractionTargetMarker
        private void DrawInteractionTargetMarker(GameTime gameTime)
        {
            if (InteractionData.Target?.IsHPMeterVisible == false)
            {
                Game.SpriteBatch.Begin(Camera);
                interactionTargetMarker.Draw(gameTime);
                Game.SpriteBatch.End();
            }
        }

        // DrawFlyOffs
        private void DrawFlyOffs(GameTime gameTime)
        {
            for (var i = ObjectPools.FlyOffs.InUse.Count - 1; i >= 0; i--)
            {
                ObjectPools.FlyOffs.InUse[i].Draw(gameTime);
            }
        }

        // RegisterAotTypes
        private static void RegisterAotTypes()
        {
            AotTypeRegistry.Register(typeof(Actor));
            AotTypeRegistry.Register(typeof(Bonfire));
            AotTypeRegistry.Register(typeof(Creature));
            AotTypeRegistry.Register(typeof(CreditsRoom));
            AotTypeRegistry.Register(typeof(FigTree));
            AotTypeRegistry.Register(typeof(Fleshiness));
            AotTypeRegistry.Register(typeof(GameRoom));
            AotTypeRegistry.Register(typeof(ItemOrb));
            AotTypeRegistry.Register(typeof(Pottery));
            AotTypeRegistry.Register(typeof(ProceduralRoom));
            AotTypeRegistry.Register(typeof(Prop));
            AotTypeRegistry.Register(typeof(SpearTrap));
            AotTypeRegistry.Register(typeof(GroundTorch));
            AotTypeRegistry.Register(typeof(Trunk));

            AotTypeRegistry.Register("add-dialog-option", typeof(AddDialogOptionCommand));
            AotTypeRegistry.Register("add-hole", typeof(AddHoleCommand), CodingContext.EntityDeclaration);
            AotTypeRegistry.Register("add-item", typeof(AddItemCommand));
            AotTypeRegistry.Register("add-light", typeof(AddLightCommand), CodingContext.EntityDeclaration);
            AotTypeRegistry.Register("add-trigger-area", typeof(AddTriggerAreaCommand), CodingContext.EntityDeclaration);
            AotTypeRegistry.Register("add-walk-area", typeof(AddWalkAreaCommand), CodingContext.EntityDeclaration);
            AotTypeRegistry.Register("animate-actor", typeof(AnimateActorCommand));
            AotTypeRegistry.Register("apply-usage", typeof(ApplyUsageCommand));
            AotTypeRegistry.Register("attach-light", typeof(AttachLightCommand), CodingContext.EntityDeclaration);
            AotTypeRegistry.Register("await-approach", typeof(AwaitApproachCommand));
            AotTypeRegistry.Register("await-bonfire-scene", typeof(AwaitBonfireSceneCommand));
            AotTypeRegistry.Register("await-credits", typeof(AwaitCreditsCommand));
            AotTypeRegistry.Register("await-dialog-block", typeof(AwaitDialogBlockCommand));
            AotTypeRegistry.Register("await-input", typeof(AwaitInputCommand));
            AotTypeRegistry.Register("craft-item", typeof(CraftItemCommand));
            AotTypeRegistry.Register("create-dialog-block", typeof(CreateDialogBlockCommand));
            AotTypeRegistry.Register("echo", typeof(EchoCommand));
            AotTypeRegistry.Register("ensure-session-scene", typeof(EnsureSessionSceneCommand));
            AotTypeRegistry.Register("epigrah", typeof(EpigraphCommand));
            AotTypeRegistry.Register("exit-session", typeof(ExitSessionCommand));
            AotTypeRegistry.Register("if-can-pickup-item-reward", typeof(IfCanPickUpItemRewardStatement));
            AotTypeRegistry.Register("if-test-skill", typeof(IfTestSkillStatement));
            AotTypeRegistry.Register("narrate", typeof(NarrateCommand));
            AotTypeRegistry.Register("pickup-item-reward", typeof(PickUpItemRewardCommand));
            AotTypeRegistry.Register("place-item", typeof(PlaceItemCommand));
            AotTypeRegistry.Register("say", typeof(SayCommand));
            AotTypeRegistry.Register("select-walk-area", typeof(SelectWalkAreaCommand));
            AotTypeRegistry.Register("sepia-effect", typeof(SepiaEffectCommand));
            AotTypeRegistry.Register("set-item-reward", typeof(SetItemRewardCommand), CodingContext.Any);
            AotTypeRegistry.Register("set-light", typeof(SetLightCommand));
            AotTypeRegistry.Register("show-message", typeof(ShowMessageCommand));
            AotTypeRegistry.Register("terminate-dialog-block", typeof(TerminateDialogBlockCommand));
            AotTypeRegistry.Register("x-tween", typeof(XTweenCommand));
            AotTypeRegistry.Register("y-tween", typeof(YTweenCommand));
        }

        // RemoveAnnonymousChildren
        private static void RemoveAnnonymousChildren(Entity entity)
        {
            var annonymousList = new List<Entity>();

            for (var i = 0; i < entity.Children.Count; i++)
            {
                if (entity.Children[i].InstanceKind == EntityInstanceKind.Anonymous)
                    annonymousList.Add(entity.Children[i]);
            }

            for (var i = 0; i < annonymousList.Count; i++)
            {
                annonymousList[i].Unparent();
            }
        }

        // RenewSeed
        private void RenewSeed()
        {
            Seed = RandomNumberGenerator.GetInt32(int.MaxValue);
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

        // OnAwait
        protected override void OnAwait()
        {
            MouseCursor.Icon = MouseCursorIcon.Wait;
            if (Game.SceneManager.CurrentScene is InventoryScene)
                Game.SceneManager.Pop();
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            if (gameSavedText.Tweens.IsTweening)
            {
                Game.SpriteBatch.Begin(Game.Camera);
                gameSavedText.Draw(gameTime);
                Game.SpriteBatch.End();
            }

            /*
            if (IsPaused)
            {
                Game.SpriteBatch.Begin(Game.Camera);
                Game.Shapes.DrawRectangle(Screen.Area, ColorPalette.SceneShade);
                Game.SpriteBatch.End();
            }
            */

            SepiaEffect.Draw(gameTime);

            ApplyGrainEffect();
            DrawInteractionTargetMarker(gameTime);
            DrawFlyOffs(gameTime);

            if (Player != null && !Player.IsDead)
                HUD?.Draw(gameTime);

            console?.Draw(gameTime);

            SpeechText.DrawTexts(gameTime);

            roomEditor?.Draw(gameTime);
        }

        // OnEnterRoom
        protected override void OnEnterRoom(Room room)
        {
            Game.SceneManager.PopUntil(this);
            MouseCursor.Reset();
            SyncProceduralMusic();

            if (lastBonfireNameInSaveFile != null)
            {
                TransitionManager.DefaultTransition.In(0);
                LastBonfire = FindEntity<Bonfire>(lastBonfireNameInSaveFile);
                RestartFromLastBonfire();
                TransitionManager.DefaultTransition.Out(3000);
            }

            if (room is GameRoom gameRoom)
                gameRoom.WalkArea?.BuildStaticNavGraph();
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
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
#if DEBUG

            // Console
            if (console?.IsActive == true)
                return HandleInputResult.Handled;

            // Room editor
            else if (roomEditor?.HandleInput() == HandleInputResult.Handled)
                return HandleInputResult.Handled;

#endif

            // HUD
            if (HUD.HandleInput(gameTime) == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            // Player
            if (PlayerInputHandler.HandleInput(gameTime) == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            if (InputBindings.Back.IsPressed(PlayerIndex.One))
                Game.Exit();

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
                throw new InvalidOperationException("Session node attributes not found.");

            // BonfireLevel
            if (sessionNode.Attributes[nameof(PlayerData.Bonfires)]?.Value is string bonfireLevelValue)
                PlayerData.Bonfires = XmlConvert.ToInt32(bonfireLevelValue);

            // LastBonfire
            lastBonfireNameInSaveFile = sessionNode.Attributes[nameof(LastBonfire)]?.Value;

            // DisplayHPMeter
            if (sessionNode.Attributes[nameof(DisplayHPMeter)]?.Value is string displayHPMeterValue)
                DisplayHPMeter = XmlConvert.ToBoolean(displayHPMeterValue);

            // Grace
            if (sessionNode.Attributes[nameof(PlayerData.Grace)]?.Value is string graceValue)
                PlayerData.Grace = XmlConvert.ToInt32(graceValue);

            // Inventory
            if (sessionNode.Attributes[nameof(PlayerData.Inventory)]?.Value is string inventoryValue)
                PlayerData.Inventory.Deserialize(inventoryValue);

            // InventoryEnabled
            if (sessionNode.Attributes[nameof(InventoryEnabled)]?.Value is string inventoryEnabledValue)
                InventoryEnabled = XmlConvert.ToBoolean(inventoryEnabledValue);

            // IsSafe
            if (sessionNode.Attributes[nameof(PlayerData.IsSafe)]?.Value is string isSafeValue)
                PlayerData.IsSafe = XmlConvert.ToBoolean(isSafeValue);

            // Player
            if (sessionNode.Attributes[nameof(Player)]?.Value is string player)
                Player = FindEntity<Actor>(player);

            // Player position
            if (sessionNode.Attributes[nameof(playerPosition)]?.Value is string playerPositionValue)
                playerPosition = DataConvert.ToVector2(playerPositionValue);

            // RunCount
            if (sessionNode.Attributes[nameof(RunIndex)]?.Value is string runCountValue)
                RunIndex = XmlConvert.ToInt32(runCountValue);

            // SepiaIntensity
            if (sessionNode.Attributes["SepiaIntensity"]?.Value is string sepiaIntensityValue)
                SepiaEffect.Intensity = XmlConvert.ToSingle(sepiaIntensityValue);

            // UnlockedDefinitions
            if (sessionNode.Attributes["UnlockedDefinitions"]?.Value is string UnlockedDefinitionsValue)
                unlockManager.Deserialize(UnlockedDefinitionsValue);

            // Dropped key items
            droppedKeyItems.Clear();
            if (sessionNode.Attributes["DroppedKeyItems"]?.Value is string droppedKeyItemsValue)
            {
                var orbs = droppedKeyItemsValue.Split(';');

                foreach (var item in orbs)
                {
                    var itemData = item.Split('|');
                    if (GameData.Items.Find(itemData[0]) is ItemDefinition itemDefinition)
                    {
                        var amount = XmlConvert.ToInt32(itemData[1]);
                        if (FindEntity<GameRoom>(itemData[2]) is GameRoom room)
                        {
                            var pos = DataConvert.ToVector2(itemData[3]);
                            droppedKeyItems.Add(new(itemDefinition, amount, room, pos));
                        }
                    }
                }
            }
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
            gameSavedText.Tweens.OpacityTween = FloatTween.Create(TweenStyle.QuadraticInOut, 1, .8f, 300, 10);
        }

        // OnScriptLibraryLoaded
        protected override void OnScriptLibraryLoaded()
        {
            GameDataValidator.Validate(ScriptLibrary);
        }

        // OnStarted
        protected override void OnStarted()
        {
            unlockManager.Refresh();
            
            if (!PlayerData.IsSafe)
            {
                ApplyDeathPenalty();
                PlayerData.IsSafe = true;
            }

            Dictionary<string, GameThing> dict = [];

            // Collect all things that has data-driven definitions
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

            foreach (var item in droppedKeyItems)
            {
                ItemOrb.Drop(item.itemDef, item.amount, item.room, item.position);
            }

            droppedKeyItems.Clear();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            SepiaEffect.Update(gameTime);

            gameSavedText.Update(gameTime);

            if (console != null)
            {
                if (console.IsActive && roomEditor != null)
                    roomEditor.IsActive = false;

                console?.Update(gameTime);
            }

            if (!IsAwaiting)
            {
                if (Player?.IsDead == true)
                {
                    Player?.StopMoving();
                    AwaitRoutine(RoutineNames.DeathByHealth);
                }
            }

            HUD.Update(gameTime);

            if (IsCurrentScene)
            {
                if (Room == null || !Room.ControlMouseCursor)
                    InteractionContext.Refresh();
            }

            if (Player != null && !IsAwaiting && PlayerData.Inventory.Count > 0 && IsCurrentScene)
            {
                if (Player.HeldProp == null)
                {
                    if (InputManager.DefaultPlayer.Mouse.VirtualPosition.Y > 130)
                    {
                        InteractionContext.HeldItem = null;
                        Overlays.ShowInventory();
                        return;
                    }
                }
            }

            if (InteractionData.Target != null && !InteractionData.Target.IsPlayer)
            {
                interactionTargetMarker.Update(gameTime);
                interactionTargetMarker.Position = InteractionData.Target.GetOverheadPosition(new(0, -3));
            }
        }

        // OnWrite
        protected override void OnWrite(XmlWriter output)
        {
            // BonfireLevel
            output.WriteAttributeString(nameof(PlayerData.Bonfires), XmlConvert.ToString(PlayerData.Bonfires));

            // LastBonfire
            if (LastBonfire != null)
                output.WriteAttributeString(nameof(LastBonfire), LastBonfire.Name);

            // DisplayHPMeter
            output.WriteAttributeString(nameof(DisplayHPMeter), XmlConvert.ToString(DisplayHPMeter));

            // Grace
            output.WriteAttributeString(nameof(PlayerData.Grace), XmlConvert.ToString(PlayerData.Grace));

            // Inventory
            output.WriteAttributeString(nameof(PlayerData.Inventory), PlayerData.Inventory.Serialize());

            // InventoryEnabled
            output.WriteAttributeString(nameof(InventoryEnabled), XmlConvert.ToString(InventoryEnabled));

            // IsSafe
            output.WriteAttributeString(nameof(PlayerData.IsSafe), XmlConvert.ToString(PlayerData.IsSafe));

            // Player
            if (Player != null)
                output.WriteAttributeString(nameof(Player), Player.Name);

            // PlayerPosition
            if (playerPosition.HasValue)
                output.WriteAttributeString(nameof(playerPosition), DataConvert.ToString(playerPosition.Value));

            // RunCount
            output.WriteAttributeString(nameof(RunIndex), XmlConvert.ToString(RunIndex));

            // SepiaIntensity
            output.WriteAttributeString("SepiaIntensity", XmlConvert.ToString(SepiaEffect.Intensity));

            // UnlockedDefinitions
            output.WriteAttributeString("UnlockedDefinitions", unlockManager.Serialize());

            // Dropped key items
            var keyItems = new List<string>();
            foreach (var room in Entities.OfType<GameRoom>())
            {
                foreach (var orb in room.Children.OfType<ItemOrb>())
                {
                    if (orb.ItemReward is { IsKeyItem: true } itemDef)
                    {
                        var value = $"{itemDef.Name}|{XmlConvert.ToString(orb.ItemRewardAmount)}|{room.Name}|{DataConvert.ToString(orb.Position)}";
                        keyItems.Add(value);
                    }
                }
            }

            output.WriteAttributeString("DroppedKeyItems", string.Join(";", keyItems));
        }

        #endregion

        // ApplyDeathPenalty
        [ScriptMethod]
        public void ApplyDeathPenalty()
        {
            PlayerData.Grace = 0;
            Player?.HP = Player.MaxHP / 2;

            for (var i = PlayerData.Inventory.Count - 1; i >= 0; i--)
            {
                var item = PlayerData.Inventory[i];

                if (item.Definition.IsKeyItem)
                    continue;

                if (item.Definition.LostOnDeath)
                {
                    PlayerData.Inventory.RemoveAt(i);
                    continue;
                }

                if (item.Definition.IsPerishable)
                    item.ApplyUsage(null);
            }

            if (CanSave())
                Save();
        }

        // ApplyTimePenalty
        [ScriptMethod]
        public void ApplyTimePenalty()
        {
            for (var i = PlayerData.Inventory.Count - 1; i >= 0; i--)
            {
                var item = PlayerData.Inventory[i];

                if (item.Definition.IsPerishable)
                    item.ApplyUsage(null);
            }

            Save();
        }

        // BonfireMode
        [ScriptProperty]
        public bool BonfireMode { get; set; }

        // CleanupWorld
        // TODO: MUST call this 
        public void CleanupWorld()
        {
            static bool CanCleanup(Entity entity)
            {
                return entity is not ItemOrb itemOrb || itemOrb.AllowCleanup;
            }

            var allRooms = new List<GameRoom>(Entities.OfType<GameRoom>());
            foreach (var room in allRooms)
            {
                RemoveAnnonymousChildren(room);
            }

            CleanUpVolatileRuntimeClones(CanCleanup);
        }

        // DangerousTarget
        [ScriptProperty]
        public GameThing? DangerousTarget { get; set; }

        // DialogOptionId
        [ScriptProperty]
        public int DialogOptionId { get; set; }

        // DisplayHPMeter
        [ScriptProperty]
        public bool DisplayHPMeter { get; set; }

        // EnoughMaterials
        [ScriptProperty]
        public bool EnoughMaterials => PlayerData.Inventory.Find(ItemNames.Material) is Item item && item.Amount >= 5;

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

        // HUD
        public HUD HUD { get; }

        // IncrementBonfireCounter
        [ScriptMethod]
        public void IncrementBonfireCounter()
        {
            PlayerData.Bonfires++;
        }

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

        // IsUnlocked
        public bool IsUnlocked(string definitionName)
        {
            return unlockManager.IsUnlocked(definitionName);
        }

        // KillEnemies
        [ScriptMethod]
        public void KillEnemies()
        {
            if (Room == null)
                return;

            for (int i = Room.Children.Count - 1; i >= 0; i--)
            {
                if (Room.Children[i] is Actor actor && !actor.IsDead && actor.Faction == Faction.Creature)
                    actor.Die();
            }
        }

        // LastBonfire
        [ScriptProperty]
        public Bonfire? LastBonfire { get; set; }

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

        // OutcomeTarget
        [ScriptProperty]
        public override GameThing? OutcomeTarget => base.OutcomeTarget as GameThing;

        // Overlays
        public SessionOverlays Overlays { get; }

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
                    PlayerInputHandler.Owner = field;
                    HUD?.Reset();
                    InteractionData.Clear();
                    if (value != null)
                        Camera.Follow(value);
                }
            }
        }

        // PlayerData
        public PlayerData PlayerData { get; }

        // PlayerInputHandler
        public PlayerInputHandler<Actor> PlayerInputHandler { get; } = new(PlayerIndex.One);

        // PreviousRoom
        [ScriptProperty]
        public new GameRoom? PreviousRoom => (GameRoom?)base.PreviousRoom;

        // Random
        public Random Random { get; private set; } = new(0);

        // RespawnWorld
        [ScriptMethod]
        public void RespawnWorld()
        {
            unlockManager.Refresh();
            RenewSeed();
            CleanupWorld();
            if (Room is ProceduralRoom proceduralRoom)
                proceduralRoom.Populate();
        }

        // RestartFromLastBonfire
        [ScriptMethod]
        public void RestartFromLastBonfire()
        {
            if (LastBonfire != null)
            {
                LastBonfire.Enter(true);
                LastBonfire.Exit(true);

                if (Room != null)
                {
                    if (Player != null)
                    {
                        Room.Children.Add(Player);

                        if (LastBonfire != null)
                            Player.Position = LastBonfire.GetApproachPosition(Player);

                        Camera.Follow(Player);
                        Camera.FocusTarget();
                    }

                    //HUD.RoomTitle.Show(Room);
                }
            }
        }

        // Room
        [ScriptProperty]
        public new GameRoom? Room => (GameRoom?)base.Room;

        // RunIndex
        [ScriptProperty]
        public int RunIndex { get; set; }

        // Seed
        public int Seed
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Random = new(field);
                }
            }
        }

        // SepiaEffect
        public SepiaEffect SepiaEffect { get; } = new();

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

        // TryCraft
        public ItemDefinition? TryCraft()
        {
            var table = new ChanceTable();

            for (int i = 0; i < GameData.Items.Count; i++)
            {
                var itemDef = GameData.Items[i];

                // 1. Excluir ítems no crafteables o sin peso
                if (itemDef.CraftWeight <= 0)
                    continue;

                // 2. Excluir ítems que el jugador aún no ha desbloqueado
                if (!unlockManager.IsUnlocked(itemDef.Name))
                    continue;

                // Agrega el ítem a la ruleta usando su CraftWeight
                table.Add(itemDef.Name, itemDef.CraftWeight, itemDef);
            }

            if (table.Count == 0)
                return null;

            return table.GetItem()?.Context as ItemDefinition;
        }
    }
}