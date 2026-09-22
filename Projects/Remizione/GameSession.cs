using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Remizione.Props;
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
    public sealed partial class GameSession : Session
    {
        #region Private fields

        private readonly ScriptConsole? console;
        private readonly List<(ItemDefinition itemDef, int amount, GameRoom room, Vector2 position)> droppedKeyItems = [];
        private readonly EchoScene echoScene;
        private readonly InventoryScene inventoryScene;
        private readonly ItemInfoScene itemInfoScene;
        private string? lastBonfireName;
        private readonly NarrationScene narrationScene;
        private Vector2? playerPosition;
        private FrozenDictionary<string, GameThing>? proceduralCatalog;
        private readonly RoomEditor? roomEditor;
        private readonly Sprite savingIcon;
        private readonly HashSet<string> unlockedDefinitions = [];

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
            this.narrationScene = new NarrationScene(this);
            this.PlayerData = new(this);
            this.inventoryScene = new(PlayerData.Inventory);
            this.itemInfoScene = new(this);
            this.HUD = new(this);
            this.LootGenerator = new(this);
            this.RenewSeed();

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
                console.CommandList.Add("put PotteryA into $Room #at:260,130");

                roomEditor = new RoomEditor(this);
            }

            // Saving icon
            this.savingIcon = new Sprite(Atlases.UI.SavingIcon)
            {
                PivotOrigin = RectanglePoint.Center,
                Position = Screen.Area.GetPoint(RectanglePoint.RightTop, -9, 7)
            };

            LocalizationSource = LocalizationSource.Script;
        }

        #endregion

        #region Private members

        // RegisterAotTypes
        private static void RegisterAotTypes()
        {
            AotTypeRegistry.Register(typeof(Actor));
            AotTypeRegistry.Register(typeof(Bonfire));
            AotTypeRegistry.Register(typeof(CreditsRoom));
            AotTypeRegistry.Register(typeof(Decoration));
            AotTypeRegistry.Register(typeof(EnviousEye));
            AotTypeRegistry.Register(typeof(Fleshiness));
            AotTypeRegistry.Register(typeof(GameRoom));
            AotTypeRegistry.Register(typeof(ItemOrb));
            AotTypeRegistry.Register(typeof(Pottery));
            AotTypeRegistry.Register(typeof(ProceduralRoom));
            AotTypeRegistry.Register(typeof(Prop));
            AotTypeRegistry.Register(typeof(Rat));
            AotTypeRegistry.Register(typeof(SpearTrap));
            AotTypeRegistry.Register(typeof(Torch));
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
            AotTypeRegistry.Register("consume-item", typeof(ConsumeItemCommand));
            AotTypeRegistry.Register("create-dialog-block", typeof(CreateDialogBlockCommand));
            AotTypeRegistry.Register("echo", typeof(EchoCommand));
            AotTypeRegistry.Register("ensure-session-scene", typeof(EnsureSessionSceneCommand));
            AotTypeRegistry.Register("exit-session", typeof(ExitSessionCommand));
            AotTypeRegistry.Register("if-can-pickup", typeof(IfCanPickUpStatement));
            AotTypeRegistry.Register("if-test-skill", typeof(IfTestSkillStatement));
            AotTypeRegistry.Register("narrate", typeof(NarrateCommand));
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

        // OnAwait
        protected override void OnAwait()
        {
            MouseCursor.Icon = MouseCursorIcon.Wait;

            if (inventoryScene.IsCurrentScene)
                Game.SceneManager.Pop();
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            if (savingIcon.Tweens.IsTweening)
            {
                Game.SpriteBatch.Begin(Game.Camera);
                savingIcon.Draw(gameTime);
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

            /*
            Game.RenderTargets.Swap();
            Game.SpriteBatch.Begin(effect: RemizioneGame.Effects.CRT.Effect);
            Game.SpriteBatch.Draw(Game.RenderTargets.PreviousTarget, Vector2.Zero, Color.White);
            Game.SpriteBatch.End();
            */

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

            if (lastBonfireName != null)
            {
                TransitionManager.DefaultTransition.In(0);
                LastBonfire = FindEntity<Bonfire>(lastBonfireName);
                RespawnWorld();
                TransitionManager.DefaultTransition.Out(3000);
            }
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

            // LastBonfire
            lastBonfireName = sessionNode.Attributes[nameof(LastBonfire)]?.Value;

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

            // Player
            if (sessionNode.Attributes[nameof(Player)]?.Value is string player)
                Player = FindEntity<Actor>(player);

            // Player position
            if (sessionNode.Attributes[nameof(playerPosition)]?.Value is string playerPositionValue)
                playerPosition = DataConvert.ToVector2(playerPositionValue);

            // RunCount
            if (sessionNode.Attributes[nameof(RunIndex)]?.Value is string runCountValue)
                RunIndex = XmlConvert.ToInt32(runCountValue);

            // UnlockedDefinitions
            if (sessionNode.Attributes["UnlockedDefinitions"]?.Value is string UnlockedDefinitionsValue)
            {
                foreach (var name in UnlockedDefinitionsValue.Split(","))
                {
                    if (Definition.IsDefined(name))
                        unlockedDefinitions.Add(name);
                }
            }

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
            savingIcon.Tweens.OpacityTween = FloatTween.Create(TweenStyle.QuadraticInOut, 1, .8f, 300, 10);
            savingIcon.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.QuadraticInOut, 1, 1.05f, 150, 20);
        }

        // OnScriptLibraryLoaded
        protected override void OnScriptLibraryLoaded()
        {
            GameDataValidator.Validate(ScriptLibrary);
        }

        // OnStarted
        protected override void OnStarted()
        {
            foreach (var definition in Definition.All)
            {
                if (definition.IsUnlockedByDefault)
                    Unlock(definition);
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

            savingIcon.Update(gameTime);

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

            if (PlayerData.Inventory.Count > 0)
            {
                if (Player != null && Player.CarriedProp == null)
                {
                    if (!IsAwaiting && IsCurrentScene)
                    {
                        if (InputManager.DefaultPlayer.Mouse.VirtualPosition.Y > 130)
                        {
                            InteractionContext.HeldItem = null;
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
            // LastBonfire
            if (LastBonfire != null)
                output.WriteAttributeString(nameof(LastBonfire), LastBonfire.Name);

            // DisplayHPMeter
            output.WriteAttributeString(nameof(DisplayHPMeter), XmlConvert.ToString(DisplayHPMeter));

            // Grace
            output.WriteAttributeString(nameof(PlayerData.Grace), XmlConvert.ToString(PlayerData.Grace));

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

            // UnlockedDefinitions
            output.WriteAttributeString("UnlockedDefinitions", string.Join(",", unlockedDefinitions));

            // Dropped key items
            var keyItems = new List<string>();
            foreach (var room in Entities.OfType<GameRoom>())
            {
                // Orbs
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

        // ApplyDeath
        [ScriptMethod]
        public void ApplyDeath()
        {
            var allRooms = new List<GameRoom>(Entities.OfType<GameRoom>());
            foreach (var room in allRooms)
            {
                room.Cleanup();
            }

            PlayerData.Inventory.Clear();
            PlayerData.Grace = 0;
        }

        // DangerousTarget
        [ScriptProperty]
        public GameThing? DangerousTarget { get; set; }

        // DialogOptionId
        [ScriptProperty]
        public int DialogOptionId { get; set; }

        // DisplayHPMeter
        [ScriptProperty]
        public bool DisplayHPMeter { get; set; } = true;

        // Echo
        public void Echo(string text)
        {
            if (Game.SceneManager.CurrentScene is not EchoScene)
                Game.SceneManager.Push(echoScene);

            echoScene.Show(text);
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

        // GlobalLightScale
        [ScriptProperty]
        public Vector2 GlobalLightScale
        {
            get => Environment.GlobalLight.Scale;
            set => Environment.GlobalLight.Scale = value;
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
                if (Room != null)
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

        // IsUnlocked
        public bool IsUnlocked(Definition definition)
        {
            return unlockedDefinitions.Contains(definition.Name);
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

        // Narrate
        public void Narrate(string text, string soundName)
        {
            if (Game.SceneManager.CurrentScene is not NarrationScene)
                Game.SceneManager.Push(narrationScene);

            narrationScene.Show(text, soundName);
        }

        // NextRoom
        [ScriptProperty]
        public new GameRoom? NextRoom => (GameRoom?)base.NextRoom;

        // ObjectPools
        public ObjectPools ObjectPools { get; }

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

        // Random
        public Random Random { get; private set; } = new(0);

        // RenewSeed
        public void RenewSeed()
        {
            Seed = RandomNumberGenerator.GetInt32(int.MaxValue);
        }

        // RespawnWorld
        [ScriptMethod]
        public void RespawnWorld()
        {
            if (Player?.IsDead == true)
                Player.Reheal();

            if (LastBonfire != null)
            {
                LastBonfire.BeginRest(true);
                LastBonfire.EndRest();

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

                    HUD.RoomTitle.Show(Room);
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

        // ShowItemInfo
        public void ShowItemInfo(Item item)
        {
            itemInfoScene.Show(item);
            Game.SceneManager.Push(itemInfoScene);
        }

        // Unlock
        public void Unlock(Definition defintion)
        {
            unlockedDefinitions.Add(defintion.Name);
        }
    }
}