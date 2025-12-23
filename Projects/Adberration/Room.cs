using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Adberration
{
    /// <summary>
    /// Room
    /// </summary>
    public abstract class Room : Entity, IInputHandler
    {
        #region Private fields

        private readonly List<Area> areas = [];
        private readonly List<Thing> culledThings = new(1000);
        private readonly Script? enteringScript;
        private readonly Script? exitingScript;
        private readonly List<Script> routines = [];
        private readonly List<SoundInstance> sounds = [];

        #endregion

        #region Constructor

        // Constructor
        protected Room(Session session, string name)
            : base(session, name)
        {
            AtlasName = DeclaredName;
            CustomHeight = Session.Game.ViewportAdapter.VirtualHeight;
            CustomWidth = Session.Game.ViewportAdapter.VirtualWidth;
            CulledThings = new ReadOnlyCollection<Thing>(culledThings);
            Areas = new ReadOnlyCollection<Area>(areas);

            // Cache scripts
            if (InstanceKind != InstanceKind.Anonymous)
            {
                enteringScript = session.ScriptLibrary.FindScript(ScriptType.Entering, Name);
                enteringScript ??= session.ScriptLibrary.FindScript(ScriptType.Entering, DeclaredName);

                exitingScript = session.ScriptLibrary.FindScript(ScriptType.Exiting, Name);
                exitingScript ??= session.ScriptLibrary.FindScript(ScriptType.Exiting, DeclaredName);
            }
        }

        #endregion

        #region Private members

        // ResetAreas
        private void ResetAreas()
        {
            for (var i = 0; i < areas.Count; i++)
            {
                areas[i].Reset();
            }
        }

        // StopRoutines
        private void StopRoutines()
        {
            for (var i = 0; i < routines.Count; i++)
            {
                if (!Session.IsAwaitingScript(routines[i]))
                    Session.ScriptProcessor.StopScript(routines[i]);
            }

            routines.Clear();
        }

        // StopSounds
        private void StopSounds()
        {
            for (var i = 0; i < sounds.Count; i++)
            {
                sounds[i].Stop();
            }

            sounds.Clear();
        }

        #endregion

        #region Protected members

        // Areas
        protected ReadOnlyCollection<Area> Areas { get; }

        // Content
        protected ContentManager? Content { get; private set; }

        // GetAtlasPath
        protected abstract string GetAtlasPath();

        // OnHandleInput
        protected virtual HandleInputResult OnHandleInput(GameTime gameTime)
        {
            return HandleInputResult.Unhandled;
        }

        // OnEntering
        protected virtual void OnEntering()
        {
        }

        // OnExit
        protected virtual void OnExit()
        {
        }

        // OnLoad
        protected override void OnLoad()
        {
            Content = Session.Game.GetNewContentManager();

            // Load atlas
            if (Atlas == null && !string.IsNullOrWhiteSpace(AtlasName))
            {
                var atlasEncodedName = ScriptSyntax.RuntimeRoomNamePrefix + AtlasName;
                this.Atlas = Atlas.FindInstance(atlasEncodedName);
                this.Atlas ??= new Atlas(Content, atlasEncodedName, GetAtlasPath(), false);
            }

            ResetAreas();
        }

        // OnPause
        protected override void OnPause()
        {
            // Pause all local scripts
            for (var i = 0; i < routines.Count; i++)
            {
                Session.ScriptProcessor.PauseScript(routines[i]);
            }

            // Pause all sounds
            for (var i = 0; i < sounds.Count; i++)
            {
                sounds[i].Pause();
            }
        }

        // OnResume
        protected override void OnResume()
        {
            for (var i = 0; i < routines.Count; i++)
            {
                Session.ScriptProcessor.ResumeScript(routines[i]);
            }

            for (var i = 0; i < sounds.Count; i++)
            {
                sounds[i].Resume();
            }
        }

        // OnUnload
        protected override void OnUnload()
        {
            if (Content != null)
            {
                Atlas.DisposeFromContent(Content);
                Content.Dispose();
                Content = null;
            }

            StopRoutines();
            StopSounds();

            base.OnUnload();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            // Skip Update if room is no longer the current scene
            if (!IsCurrentRoom)
                return;

            base.OnUpdate(gameTime);

            InvalidateCulledThings();

            for (var i = 0; i < culledThings.Count; i++)
            {
                culledThings[i].Update(gameTime);
            }
        }

        // InvalidateCulledThings
        protected void InvalidateCulledThings()
        {
            var count = 0;

            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].IsActiveInGameLoop)
                {
                    if (count < culledThings.Count)
                        culledThings[count] = Children[i];
                    else
                        culledThings.Add(Children[i]);

                    count++;
                }
            }

            if (count < culledThings.Count)
                culledThings.RemoveRange(count, culledThings.Count - count);

            culledThings.Sort(EntityDepthComparer.Instance);
        }

        #endregion

        #region Internal members

        // Enter
        internal void Enter()
        {
            if (enteringScript != null)
                RunScript(enteringScript);

            OnEntering();

            for (var i = 0; i < Children.Count; i++)
            {
                Children[i].EnterRoom();
            }
        }

        // Exit
        internal void Exit()
        {
            if (exitingScript != null)
                RunScript(exitingScript);

            OnExit();

            for (var i = 0; i < Children.Count; i++)
            {
                Children[i].ExitRoom();
            }
        }

        // RegisterRoutine
        internal void RegisterRoutine(Script routine)
        {
            if (!routines.Contains(routine))
                routines.Add(routine);
        }

        // RegisterSound
        internal void RegisterSound(SoundInstance soundInstance)
        {
            if (!sounds.Contains(soundInstance))
                sounds.Add(soundInstance);
        }

        #endregion

        // AllowSaving
        [ScriptProperty]
        public bool AllowSaving { get; set; } = true;

        // AtlasName
        [ScriptProperty]
        public string AtlasName { get; set; } = string.Empty;

        // CanParent
        public override bool CanParent(Entity child)
        {
            return child is Thing && base.CanParent(child);
        }

        // CanSave
        public bool CanSave => Persistent && AllowSaving;

        // CulledThings
        public ReadOnlyCollection<Thing> CulledThings { get; }

        // CustomHeight
        [ScriptProperty(CodingContext.EntityDeclaration)]
        public int CustomHeight { get; set; }

        // CustomWidth
        [ScriptProperty(CodingContext.EntityDeclaration)]
        public int CustomWidth { get; set; }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (Session.IsAwaiting)
                return HandleInputResult.Unhandled;
            else
                return OnHandleInput(gameTime);
        }

        // IsCurrentRoom
        [ScriptProperty]
        public bool IsCurrentRoom => Session.Room == this;

        // IsPreviousRoom
        [ScriptProperty]
        public bool IsPreviousRoom => Session.PreviousRoom == this;

        // ScrollLock
        [ScriptProperty]
        public ScrollLock ScrollLock { get; set; }

        // ToString
        public override string ToString()
        {
            return Name;
        }

        // UnloadMode
        public UnloadMode UnloadMode { get; set; }

        // Unparent
        public sealed override void Unparent()
        {
        }

        // Zoom
        [ScriptProperty]
        public float Zoom
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    if (IsCurrentRoom)
                        Session.Camera.Zoom = field;
                }
            }
        } = 1;

        /// <summary>
        /// Area
        /// </summary>
        public class Area : INamedObject
        {

            #region Constructor

            // Constructor
            public Area(Room room, string name, FlagCondition? condition, Vector2[] vertices)
            {
                this.Room = room;
                this.Name = name;
                this.Condition = condition;
                this.Polygon = new ReadOnlyPolygon(Geometry.SimplifyPolygon(vertices));

                room.areas.Add(this);
            }

            #endregion

            #region Protected members

            // OnEnabledChanged
            protected virtual void OnEnabledChanged()
            {
            }

            // OnReset
            protected virtual void OnReset()
            {
            }

            // OnUpdate
            protected virtual void OnUpdate(GameTime gameTime)
            {
            }

            #endregion

            // Condition
            public FlagCondition? Condition { get; }

            // IsEmpty
            public bool IsEmpty => Polygon == null || Polygon.IsEmpty;

            // IsEnabled
            public bool IsEnabled
            {
                get;
                set
                {
                    if (value != field)
                    {
                        field = value;
                        OnEnabledChanged();
                    }
                }
            } = true;

            // IsInCullingBox
            public bool IsInCullingBox => Room.Session.Camera.CullingBox.Intersects(Polygon.BoundingRectangleF);

            // Name
            public string Name { get; }

            // Polygon
            public ReadOnlyPolygon Polygon { get; }

            // Reset
            public void Reset()
            {
                OnReset();
            }

            // Room
            public Room Room { get; }

            // Test
            public bool Test()
            {
                return IsEnabled && (Condition == null || Condition.Evaluate());
            }

            // ToString
            public override string ToString()
            {
                return Name;
            }

            // Update
            public void Update(GameTime gameTime)
            {
                OnUpdate(gameTime);
            }
        }
    }
}
