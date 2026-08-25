using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace Adberration
{
    /// <summary>
    /// Session
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public abstract class Session : Scene
    {
        #region Constants

        private const string ChildrenAttribute = "CH";
        private const string EntityElement = "E";
        private const string NameAttribute = "N";

        #endregion

        #region Private fields

        private readonly Stack<Script> awaitingScripts = new();
        private readonly Stack<Room> busyRooms = new();
        private readonly Stack<Script> busyRoomsScripts = new();
        private bool canRun;
        private readonly Dictionary<string, Entity> entities = [];
        private readonly NamedCollection<Entity> entityList = [];
        private string? musicTagRoomScope;
        private string? musicTagScriptScope;
        private long nextEntityId;
        private Script? outcomeScript;
        private bool pendingSave;
        private readonly List<Entity> persistentEntities = [];
        private readonly Dictionary<string, int> randomNumbers = [];
        private Room? startingRoom;

        #endregion

        // Static constructor
        static Session()
        {
            RegisterAotTypes();
        }

        #region Constructor

        // Constructor
        protected Session(AdventureGame game, PersistenceModel persistenceModel, string scriptLibraryPath, int saveFileNumber)
        {
            this.Game = game;
            this.ExclusiveDraw = true;
            this.PausePreviousScenes = true;
            this.SaveFileNumber = saveFileNumber;
            this.ScriptProcessor = new ScriptProcessor(this);
            this.ScriptLibrary = new ScriptLibrary(this, scriptLibraryPath);
            this.ScriptEnvironment = new ScriptEnvironment(this);
            this.Entities = new NamedReadOnlyCollection<Entity>(entityList);
            this.Camera = new Camera("Room") { CullingBoxScale = new Vector2(4) };
            this.PersistenceModel = persistenceModel;
            this.SaveFileName = saveFileNumber >= 0 ? SaveFile.EncodeName(saveFileNumber) : string.Empty;
        }

        #endregion

        #region Private members

        // AssertInitialized
        private void AssertInitialized()
        {
            if (State == GameSessionState.Uninitialized)
                throw new InvalidOperationException("Game session is not initialized.");
        }

        // BeginEnterRoomOutcome
        private void BeginEnterRoomOutcome()
        {
            if (Room == null || busyRooms.Contains(Room))
                return;

            if (!string.IsNullOrEmpty(Room.Name))
            {
                if (ScriptLibrary.FindScript(ScriptType.Enter, Room.Name) is Script script)
                {
                    AwaitScript(script);
                    busyRooms.Push(Room);
                    busyRoomsScripts.Push(script);
                }
            }
        }

        // DeserializeEntities
        private IEnumerable<Entity> DeserializeEntities(string value)
        {
            var names = value.Split(';');
            foreach (var name in names)
            {
                if (FindEntity(name) is Entity entity)
                    yield return entity;
            }
        }

        // EndOutcome
        private void EndOutcome()
        {
            var oldScript = outcomeScript;
            var oldTarget = OutcomeTarget;

            State = GameSessionState.Idle;
            outcomeScript = null;
            OutcomeTarget = null;

            if (oldScript != null && oldTarget != null)
                OnOutcomeCompleted(oldScript, oldTarget);
        }

        // ExitRoom
        private void ExitRoom(Room currentRoom, Room nextRoom)
        {
            this.NextRoom = nextRoom;

            currentRoom.Deactivate();
            OnExitRoom(currentRoom, nextRoom);

            if (musicTagRoomScope != null)
            {
                AudioManager.Music.PlayTag(musicTagRoomScope);
                musicTagRoomScope = null;
            }

            if (currentRoom.UnloadMode == UnloadMode.Automatic)
                currentRoom.Unload();
            else
                currentRoom.Pause();

            OnExitRoomCompleted(currentRoom, nextRoom);

            NextRoom = null;
            PreviousRoom = currentRoom;
            Room = null;
        }

        // ReadCore
        private Room? ReadCore(Stream input)
        {
            XmlDocument doc = new();
            doc.Load(input);

            string? musicSoundName = null;

            var sessionNode = doc.DocumentElement ?? throw new InvalidOperationException("Session node not found.");

            // Counters
            if (sessionNode.Attributes[GameSessionPersistenceAttributeName.Counters.ToString()]?.Value is string savedCounters)
            {
                var list = savedCounters.Split(',');
                for (var i = 0; i < list.Length; i++)
                {
                    var values = list[i].Split('=');
                    var name = values[0];
                    var value = XmlConvert.ToInt32(values[1]);

                    if (ScriptEnvironment.FindCounter(name) is Counter counter)
                        counter.Value = value;
                }
            }

            // Flags
            if (sessionNode.Attributes[GameSessionPersistenceAttributeName.Flags.ToString()]?.Value is string savedFlags)
            {
                var list = savedFlags.Split(',');
                for (var i = 0; i < list.Length; i++)
                {
                    var values = list[i].Split('=');
                    var name = values[0];
                    var value = XmlConvert.ToBoolean(values[1]);

                    if (ScriptEnvironment.FindFlag(name) is Flag flag)
                        flag.Value = value;
                }
            }

            // AllowSaving
            if (sessionNode.Attributes[GameSessionPersistenceAttributeName.AllowSaving.ToString()]?.Value is string allowSaving)
                AllowSaving = XmlConvert.ToBoolean(allowSaving);

            // Current Room
            Room? result = null;
            if (sessionNode.Attributes[GameSessionPersistenceAttributeName.Room.ToString()]?.Value is string roomName)
                result = FindEntity<Room>(roomName);

            // MusicSoundName
            if (sessionNode.Attributes[GameSessionPersistenceAttributeName.MusicSoundName.ToString()]?.Value is string musicSoundNameString)
                musicSoundName = musicSoundNameString;

            // MusicTag
            if (sessionNode.Attributes[GameSessionPersistenceAttributeName.MusicTag.ToString()]?.Value is string musicTag)
                AudioManager.Music.PlayTag(musicTag, 1000);

            // MusicTagRoomScope
            if (sessionNode.Attributes[GameSessionPersistenceAttributeName.MusicTagRoomScope.ToString()]?.Value is string musicTagRoomScope)
                this.musicTagRoomScope = musicTagRoomScope;

            // Previous Room
            if (sessionNode.Attributes[GameSessionPersistenceAttributeName.PreviousRoom.ToString()]?.Value is string previousRoomValue)
                PreviousRoom = FindEntity<Room>(previousRoomValue);

            // PlayTime
            if (sessionNode.Attributes[GameSessionPersistenceAttributeName.PlayTime.ToString()]?.Value is string playTimeValue)
                PlayTime = XmlConvert.ToTimeSpan(playTimeValue);

            // Chapter
            if (sessionNode.Attributes[GameSessionPersistenceAttributeName.Chapter.ToString()]?.Value is string chapterValue)
                Chapter = XmlConvert.ToInt32(chapterValue);

            // Difficulty
            if (sessionNode.Attributes[GameSessionPersistenceAttributeName.Difficulty.ToString()]?.Value is string difficultyValue)
            {
                if (int.TryParse(difficultyValue, out var difficultyNumber))
                    Difficulty = difficultyNumber;
            }

            // Progress
            if (sessionNode.Attributes[GameSessionPersistenceAttributeName.Progress.ToString()]?.Value is string progressValue)
                Progress = XmlConvert.ToInt32(progressValue);

            OnRead(sessionNode);

            // Step 2: Collect saved entities
            HashSet<Entity> existingEntities = [];
            Dictionary<string, string> childrenInfo = [];
            var entities = doc.GetElementsByTagName(EntityElement);
            foreach (XmlNode? entityElement in entities)
            {
                var attributes = entityElement?.Attributes;
                if (attributes == null)
                    continue;

                if (attributes[NameAttribute]?.Value is string name)
                {
                    var children = attributes[ChildrenAttribute]?.Value;

                    // If entity is declared...
                    if (FindEntity(name) is Entity targetEntity)
                    {
                        existingEntities.Add(targetEntity);
                        if (children != null)
                            childrenInfo.Add(name, children);
                    }
                }
            }

            // Clear children. New and transient entities are preserved.
            foreach (var entity in existingEntities)
            {
                for (var i = entity.Children.Count - 1; i >= 0; i--)
                {
                    if (existingEntities.Contains(entity.Children[i]) && entity.Children[i].Persistent)
                        entity.Children.Remove(entity.Children[i]);
                }
            }

            // Step 3: Recreate Parent-Child relationship
            foreach (var keyValue in childrenInfo)
            {
                var parent = FindEntity(keyValue.Key);

                if (parent != null)
                {
                    foreach (var child in DeserializeEntities(keyValue.Value))
                    {
                        if (child.Persistent && child is Thing thing)
                            parent.Children.Add(thing);
                    }
                }
            }

            // Step 4: Deserialize
            foreach (XmlNode? entityElement in entities)
            {
                if (entityElement?.Attributes?[NameAttribute]?.Value is string name)
                {
                    if (FindEntity(name) is Entity targetEntity)
                    {
                        // Properties
                        if (targetEntity.Persistent)
                        {
                            PersistenceModel.Deserialize(targetEntity, entityElement);
                            targetEntity.Read(entityElement);
                        }
                    }
                }
            }

            OnReadCompleted(sessionNode);

            if (!string.IsNullOrWhiteSpace(musicSoundName))
                AudioManager.Music.Play(musicSoundName, true);

            return result;
        }

        // RegisterStatements
        private static void RegisterAotTypes()
        {
            AotTypeRegistry.Register(ScriptSyntax.ProperyReference, typeof(SetPropertyCommand), CodingContext.Any);
            AotTypeRegistry.Register("animate-opacity", typeof(AnimateOpacityCommand));
            AotTypeRegistry.Register("animation", typeof(AnimationCommand), CodingContext.EntityDeclaration);
            AotTypeRegistry.Register("await-animation", typeof(AwaitAnimationCommand));
            AotTypeRegistry.Register("await-camera", typeof(AwaitCameraCommand));
            AotTypeRegistry.Register("await", typeof(AwaitCommand));
            AotTypeRegistry.Register("await-enter-room", typeof(AwaitEnterRoomCommand));
            AotTypeRegistry.Register("await-move", typeof(AwaitMoveCommand));
            AotTypeRegistry.Register("await-music", typeof(AwaitMusicCommand));
            AotTypeRegistry.Register("await-opacity-tween", typeof(AwaitOpacityTweenCommand), CodingContext.Any);
            AotTypeRegistry.Register("await-routine", typeof(AwaitRoutineCommand));
            AotTypeRegistry.Register("await-script", typeof(AwaitScriptCommand));
            AotTypeRegistry.Register("await-session-scene", typeof(AwaitSessionSceneCommand));
            AotTypeRegistry.Register("await-shake", typeof(AwaitShakeCommand));
            AotTypeRegistry.Register("await-sound", typeof(AwaitSoundCommand));
            AotTypeRegistry.Register("await-sync-scripts", typeof(AwaitSyncScriptsCommand));
            AotTypeRegistry.Register("await-this-routine", typeof(AwaitThisRountineCommand));
            AotTypeRegistry.Register("await-transition", typeof(AwaitTransitionCommand));
            AotTypeRegistry.Register("change-sound-settings", typeof(ChangeSoundSettingsCommand));
            AotTypeRegistry.Register("color-tween", typeof(ColorTweenCommand), CodingContext.Any);
            AotTypeRegistry.Register("const", typeof(ConstCommand), CodingContext.Declaration);
            AotTypeRegistry.Register(ScriptSyntax.MethodReference, typeof(CallMethodCommand), CodingContext.Any);
            AotTypeRegistry.Register("else", typeof(ElseStatement));
            AotTypeRegistry.Register("endif", typeof(EndifStatement));
            AotTypeRegistry.Register("flip", typeof(FlipCommand));
            AotTypeRegistry.Register("goto-frame-index", typeof(GoToFrameIndexCommand));
            AotTypeRegistry.Register("goto-frame-position", typeof(GoToFramePositionCommand));
            AotTypeRegistry.Register("if-counter", typeof(IfCounterStatement));
            AotTypeRegistry.Register("if-entity", typeof(IfEntityStatement));
            AotTypeRegistry.Register("if-entity-type", typeof(IfEntityTypeStatement));
            AotTypeRegistry.Register("if-flag", typeof(IfFlagStatement));
            AotTypeRegistry.Register("if-parent", typeof(IfParentStatement));
            AotTypeRegistry.Register("if-random-number", typeof(IfRandomNumberStatement));
            AotTypeRegistry.Register("if-roll", typeof(IfRollStatement));
            AotTypeRegistry.Register("if-routine-running", typeof(IfRoutineRunningStatement));
            AotTypeRegistry.Register("if", typeof(IfCoreStatement));
            AotTypeRegistry.Register("restart", typeof(RestartStatement));
            AotTypeRegistry.Register("return", typeof(ReturnStatement));
            AotTypeRegistry.Register(ScriptSyntax.CloneKeyword, typeof(CloneCommand), CodingContext.Instantiation);
            AotTypeRegistry.Register("counter", typeof(CounterCommand), CodingContext.Declaration);
            AotTypeRegistry.Register("decrement-counter", typeof(DecrementCounterCommand));
            AotTypeRegistry.Register("export-localizable-texts", typeof(ExportLocalizableTextsCommand));
            AotTypeRegistry.Register("fade-sound", typeof(FadeSoundCommand));
            AotTypeRegistry.Register("flag", typeof(FlagCommand), CodingContext.Declaration);
            AotTypeRegistry.Register("focus", typeof(FocusCommand));
            AotTypeRegistry.Register("focus-xy", typeof(FocusXYCommand));
            AotTypeRegistry.Register("follow", typeof(FollowCommand));
            AotTypeRegistry.Register("frame", typeof(FrameCommand), CodingContext.EntityDeclaration);
            AotTypeRegistry.Register("generate-random-number", typeof(GenerateRandomNumberCommand));
            AotTypeRegistry.Register("increment-counter", typeof(IncrementCounterCommand));
            AotTypeRegistry.Register("move", typeof(MoveCommand));
            AotTypeRegistry.Register("opacity-tween", typeof(OpacityTweenCommand), CodingContext.Any);
            AotTypeRegistry.Register("pause-routine", typeof(PauseRoutineCommand));
            AotTypeRegistry.Register("pause-sound", typeof(PauseSoundCommand));
            AotTypeRegistry.Register("play-animation", typeof(PlayAnimationCommand));
            AotTypeRegistry.Register("play-music", typeof(PlayMusicCommand));
            AotTypeRegistry.Register("play-sound", typeof(PlaySoundCommand));
            AotTypeRegistry.Register("play-music-tag", typeof(PlayMusicTagCommand));
            AotTypeRegistry.Register("pop-scene", typeof(PopSceneCommand));
            AotTypeRegistry.Register("position-tween", typeof(PositionTweenCommand), CodingContext.Any);
            AotTypeRegistry.Register("position-x-tween", typeof(XTweenCommand), CodingContext.Any);
            AotTypeRegistry.Register("position-y-tween", typeof(YTweenCommand), CodingContext.Any);
            AotTypeRegistry.Register("put", typeof(PutCommand), CodingContext.Any);
            AotTypeRegistry.Register("random-position", typeof(RandomPositionCommand));
            AotTypeRegistry.Register("reload", typeof(ReloadCommand));
            AotTypeRegistry.Register("reset-camera", typeof(ResetCameraCommand));
            AotTypeRegistry.Register("reset-tweens", typeof(ResetTweensCommand), CodingContext.Any);
            AotTypeRegistry.Register("resume-routine", typeof(ResumeRoutineCommand));
            AotTypeRegistry.Register("resume-sound", typeof(ResumeSoundCommand));
            AotTypeRegistry.Register("rotation-tween", typeof(RotationTweenCommand), CodingContext.Any);
            AotTypeRegistry.Register("save-game", typeof(SaveGameCommand));
            AotTypeRegistry.Register("scale-tween", typeof(ScaleTweenCommand), CodingContext.Any);
            AotTypeRegistry.Register("set-achievement", typeof(SetAchievementCommand));
            AotTypeRegistry.Register("set-counter", typeof(SetCounterCommand));
            AotTypeRegistry.Register("set-flag", typeof(SetFlagCommand));
            AotTypeRegistry.Register("set-music-tag", typeof(SetMusicTagCommand));
            AotTypeRegistry.Register("shake", typeof(ShakeCommand));
            AotTypeRegistry.Register("shake-horizontally", typeof(ShakeHorizontallyCommand));
            AotTypeRegistry.Register("shake-vertically", typeof(ShakeVerticallyCommand));
            AotTypeRegistry.Register("start-routine", typeof(StartRoutineCommand));
            AotTypeRegistry.Register("stop-animation", typeof(StopAnimationCommand));
            AotTypeRegistry.Register("stop-following", typeof(StopFollowingCommand));
            AotTypeRegistry.Register("stop-moving", typeof(StopMovingCommand));
            AotTypeRegistry.Register("stop-music", typeof(StopMusicCommand));
            AotTypeRegistry.Register("stop-routine", typeof(StopRoutineCommand));
            AotTypeRegistry.Register("stop-shaking", typeof(StopShakingCommand));
            AotTypeRegistry.Register("stop-sound", typeof(StopSoundCommand));
            AotTypeRegistry.Register("stop-vibration", typeof(StopVibrationCommand));
            AotTypeRegistry.Register("suspend-input", typeof(SuspendInputCommand));
            AotTypeRegistry.Register("suspend-vibration", typeof(SuspendVibrationCommand));
            AotTypeRegistry.Register("toggle-flag", typeof(ToggleFlagCommand));
            AotTypeRegistry.Register("transition", typeof(TransitionCommand));
            AotTypeRegistry.Register("unparent", typeof(UnparentCommand));
            AotTypeRegistry.Register("vibrate", typeof(VibrateCommand));
            AotTypeRegistry.Register("zoom", typeof(ZoomCommand));
        }

        // SerializeEntities
        private static string SerializeEntities(Entity.ChildCollection things)
        {
            List<string> list = [];

            for (var i = 0; i < things.Count; i++)
            {
                if (things[i].Persistent && things[i].InstanceKind != EntityInstanceKind.Anonymous)
                    list.Add(things[i].Name);
            }

            return string.Join(";", list);
        }

        // UpdateScripts
        private void UpdateScripts()
        {
            if (awaitingScripts.Count > 0)
            {
                // Remove all executed scripts
                while (awaitingScripts.Count > 0)
                {
                    if (!ScriptProcessor.IsExecutingScript(awaitingScripts.Peek()))
                    {
                        var finishedScript = awaitingScripts.Pop();
                        if (busyRoomsScripts.Count > 0 && busyRoomsScripts.Peek() == finishedScript)
                        {
                            busyRooms.Pop();
                            busyRoomsScripts.Pop();

                            if (busyRooms.Count == 0)
                                IsFirstRoomSinceLoad = false;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if (awaitingScripts.Count == 0)
                {
                    if (musicTagScriptScope != null)
                    {
                        AudioManager.Music.PlayTag(musicTagScriptScope);
                        musicTagScriptScope = null;
                    }

                    OnAwaitCompleted();
                }
            }

            if (!IsAwaiting)
            {
                if (outcomeScript != null && outcomeScript.IsCompleted)
                    EndOutcome();

                if (pendingSave)
                    Save();

                State = GameSessionState.Idle;
            }
        }

        // WriteCore
        private void WriteCore(XmlWriter output)
        {
            if (Room == null)
                throw new InvalidOperationException("There is no current room.");

            output.WriteStartDocument();

            output.WriteStartElement("GameSession");

            // Counters
            var counters = ScriptEnvironment.GetCounters();
            if (counters.Length > 0)
            {
                List<string> counterList = [];
                foreach (var counter in counters)
                {
                    if (counter.Persistent)
                        counterList.Add(counter.Name + "=" + XmlConvert.ToString(counter.Value));
                }
                output.WriteAttributeString(GameSessionPersistenceAttributeName.Counters.ToString(), string.Join(",", counterList));
            }

            // Flags
            var flags = ScriptEnvironment.GetFlags();
            if (flags.Length > 0)
            {
                List<string> flagList = [];
                foreach (var flag in flags)
                {
                    if (flag.Persistent)
                        flagList.Add(flag.Name + "=" + XmlConvert.ToString(flag.Value));
                }
                output.WriteAttributeString(GameSessionPersistenceAttributeName.Flags.ToString(), string.Join(",", flagList));
            }

            // AllowSaving
            output.WriteAttributeString(GameSessionPersistenceAttributeName.AllowSaving.ToString(), XmlConvert.ToString(AllowSaving));

            // Room
            if (!string.IsNullOrWhiteSpace(Room.Name))
                output.WriteAttributeString(GameSessionPersistenceAttributeName.Room.ToString(), Room.Name);

            // Previous Room
            if (PreviousRoom != null)
                output.WriteAttributeString(GameSessionPersistenceAttributeName.PreviousRoom.ToString(), PreviousRoom.Name);

            // LastSaved
            output.WriteAttributeString(GameSessionPersistenceAttributeName.LastSaved.ToString(), XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Local));

            // Demo
            output.WriteAttributeString(GameSessionPersistenceAttributeName.Demo.ToString(), XmlConvert.ToString(IsDemo));

            if (AudioManager.Music.IsLooped && AudioManager.Music.CurrentSoundName.Length > 0)
                output.WriteAttributeString(GameSessionPersistenceAttributeName.MusicSoundName.ToString(), AudioManager.Music.CurrentSoundName);

            // MusicTag
            output.WriteAttributeString(GameSessionPersistenceAttributeName.MusicTag.ToString(), AudioManager.Music.CurrentTag);

            // MusicTagRoomScope
            if (musicTagRoomScope != null)
                output.WriteAttributeString(GameSessionPersistenceAttributeName.MusicTagRoomScope.ToString(), musicTagRoomScope);

            // PlayTime
            output.WriteAttributeString(GameSessionPersistenceAttributeName.PlayTime.ToString(), XmlConvert.ToString(PlayTime));

            // Chapter
            output.WriteAttributeString(GameSessionPersistenceAttributeName.Chapter.ToString(), XmlConvert.ToString(Chapter));

            // Difficulty
            output.WriteAttributeString(GameSessionPersistenceAttributeName.Difficulty.ToString(), XmlConvert.ToString(Difficulty));

            // Progress
            output.WriteAttributeString(GameSessionPersistenceAttributeName.Progress.ToString(), XmlConvert.ToString(Progress));

            // Version
            output.WriteAttributeString(GameSessionPersistenceAttributeName.Version.ToString(), PersistenceModel.Version);

            OnWrite(output);

            WriteEntities(output);

            output.WriteEndElement();

            OnWriteCompleted(output);
        }

        // WriteEntities
        private void WriteEntities(XmlWriter output)
        {
            output.WriteStartElement("Entities");

            foreach (var entity in persistentEntities)
            {
                output.WriteStartElement(EntityElement);

                // Name
                output.WriteAttributeString(NameAttribute, entity.Name);

                // Children
                if (entity.Children.Count > 0)
                {
                    var value = SerializeEntities(entity.Children);
                    if (value.Length > 0)
                        output.WriteAttributeString(ChildrenAttribute, value);
                }

                // Serialize properties
                PersistenceModel.Serialize(entity, output);

                entity.Write(output);

                output.WriteEndElement();
            }

            output.WriteEndElement();
        }

        // WriteSaveFileTask
        private void WriteSaveFileTask(string fileName, MemoryStream sessionData)
        {
            using (var input = Game.PlatformBridge.FileSystem.EncryptSaveFiles ? XOREncryptor.AsStream(sessionData, XOREncryptor.EncryptionKey) : sessionData)
            {
                SaveFileHeader header = new(fileName, PersistenceModel.Version, DateTime.Now, IsDemo, Chapter, PlayTime, Difficulty, Progress);
                Game.PlatformBridge.FileSystem.WriteSaveFile(header, input);
            }

            sessionData.Dispose();
            IsSaving = false;

            OnSaveCompleted();
        }

        // WriteSessionData
        private MemoryStream WriteSessionData()
        {
            MemoryStream output = new();

            using (XmlWriter w = XmlWriter.Create(output, new XmlWriterSettings() { CloseOutput = false }))
            {
                WriteCore(w);
                output.Flush();
            }

            output.Position = 0;

            return output;
        }

        #endregion

        #region Protected members

        // CanHandleRoomInput
        protected virtual bool CanHandleRoomInput => true;

        // Dispose
        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
            {
                foreach (var entity in Entities)
                {
                    if (entity is Room room)
                        room.Unload();
                }

                CleanUpRuntimeEntities();
                IsRunning = false;
                OnShutDown();
            }

            IsDisposed = true;

            base.Dispose(disposing);
        }

        // OnAwait
        protected virtual void OnAwait()
        {
        }

        // OnAwaitCompleted
        protected virtual void OnAwaitCompleted()
        {
        }

        // OnContinuousUpdate
        protected override void OnContinuousUpdate(GameTime gameTime)
        {
            base.OnContinuousUpdate(gameTime);
            PlayTime += gameTime.ElapsedGameTime;
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!IsRunning || IsDisposed)
                return;

            Room?.Draw(gameTime);
        }

        // OnEnterRoom
        protected virtual void OnEnterRoom(Room room)
        {
        }

        // OnExitRoom
        protected virtual void OnExitRoom(Room currentRoom, Room nextRoom)
        {
        }

        // OnExitRoomCompleted
        protected virtual void OnExitRoomCompleted(Room currentRoom, Room nextRoom)
        {
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput()
        {
            if (IsRunning && !IsDisposed && (State == GameSessionState.Idle || IsAwaiting))
            {
                if (Room != null && CanHandleRoomInput)
                    return Room.HandleInput();
                else
                    return base.OnHandleInput();
            }

            return HandleInputResult.Unhandled;
        }

        // OnOutcome
        protected virtual void OnOutcome(Thing target)
        {
        }

        // OnOutcomeCompleted
        protected virtual void OnOutcomeCompleted(Script script, Thing target)
        {
        }

        // OnRead
        protected virtual void OnRead(XmlNode sessionNode)
        {
        }

        // OnReadCompleted
        protected virtual void OnReadCompleted(XmlNode sessionNode)
        {
        }

        // OnRun
        protected virtual void OnRun()
        {
        }

        // OnRunCompleted
        protected virtual void OnRunCompleted()
        {
        }

        // OnSave
        protected virtual void OnSave()
        {
        }

        // OnSaveCompleted
        protected virtual void OnSaveCompleted()
        {
        }

        // OnScriptLibraryLoaded
        protected virtual void OnScriptLibraryLoaded()
        {
        }

        // OnShutDown
        protected virtual void OnShutDown()
        {
        }

        // OnStarted
        protected virtual void OnStarted()
        {
        }

        // OnStarting
        protected virtual void OnStarting()
        {
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (!IsRunning || IsDisposed)
                return;

            ScriptProcessor.Update(gameTime);

            Camera.Update(gameTime);
            Room?.Update(gameTime);

            UpdateScripts();

            Game.PlatformBridge.Update(gameTime);
        }

        // OnWrite
        protected virtual void OnWrite(XmlWriter output)
        {
        }

        // OnWriteCompleted
        protected virtual void OnWriteCompleted(XmlWriter output)
        {
        }

        #endregion

        #region Internal members

        // AnimationManager
        internal AnimationManager AnimationManager { get; } = new();

        // NextEntityId
        internal long NextEntityId()
        {
            return ++nextEntityId;
        }

        // RegisterEntity
        internal void RegisterEntity(Entity entity)
        {
            if (State != GameSessionState.LoadingScripts && !ScriptEnvironment.IsCreatingClone)
                throw new InvalidOperationException("This action can be performed during the initialization only.");

            if (!ScriptEnvironment.IsCreatingClone)
            {
                NameValidator.CheckName(entity.Name);

                if (ScriptEnvironment.IsReservedWord(entity.Name))
                    throw new InvalidOperationException($"The name '{entity.Name}' is a reserved word.");
            }

            entities.Add(entity.Name, entity);
            entityList.Add(entity);
        }

        // ScriptEnvironment
        internal ScriptEnvironment ScriptEnvironment { get; private set; }

        #endregion

        // AllowSaving
        [ScriptProperty]
        public bool AllowSaving { get; set; } = true;

        // AwaitRoutine
        public bool AwaitRoutine(string name)
        {
            if (ScriptLibrary.FindRoutine(name) is Script script)
            {
                AwaitScript(script);
                return true;
            }

            return false;
        }

        // AwaitingScript
        public Script? AwaitingScript
        {
            get
            {
                if (awaitingScripts.Count == 0)
                    return null;

                var script = awaitingScripts.Peek();
                if (script.IsCompleted)
                {
                    UpdateScripts();
                    return awaitingScripts.Count > 0 ? awaitingScripts.Peek() : null;
                }
                else
                {
                    return script;
                }
            }
        }

        // AwaitScript
        public void AwaitScript(Script script)
        {
            var containsScript = awaitingScripts.Contains(script);

            // Case 1: Script is not in sync list
            // Case 2: Script is in the sync list but completed
            // In any case it must be started
            if (!containsScript || script.IsCompleted)
            {
                if (!containsScript)
                {
                    awaitingScripts.Push(script);
                    if (awaitingScripts.Count == 1)
                        OnAwait();
                }

                if (!ScriptProcessor.IsExecutingScript(script))
                    ScriptProcessor.StartScript(script);
            }

            if (IsAwaiting)
                State = GameSessionState.AwaitingScripts;
        }

        // BeginOutcome
        public void BeginOutcome(Script script, Thing target)
        {
            AssertInitialized();

            CodeContract.NotDisposed(nameof(Session), IsDisposed);

            if (State != GameSessionState.Idle)
                throw new InvalidOperationException("Cannot start an outcome while not in idle state.");

            if (ScriptProcessor.IsExecutingScript(script))
                throw new InvalidOperationException($"The script '{script.Name}' is being executed.");

            outcomeScript = script;
            OutcomeTarget = target;

            OnOutcome(OutcomeTarget);

            if (outcomeScript.HasCapability(ScriptCapability.SetTargetEntity) && !string.IsNullOrWhiteSpace(OutcomeTarget.Name))
                outcomeScript.SetTargetEntity(OutcomeTarget.Name);

            AwaitScript(outcomeScript);
        }

        // Camera
        public Camera Camera { get; }

        // CanSave
        public virtual bool CanSave => AllowSaving && !IsSaving && !IsAwaiting && SaveFileNumber >= 0 && Room?.CanSave == true;

        // Chapter
        [ScriptProperty]
        public int Chapter { get; set; }

        // CleanUpRuntimeEntities
        public void CleanUpRuntimeEntities()
        {
            var runtimeEntities = new List<Entity>();

            for (var i = 0; i < entityList.Count; i++)
            {
                if (entityList[i].InstanceKind == EntityInstanceKind.RuntimeClone)
                    runtimeEntities.Add(entityList[i]);
            }

            for (var i = 0; i < runtimeEntities.Count; i++)
            {
                runtimeEntities[i].Unparent();
                entities.Remove(runtimeEntities[i].Name);
                entityList.Remove(runtimeEntities[i]);
            }
        }

        // CreateFlagCondition
        public FlagCondition CreateFlagCondition(IList<string> flags)
        {
            List<FlagExpression> expressions = [];

            for (var i = 0; i < flags.Count; i++)
            {
                var negate = flags[i].StartsWith(ScriptSyntax.LogicalNegation, StringComparison.Ordinal);
                var flagName = negate ? flags[i].Substring(1) : flags[i];

                if (ScriptEnvironment.FindFlag(flagName) is Flag flag)
                    expressions.Add(new FlagExpression(flag, negate));
                else
                    throw new InvalidOperationException("Flag not found during evaluation.");
            }

            return new FlagCondition(expressions);
        }

        // CreateThingClone
        public Thing CreateThingClone(string declaredName, string instanceName)
        {
            return ScriptEnvironment.CreateThingClone(declaredName, instanceName, false);
        }

        // CurrentMusicName
        [ScriptProperty]
        public string CurrentMusicName => AudioManager.Music.CurrentSoundName;

        // DebugMode
        [ScriptProperty]
        public static bool DebugMode => EngendroGame.DebugMode;

        // Difficulty
        [ScriptProperty]
        public int Difficulty { get; set; }

        // EnterRoom
        public bool EnterRoom(Room nextRoom)
        {
            AssertInitialized();

            CodeContract.NotDisposed(nameof(Session), IsDisposed);

            // Same room
            if (IsEnteringRoom(nextRoom))
                return false;

            // Exit current room
            if (Room != null)
                ExitRoom(this.Room, nextRoom);

            // New room
            this.Room = nextRoom;

            nextRoom.Load();
            nextRoom.Activate();

            OnEnterRoom(nextRoom);
            BeginEnterRoomOutcome();

            if (busyRooms.Count == 0)
                IsFirstRoomSinceLoad = false;

            return true;
        }

        // Entities
        public NamedReadOnlyCollection<Entity> Entities { get; }

        // FindEntity
        public Entity? FindEntity(string name)
        {
            AssertInitialized();
            CodeContract.NotDisposed(nameof(Session), IsDisposed);

            if (string.IsNullOrEmpty(name))
                return null;
            else if (entities.TryGetValue(name, out var value))
                return value;
            else
                return null;
        }

        // FindEntity
        public T? FindEntity<T>(string name) where T : Entity
        {
            return FindEntity(name) as T;
        }

        // Game
        public new AdventureGame Game { get; }

        // GenerateRandomNumber
        public int GenerateRandomNumber(string name, Int32Range range)
        {
            var result = range.GetRandomValue(Random.Shared);
            randomNumbers[name] = result;
            return result;
        }

        // GetEntity
        public Entity GetEntity(string name)
        {
            var result = FindEntity(name);
            if (result == null)
                throw new InvalidOperationException($"Entity '{name}' does not exist.");
            else
                return result;
        }

        // GetEntity
        public T GetEntity<T>(string name) where T : Entity
        {
            if (FindEntity(name) is not T result)
                throw new InvalidOperationException($"Entity '{name}' does not exist.");
            else
                return result;
        }
        // GetRandomNumber
        public int GetRandomNumber(string name)
        {
            return randomNumbers[name];
        }

        // InterruptAwaitingScript
        public bool InterruptAwaitingScript()
        {
            var result = false;

            while (awaitingScripts.Count > 0)
            {
                if (awaitingScripts.Peek() is Script script)
                {
                    ScriptProcessor.StopScript(script);
                    awaitingScripts.Pop();
                    result = true;
                }
            }

            if (result)
                UpdateScripts();

            return result;
        }

        // IsAwaiting
        [ScriptProperty]
        public bool IsAwaiting => awaitingScripts.Count > 0;

        // IsAwaitingScript
        public bool IsAwaitingScript(Script script)
        {
            return awaitingScripts.Count != 0 && awaitingScripts.Contains(script);
        }

        // IsDemo
        [ScriptProperty]
        public bool IsDemo => Game.IsDemo;

        // IsDisposed
        public bool IsDisposed { get; private set; }

        // IsEnteringRoom
        public bool IsEnteringRoom(Room room)
        {
            return busyRooms.Contains(room);
        }

        // IsFirstRoomSinceLoad
        [ScriptProperty]
        public bool IsFirstRoomSinceLoad { get; private set; }

        // IsInitializing
        public bool IsInitializing { get; private set; }

        // IsNewSession
        [ScriptProperty]
        public bool IsNewSession { get; private set; }

        // IsOutcomeInProgress
        [ScriptProperty]
        public bool IsOutcomeInProgress => outcomeScript != null;

        // IsRunning
        public bool IsRunning { get; private set; }

        // IsSaving
        public bool IsSaving { get; private set; }

        // LocalizationSource
        [ScriptProperty]
        public static LocalizationSource LocalizationSource { get; set; }

        // MusicState
        [ScriptProperty]
        public SoundState MusicState => AudioManager.Music.State;

        // NextRoom
        [ScriptProperty]
        public virtual Room? NextRoom { get; private set; }

        // OutcomeTarget
        [ScriptProperty(CodingContext.Execution)]
        public virtual Thing? OutcomeTarget { get; private set; }

        // PersistenceModel
        public PersistenceModel PersistenceModel { get; protected set; }

        // PlayMusicTag
        public void PlayMusicTag(string tag, MusicTagScope scope, int fade)
        {
            if (scope == MusicTagScope.Script)
            {
                musicTagScriptScope = AudioManager.Music.CurrentTag;
            }
            else if (scope == MusicTagScope.Room)
            {
                if (musicTagRoomScope != null)
                    return;

                musicTagRoomScope = AudioManager.Music.CurrentTag;
            }

            AudioManager.Music.PlayTag(tag, fade);
        }

        // PlayTime
        public TimeSpan PlayTime { get; private set; }

        // PreviousRoom
        [ScriptProperty]
        public virtual Room? PreviousRoom { get; private set; }

        // Progress
        [ScriptProperty]
        public int Progress { get; set; }

        // Room
        [ScriptProperty]
        public virtual Room? Room { get; private set; }

        // Run
        public void Run()
        {
            if (!canRun)
                throw new InvalidOperationException();

            if (IsRunning)
                throw new InvalidOperationException("Game session is already running.");

            // if (startingRoom == null && !IsNewSession)
            //    throw new InvalidOperationException("There is no starting room.");

            IsRunning = true;

            OnRun();

            if (IsNewSession)
            {
                // New session script
                if (ScriptLibrary.FindScript(ScriptType.NewSession.ToString()) is Script newSessionScript)
                    AwaitScript(newSessionScript);
                else
                    throw new InvalidOperationException("Undefined 'NewSession' script.");
            }
            else if (startingRoom != null)
            {
                EnterRoom(startingRoom);
            }

            OnRunCompleted();
        }

        // Save
        public bool Save()
        {
            AssertInitialized();
            CodeContract.NotDisposed(nameof(Session), IsDisposed);

            if (!IsRunning)
                throw new InvalidOperationException("Session is not running.");

            if (SaveFileNumber < 0)
                throw new InvalidOperationException("A slot number is required.");

            if (IsSaving)
                throw new InvalidOperationException("A save operation is already in progress.");

            if (!CanSave)
            {
                if (AllowSaving)
                    pendingSave = true;

                return false;
            }

            IsSaving = true;

            OnSave();
            var sessionData = WriteSessionData();
            Task.Run(() => WriteSaveFileTask(SaveFile.EncodeName(SaveFileNumber), sessionData));
            IsNewSession = false;
            Game.ResetElapsedTime();
            pendingSave = false;

            return true;
        }

        // SaveFileName
        public string SaveFileName { get; }

        // SaveFileNumber
        public int SaveFileNumber { get; }

        // ScriptLibrary
        public ScriptLibrary ScriptLibrary { get; }

        // ScriptProcessor
        public ScriptProcessor ScriptProcessor { get; }

        // Start
        public void Start()
        {
            CodeContract.NotDisposed(nameof(Session), IsDisposed);

            OnStarting();

            ScriptEnvironment.Activate();

            if (State != GameSessionState.Uninitialized || IsInitializing)
                throw new InvalidOperationException();

            this.IsInitializing = true;
            this.IsNewSession = !Game.PlatformBridge.FileSystem.FileExists(SaveFileName);

            // Load scripts
            State = GameSessionState.LoadingScripts;
            ScriptLibrary.Load();
            State = GameSessionState.Idle;

            OnScriptLibraryLoaded();

            startingRoom = null;
            if (!IsNewSession)
            {
                State = GameSessionState.Loading;

                if (Game.PlatformBridge.FileSystem.ReadFile(SaveFileName) is Stream stm)
                {
                    using (stm)
                    {
                        using var input = !XOREncryptor.IsEncryptedXml(stm) ? stm : XOREncryptor.AsStream(stm, XOREncryptor.EncryptionKey);
                        startingRoom = ReadCore(input);
                    }
                }

                State = GameSessionState.Idle;
            }

            for (var i = 0; i < Entities.Count; i++)
            {
                Entities[i].Initialize();

                if (Entities[i].Persistent)
                    persistentEntities.Add(Entities[i]);
            }

            persistentEntities.Sort(new EntityComparer());
            IsInitializing = false;
            IsFirstRoomSinceLoad = true;

            OnStarted();

            canRun = true;
        }

        // State
        public GameSessionState State { get; private set; }

        // Viewport
        public RectangleF Viewport => Camera.VisibleBox;

        /// <summary>
        /// EntityComparer
        /// </summary>
        private sealed class EntityComparer : IComparer<Entity>
        {
            // Compare
            public int Compare(Entity? x, Entity? y)
            {
                var name1 = x?.Name;
                var name2 = y?.Name;

                if (name1 == null && name2 == null)
                    return 0;

                if (name1 == null)
                    return -1;

                if (name2 == null)
                    return 1;

                return name1.CompareTo(name2, StringComparison.InvariantCulture);
            }
        }
    }
}