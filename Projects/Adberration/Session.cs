using Adberration.Scripting;
using Adberration.Scripting.Core;
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
        private readonly NamedObjectCollection<Entity> entityList = [];
        private string? musicTagRoomScope;
        private string? musicTagScriptScope;
        private long nextEntityId;
        private Script? outcomeScript;
        private bool pendingSave;
        private readonly List<Entity> persistentEntities = [];
        private readonly Dictionary<string, int> randomNumbers = [];
        private Room? startingRoom;

        #endregion

        #region Constructor

        // Constructor
        protected Session(AdventureGame game, PersistenceModel persistenceModel, string scriptLibraryPath, int saveFileNumber)
            : base(game, SceneSettings.ExclusiveDraw | SceneSettings.PausePreviousScenes)
        {
            this.Game = game;
            this.SaveFileNumber = saveFileNumber;
            this.ScriptProcessor = new ScriptProcessor(this);
            this.ScriptLibrary = new ScriptLibrary(this, scriptLibraryPath);
            this.ScriptEnvironment = new ScriptEnvironment(this);
            this.Entities = new NamedObjectReadOnlyCollection<Entity>(entityList);
            this.Camera = new Camera(game, "Room") { CullingBoxScale = new Vector2(1.4f) };
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

            if (ScriptLibrary.GetScript(ScriptType.Enter, Room.Name) is Script script)
            {
                AwaitScript(script);
                busyRooms.Push(Room);
                busyRoomsScripts.Push(script);
            }
        }

        // EndOutcome
        private void EndOutcome()
        {
            State = GameSessionState.Idle;

            if (OutcomeTarget != null)
                OnOutcomeCompleted(OutcomeTarget);

            outcomeScript = null;
            OutcomeTarget = null;
        }

        // ExitRoom
        private void ExitRoom(Room currentRoom, Room nextRoom)
        {
            this.NextRoom = nextRoom;

            if (musicTagRoomScope != null)
            {
                AudioManager.Music.PlayTag(musicTagRoomScope);
                musicTagRoomScope = null;
            }

            OnExitRoom(currentRoom, nextRoom);
            currentRoom.Unload();

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

                    if (ScriptEnvironment.GetCounter(name) is Counter counter)
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

                    if (ScriptEnvironment.GetFlag(name) is Flag flag)
                        flag.Value = value;
                }
            }

            // AllowSaving
            if (sessionNode.Attributes[GameSessionPersistenceAttributeName.AllowSaving.ToString()]?.Value is string allowSaving)
                AllowSaving = XmlConvert.ToBoolean(allowSaving);

            // Current Room
            Room? result = null;
            if (sessionNode.Attributes[GameSessionPersistenceAttributeName.Room.ToString()]?.Value is string roomName)
                result = GetEntity<Room>(roomName);

            if (result == null)
                throw new InvalidOperationException("Save file has an undefined room.");

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
                PreviousRoom = GetEntity<Room>(previousRoomValue);

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
                    if (GetEntity(name) is Entity targetEntity)
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
                var parent = GetEntity(keyValue.Key);

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
                    if (GetEntity(name) is Entity targetEntity)
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

            // Current Room
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

        // DeserializeEntities
        protected IEnumerable<Entity> DeserializeEntities(string value)
        {
            var names = value.Split(';');
            foreach (var name in names)
            {
                if (GetEntity(name) is Entity entity)
                    yield return entity;
            }
        }

        // Dispose
        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
            {
                Room?.Unload();
                IsRunning = false;
            }

            IsDisposed = true;

            base.Dispose(disposing);
        }

        // ExtendScriptRegistry
        protected virtual void ExtendScriptRegistry(ScriptRegistry scriptRegistry)
        {
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

        // OnEnterRoomCompleted
        protected virtual void OnEnterRoomCompleted(Room room)
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
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (IsRunning && !IsDisposed && (State == GameSessionState.Idle || IsAwaiting))
            {
                if (Room != null && CanHandleRoomInput)
                    return Room.HandleInput(gameTime);
                else
                    return base.OnHandleInput(gameTime);
            }

            return HandleInputResult.Unhandled;
        }

        // OnInitializeEntities
        protected virtual void OnInitializeEntities()
        {
        }

        // OnOutcome
        protected virtual void OnOutcome(Thing target)
        {
        }

        // OnOutcomeCompleted
        protected virtual void OnOutcomeCompleted(Thing target)
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

        // OnShowSoundCaption
        protected virtual void OnShowSoundCaption(SoundInstance soundInstance)
        {
        }

        // OnStart
        protected virtual void OnStart()
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

        // SerializeEntities
        protected static string SerializeEntities(IList<Thing> things)
        {
            List<string> list = [];

            for (var i = 0; i < things.Count; i++)
            {
                if (things[i].Persistent && things[i].InstanceKind != InstanceKind.Anonymous)
                    list.Add(things[i].Name);
            }

            return string.Join(";", list);
        }

        #endregion

        #region Internal members

        // NextEntityId
        internal long NextEntityId() => ++nextEntityId;

        // RegisterEntity
        internal void RegisterEntity(Entity entity)
        {
            if (State != GameSessionState.LoadingScripts && !ScriptEnvironment.IsCreatingClone)
                throw new InvalidOperationException("This action can be performed during the initialization phase only.");

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
            if (ScriptLibrary.GetRoutine(name) is Script script)
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
                    return awaitingScripts.Peek();
                }
                else
                    return script;
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

            if (outcomeScript.HasCapability(ScriptCapability.SetTargetEntity))
                outcomeScript.SetTargetEntity(OutcomeTarget.Name);

            AwaitScript(outcomeScript);
        }

        // Camera
        public Camera Camera { get; }

        // CanSave
        public bool CanSave => AllowSaving && !IsSaving && SaveFileNumber >= 0 && Room != null && Room.CanSave && !IsAwaiting;

        // Chapter
        [ScriptProperty]
        public int Chapter { get; set; }

        // CleanUpRuntimeEntities
        public void CleanUpRuntimeEntities()
        {
            var runtimeEntities = new List<Entity>();

            for (var i = 0; i < entityList.Count; i++)
            {
                if (entityList[i].InstanceKind == InstanceKind.RuntimeClone)
                    runtimeEntities.Add(entityList[i]);
            }

            for (var i = 0; i < runtimeEntities.Count; i++)
            {
                runtimeEntities[i].Unparent();
                entities.Remove(runtimeEntities[i].Name);
                entityList.Remove(runtimeEntities[i]);
            }
        }

        // CreateRuntimeClone
        public Thing CreateRuntimeClone(string staticName, string instanceName) => ScriptEnvironment.CreateRuntimeClone(staticName, instanceName, false);

        // CreateFlagCondition
        public FlagCondition CreateFlagCondition(IList<string> flags)
        {
            List<FlagExpression> expressions = [];

            for (var i = 0; i < flags.Count; i++)
            {
                var negate = flags[i].StartsWith(ScriptSyntax.LogicalNegation, StringComparison.Ordinal);
                var flagName = negate ? flags[i].Substring(1) : flags[i];

                if (ScriptEnvironment.GetFlag(flagName) is Flag flag)
                    expressions.Add(new FlagExpression(flag, negate));
                else
                    throw new InvalidOperationException("Flag not found during evaluation.");
            }

            return new FlagCondition(expressions);
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

            // Exit room
            if (Room != null)
                ExitRoom(this.Room, nextRoom);

            // New room
            this.Room = nextRoom;

            // EnterRoom event (Global)
            if (ScriptLibrary.GetScript(ScriptType.EnterRoom.ToString()) is Script script)
                ScriptProcessor.RunScript(script);

            nextRoom.Load();

            OnEnterRoom(nextRoom);
            OnEnterRoomCompleted(nextRoom);
            BeginEnterRoomOutcome();

            if (busyRooms.Count == 0)
                IsFirstRoomSinceLoad = false;

            return true;
        }

        // Entities
        public NamedObjectReadOnlyCollection<Entity> Entities { get; }

        // Game
        public new AdventureGame Game { get; }

        // GenerateRandomNumber
        public int GenerateRandomNumber(string name, Int32Range range)
        {
            var result = range.Random();
            randomNumbers[name] = result;
            return result;
        }

        // GetEntity
        public Entity? GetEntity(string name)
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

        // GetEntity
        public T? GetEntity<T>(string name) where T : Entity
        {
            return GetEntity(name) as T;
        }

        // GetEntityNotNull
        public Entity GetEntityNotNull(string name)
        {
            var result = GetEntity(name);
            if (result == null)
                throw new InvalidOperationException($"Entity '{name}' does not exist.");
            else
                return result;
        }

        // GetEntityNotNull
        public T GetEntityNotNull<T>(string name) where T : Entity
        {
            if (GetEntity(name) is not T result)
                throw new InvalidOperationException($"Entity '{name}' does not exist.");
            else
                return result;
        }

        // GetRandomNumber
        public int GetRandomNumber(string name) => randomNumbers[name];

        // IsAwaiting
        [ScriptProperty]
        public bool IsAwaiting => awaitingScripts.Count > 0;

        // IsAwaitingScript
        public bool IsAwaitingScript(Script script)
        {
            if (awaitingScripts.Count == 0)
                return false;

            return awaitingScripts.Contains(script);
        }

        // IsDemo
        [ScriptProperty]
        public bool IsDemo => Game.IsDemo;

        // IsDisposed
        public bool IsDisposed { get; private set; }

        // IsEnteringRoom
        public bool IsEnteringRoom(Room room) => busyRooms.Contains(room);

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
        public Thing? OutcomeTarget { get; private set; }

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

            if (startingRoom == null && !IsNewSession)
                throw new InvalidOperationException("There is no starting room.");

            IsRunning = true;

            OnRun();

            if (IsNewSession)
            {
                // New session script
                if (ScriptLibrary.GetScript(ScriptType.NewSession.ToString()) is Script newSessionScript)
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

        // ShowSoundCaption
        public void ShowSoundCaption(SoundInstance soundInstance)
        {
            OnShowSoundCaption(soundInstance);
        }

        // Start
        public void Start()
        {
            CodeContract.NotDisposed(nameof(Session), IsDisposed);

            ExtendScriptRegistry(ScriptEnvironment.ScriptRegistry);

            ScriptEnvironment.Activate();

            if (State != GameSessionState.Uninitialized || IsInitializing)
                throw new InvalidOperationException();

            this.IsInitializing = true;
            this.IsNewSession = !Game.PlatformBridge.FileSystem.FileExists(SaveFileName);

            // Load scripts
            State = GameSessionState.LoadingScripts;
            ScriptLibrary.Load();
            State = GameSessionState.Idle;

            startingRoom = null;
            if (!IsNewSession)
            {
                State = GameSessionState.Loading;

                if (Game.PlatformBridge.FileSystem.ReadFile(SaveFileName) is Stream stm)
                {
                    using (stm)
                    {
                        using (var input = !XOREncryptor.IsEncryptedXml(stm) ? stm : XOREncryptor.AsStream(stm, XOREncryptor.EncryptionKey))
                        {
                            startingRoom = ReadCore(input);
                        }
                    }
                }

                State = GameSessionState.Idle;
            }

            OnInitializeEntities();

            for (var i = 0; i < Entities.Count; i++)
            {
                Entities[i].Initialize();

                if (Entities[i].Persistent)
                    persistentEntities.Add(Entities[i]);
            }

            persistentEntities.Sort(new EntityComparer());
            IsInitializing = false;
            IsFirstRoomSinceLoad = true;

            OnStart();

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

                return name1.CompareTo(name2);
            }
        }
    }
}