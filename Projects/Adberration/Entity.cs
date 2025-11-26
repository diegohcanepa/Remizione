using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Adberration
{
    /// <summary>
    /// Entity
    /// </summary>
    public abstract partial class Entity : GameObject, INamedObject, ITransform, ISoundEmitter
    {
        #region Private fields

        private string atlasFolder = string.Empty;
        private bool firstUpdate;
        private bool isInitialized;
        private bool isUnloading;
        private readonly Script? loadScript;
        private Entity? parent;
        private int pauseCount;
        private bool persistent;
        private readonly Script? unloadScript;
        private readonly List<Entity> unparentList = [];

        #endregion

        #region Constructor

        // Constructor
        protected Entity(Session session, string name)
            : base(session.Game)
        {
            this.Session = session;
            this.EntityId = session.NextEntityId();
            this.Children = new ChildCollection(this);

            // Annonymous
            if (string.IsNullOrWhiteSpace(name))
            {
                if (session.State == GameSessionState.Uninitialized || session.State == GameSessionState.LoadingScripts)
                    throw new InvalidOperationException("Cannot create anonymous entities until session is fully initialized.");

                InstanceKind = InstanceKind.Anonymous;
                name = string.Empty;
            }

            // Clone
            else if (name.Contains(ScriptSyntax.CloneSuffix))
            {
                InstanceKind = Session.State == GameSessionState.LoadingScripts ? InstanceKind.ScriptClone : InstanceKind.RuntimeClone;
            }

            // Static
            else
            {
                InstanceKind = InstanceKind.Static;
            }

            this.Name = name;
            this.StaticName = InstanceKind == InstanceKind.Anonymous ? GetType().Name : ScriptSyntax.GetStaticName(Name);

            if (InstanceKind != InstanceKind.Anonymous)
                session.RegisterEntity(this);

            Sprite = new EntitySprite(Game, this);

            DefaultImageName = StaticName;
            var index = StaticName.LastIndexOf('-');
            if (index != -1)
                DefaultImageName = StaticName.Substring(index + 1);

            // Cache load script
            if (InstanceKind != InstanceKind.Anonymous)
            {
                // Load script
                loadScript = session.ScriptLibrary.GetScript(ScriptType.Load, Name);
                loadScript ??= session.ScriptLibrary.GetScript(ScriptType.Load, StaticName);

                // Unload script
                unloadScript = session.ScriptLibrary.GetScript(ScriptType.Unload, Name);
                unloadScript ??= session.ScriptLibrary.GetScript(ScriptType.Unload, StaticName);
            }
        }

        #endregion

        #region ISoundEmitter implementation

        // IsAvailable
        bool ISoundEmitter.IsAvailable => Parent != null && Parent == Session.Room;

        // Update
        void ISoundEmitter.Update(SoundInstance soundInstance, float masterVolume)
        {
            OnUpdateEmittingSound(soundInstance, masterVolume);
        }

        #endregion

        #region Private members

        // EncodeImagePath
        private string EncodeImagePath()
        {
            const string texturePackerSmartFolderSuffix = "-assets/";

            if (Atlas != null)
            {
                var defaultFolder = string.IsNullOrWhiteSpace(Name) ? GetType().Name : StaticName;
                var redefinesAtlasFolder = !string.IsNullOrWhiteSpace(AtlasFolder);

                if (Atlas.UseFolderNames)
                    return (redefinesAtlasFolder ? AtlasFolder : defaultFolder) + texturePackerSmartFolderSuffix;
                else
                    return redefinesAtlasFolder ? AtlasFolder : string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }

        // NotifyChildAdded
        private void NotifyChildAdded(Entity child)
        {
            child.Parent = this;

            if (LoadState == LoadState.Loaded || LoadState == LoadState.Loading)
            {
                child.Load();
                Invalidate();
            }

            OnChildAdded(child);

            if (!Session.IsInitializing && !child.Persistent)
            {
                if (!unparentList.Contains(child))
                    unparentList.Add(child);
            }
        }

        // NotifyChildRemoved
        private void NotifyChildRemoved(Entity child)
        {
            child.Parent = null;

            if (LoadState == LoadState.Loaded)
                child.Unload();

            OnChildRemoved(child);

            unparentList.Remove(child);
        }

        // RunScript
        private void RunScript(Script script)
        {
            if (InstanceKind == InstanceKind.Anonymous)
                return;

            if (script.HasCapability(ScriptCapability.SetTargetEntity))
                script.SetTargetEntity(Name);

            Session.ScriptProcessor.RunScript(script);
        }

        #endregion

        #region Protected members

        // GetDefaultImageName
        protected string GetDefaultImageName()
        {
            if (string.IsNullOrWhiteSpace(DefaultImageName))
            {
                const char separator = '-';

                var result = StaticName;
                var index = StaticName.LastIndexOf(separator);
                if (index != -1)
                    result = StaticName.Substring(index + 1);

                return result;
            }
            else
            {
                return DefaultImageName;
            }
        }

        // MatchTransform
        protected void MatchTransform(Entity source)
        {
            Sprite.MatchTransform(source.Sprite);
        }

        // OnAnimationAdded
        protected virtual void OnAnimationAdded(SpriteAnimation animation)
        {
        }

        // OnAtlasChanged
        protected virtual void OnAtlasChanged()
        {
        }

        // OnAtlasFolderChanged
        protected virtual void OnAtlasFolderChanged()
        {
        }

        // OnChildAdded
        protected virtual void OnChildAdded(Entity child)
        {
        }

        // OnChildRemoved
        protected virtual void OnChildRemoved(Entity child)
        {
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Sprite.Draw(gameTime);
        }

        // OnInitialize
        protected virtual void OnInitialize()
        {
        }

        // OnInvalidate
        protected virtual void OnInvalidate()
        {
        }

        // OnLoad
        protected virtual void OnLoad()
        {
        }

        // OnParentChanged
        protected virtual void OnParentChanged(Entity? previousParent)
        {
        }

        // OnPause
        protected virtual void OnPause()
        {
        }

        // OnRead
        protected virtual void OnRead(XmlAttributeCollection attributes)
        {
        }

        // OnResume
        protected virtual void OnResume()
        {
        }

        // OnStart
        protected virtual void OnStart()
        {
        }

        // OnTransform
        protected virtual void OnTransform(TransformChange change)
        {
        }

        // OnUnload
        protected virtual void OnUnload()
        {
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (firstUpdate)
            {
                firstUpdate = false;
                OnStart();
            }

            Sprite.Update(gameTime);
        }

        // OnUpdateEmittingSound
        protected virtual void OnUpdateEmittingSound(SoundInstance instance, float masterVolume)
        {
        }

        // OnWrite
        protected virtual void OnWrite(XmlWriter output)
        {
        }

        // OpacityFactor
        protected float OpacityFactor
        {
            get => Sprite.OpacityFactor;
            set => Sprite.OpacityFactor = value;
        }

        // Sprite
        protected AnimatedSprite Sprite { get; }

        #endregion

        #region Internal members

        // Initialize
        internal void Initialize()
        {
            if (!isInitialized)
            {
                isInitialized = true;
                OnInitialize();
            }
        }

        // Read
        internal void Read(XmlNode node)
        {
            if (!Persistent)
                throw new InvalidOperationException("Entity has no persistence capabilities.");

            if (Session.State != GameSessionState.Loading)
                throw new InvalidOperationException("Session is not being loaded.");

            if (node.Attributes != null)
                OnRead(node.Attributes);
        }

        // Write
        internal void Write(XmlWriter output)
        {
            if (!Persistent)
                throw new InvalidOperationException("Entity has no persistence capabilities.");

            if (!Session.IsSaving)
                throw new InvalidOperationException("Session is not being saved.");

            OnWrite(output);
        }

        #endregion

        // AddAnimation
        public SpriteAnimation AddAnimation(string name)
        {
            var result = Sprite.AddAnimation(name);
            OnAnimationAdded(result);
            return result;
        }

        // AnimationName
        [ScriptProperty]
        public string AnimationName => AnimationPlayer.Animation == null ? string.Empty : AnimationPlayer.Animation.Name;

        // AnimationPlayer
        public SpriteAnimationPlayer AnimationPlayer => Sprite.Player;

        // Atlas
        [ScriptProperty]
        public Atlas? Atlas
        {
            get => Sprite.Atlas;
            set
            {
                if (value != Sprite.Atlas)
                {
                    Sprite.Atlas = value;
                    Sprite.ImagePath = EncodeImagePath();
                    OnAtlasChanged();
                }
            }
        }

        // AtlasFolder
        [ScriptProperty]
        public string AtlasFolder
        {
            get => atlasFolder;
            set
            {
                if (value != atlasFolder)
                {
                    atlasFolder = value;
                    Sprite.ImagePath = EncodeImagePath();
                    OnAtlasFolderChanged();
                }
            }
        }

        // AutoPlayAnimation
        [ScriptProperty]
        public bool AutoPlayAnimation { get; set; } = true;

        // BoundingBox
        public RectangleF BoundingBox => Sprite.BoundingBox;

        // CanParent
        public virtual bool CanParent(Entity child)
        {
            return child != this;
        }

        // Children
        public ChildCollection Children { get; }

        // Color
        [ScriptProperty]
        public Color Color
        {
            get => Sprite.Color;
            set => Sprite.Color = value;
        }

        // DefaultImageName
        [ScriptProperty]
        public string DefaultImageName
        {
            get => Sprite.DefaultImageName;
            set => Sprite.DefaultImageName = value;
        }

        // Degrees
        [ScriptProperty]
        public float Degrees
        {
            get => Sprite.Degrees;
            set
            {
                if (value != Sprite.Degrees)
                {
                    Sprite.Degrees = value;
                }
            }
        }

        // Depth
        public virtual float Depth => 0;

        // DistanceTo
        public float DistanceTo(Vector2 position)
        {
            return Sprite.DistanceTo(position);
        }

        // DistanceTo
        public float DistanceTo(Entity target)
        {
            return Sprite.DistanceTo(target.Sprite);
        }

        // Effects
        [ScriptProperty]
        public SpriteEffects Effects
        {
            get => Sprite.Effects;
            set => Sprite.Effects = value;
        }

        // EntityId
        public long EntityId { get; }

        // FlipHorizontally
        public void FlipHorizontally() => Sprite.FlipHorizontally();

        // HasChildren
        [ScriptProperty]
        public bool HasChildren => Children.Count > 0;

        // HasParent
        [ScriptProperty]
        public bool HasParent => Parent != null;

        // Height
        public int Height => Sprite.Height;

        // ImagePath
        public string ImagePath => Sprite.ImagePath;

        // InstanceKind
        public InstanceKind InstanceKind { get; }

        // Invalidate
        public void Invalidate()
        {
            Invalidate(false);
        }

        // Invalidate
        public void Invalidate(bool recurseChildren)
        {
            OnInvalidate();

            if (recurseChildren)
            {
                for (var i = 0; i < Children.Count; i++)
                {
                    Children[i].Invalidate(recurseChildren);
                }
            }
        }

        // IsFlippedHorizontally
        public bool IsFlippedHorizontally => Sprite.IsFlippedHorizontally;

        // IsFlippedVertically
        public bool IsFlippedVertically => Sprite.IsFlippedVertically;

        // IsParentOf
        public static bool IsParentOf(Entity entity1, Entity entity2)
        {
            // Same entity
            if (entity1 == entity2 || entity2.Parent == null)
                return false;

            if (entity2.Parent == entity1)
                return true;

            // If the parent is not null or equal to the first entity,   
            // call recursively using the parent of the second entity.  
            return IsParentOf(entity1, entity2.Parent);
        }

        // IsPaused
        public bool IsPaused => pauseCount > 0;

        // Load
        public void Load()
        {
            firstUpdate = true;

            if (LoadState == LoadState.Loaded)
                return;

            LoadState = LoadState.Loading;

            OnLoad();

            if (loadScript != null)
                RunScript(loadScript);

            LoadState = LoadState.Loaded;

            // Load children
            for (var i = 0; i < Children.Count; i++)
            {
                Children[i].Load();
            }

            if (AutoPlayAnimation && !AnimationPlayer.IsPlaying)
                Sprite.Player.Play(true, AnimationDirection.Forward, false);
        }

        // LoadState
        public LoadState LoadState { get; private set; }

        // Name
        public string Name { get; }

        // Opacity
        [ScriptProperty]
        public float Opacity
        {
            get => Sprite.Opacity;
            set => Sprite.Opacity = value;
        }

        // Parent
        public Entity? Parent
        {
            get => parent;
            private set
            {
                if (value != parent)
                {
                    var previousParent = parent;

                    if (value != null)
                    {
                        if (!value.CanParent(this))
                            throw new InvalidOperationException($"[{value.GetType().Name}] cannot parent [{GetType().Name}]");

                        if (IsParentOf(value, this))
                            throw new InvalidOperationException("Enities are not allowed to parent each other.");
                    }

                    parent = value;

                    OnParentChanged(previousParent);
                }
            }
        }

        // Pause
        public void Pause()
        {
            pauseCount++;
            if (pauseCount == 1)
                OnPause();
        }

        // Persistent
        public bool Persistent
        {
            get => persistent;
            internal set
            {
                if (InstanceKind == InstanceKind.Anonymous || Session.State != GameSessionState.LoadingScripts)
                    throw new InvalidOperationException();

                this.persistent = value;
            }
        }

        // Origin
        public Vector2 Origin => Sprite.Pivot.Position;

        // PivotOrigin
        [ScriptProperty]
        public RectanglePoint PivotOrigin
        {
            get => Sprite.PivotOrigin;
            set
            {
                if (value != Sprite.PivotOrigin)
                {
                    Sprite.PivotOrigin = value;
                }
            }
        }

        // PlaySound
        public SoundInstance? PlaySound(string name, bool looped = false)
        {
            if (Sound.Find(name.ToString())?.PopInstance() is SoundInstance soundInstance)
            {
                PlaySound(soundInstance, looped);
                return soundInstance;
            }
            else
            {
                return null;
            }
        }

        // PlaySound
        public SoundInstance? PlaySound(Sound sound, bool looped = false)
        {
            if (sound?.PopInstance() is SoundInstance result)
            {
                result.IsLooped = looped;
                PlaySound(result);
                return result;
            }
            else
            {
                return null;
            }
        }

        // PlaySound
        public void PlaySound(SoundInstance instance, bool looped = false)
        {
            if (instance != null)
            {
                instance.Emitter = this;
                instance.IsLooped = looped;
                instance.Play();
            }
        }

        // Position
        [ScriptProperty]
        public Vector2 Position
        {
            get => Sprite.Position;
            set
            {
                if (value != Sprite.Position)
                    Sprite.Position = value;
            }
        }

        // RenderLayerDepth
        public virtual int RenderLayerDepth => 0;

        // Resume
        public void Resume()
        {
            if (pauseCount > 0)
            {
                pauseCount--;
                if (pauseCount == 0)
                    OnResume();
            }
        }

        // Rotation
        [ScriptProperty]
        public float Rotation
        {
            get => Sprite.Rotation;
            set
            {
                if (value != Sprite.Rotation)
                    Sprite.Rotation = value;
            }
        }

        // RotationSpeed
        [ScriptProperty]
        public float RotationSpeed
        {
            get => Sprite.RotationSpeed;
            set
            {
                if (value != Sprite.RotationSpeed)
                    Sprite.RotationSpeed = value;
            }
        }

        // Scale
        [ScriptProperty]
        public Vector2 Scale
        {
            get => Sprite.Scale;
            set
            {
                if (value != Sprite.Scale)
                    Sprite.Scale = value;
            }
        }

        // ScaleX
        [ScriptProperty]
        public float ScaleX
        {
            get => Sprite.ScaleX;
            set
            {
                if (value != Sprite.ScaleX)
                    Sprite.ScaleX = value;
            }
        }

        // ScaleY
        [ScriptProperty]
        public float ScaleY
        {
            get => Sprite.ScaleY;
            set
            {
                if (value != Sprite.ScaleY)
                    Sprite.ScaleY = value;
            }
        }

        // Session
        public virtual Session Session { get; }

        // StaticName
        [ScriptProperty]
        public string StaticName { get; }

        // ToString
        public override string ToString() => Name;

        // Tweens
        public TweenManager Tweens => Sprite.Tweens;

        // Unload
        public void Unload()
        {
            if (LoadState == LoadState.Unloaded || isUnloading)
                return;

            isUnloading = true;

            if (unloadScript != null)
                RunScript(unloadScript);

            Tweens.Reset();

            // Unload children
            var entityList = Children.ToArray();
            for (var i = 0; i < entityList.Length; i++)
            {
                entityList[i].Unload();
            }

            OnUnload();

            AnimationPlayer.Stop();

            if (Atlas?.Content != Game.Content)
                Atlas = null;

            var weakChildren = unparentList.ToArray();
            for (var i = 0; i < weakChildren.Length; i++)
            {
                weakChildren[i].Unparent();
            }

            LoadState = LoadState.Unloaded;
            isUnloading = false;
        }

        // Unparent
        public abstract void Unparent();

        // Width
        public int Width => Sprite.Width;

        // X
        [ScriptProperty]
        public float X
        {
            get => Sprite.X;
            set
            {
                if (value != Sprite.X)
                    Sprite.X = value;
            }
        }

        // Y
        [ScriptProperty]
        public float Y
        {
            get => Sprite.Y;
            set
            {
                if (value != Sprite.Y)
                    Sprite.Y = value;
            }
        }

        /// <summary>
        /// ChildCollection
        /// </summary>
        public sealed partial class ChildCollection : IList<Thing>, IReadOnlyList<Thing>
        {
            #region Private fields

            private readonly List<Thing> items = [];

            #endregion

            #region Constructor

            // Constructor
            internal ChildCollection(Entity owner)
                : base()
            {
                this.Owner = owner;
            }

            #endregion

            #region IEnumerable explicit implementation

            // GetEnumerator
            IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();

            #endregion

            // Add
            public void Add(Thing item) => Insert(items.Count, item);

            // Clear
            public void Clear()
            {
                var list = items.ToArray();

                for (var i = 0; i < list.Length; i++)
                {
                    Remove(list[i]);
                }
            }

            // Contains
            public bool Contains(string name) => Find(name) != null;

            // Contains
            public bool Contains(Thing item) => items.Contains(item);

            // Count
            public int Count => items.Count;

            // CopyTo
            public void CopyTo(Thing[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);

            // Find
            public Thing? Find(string name)
            {
                for (var i = 0; i < items.Count; i++)
                {
                    if (items[i].Name == name)
                        return items[i];
                }

                return null;
            }

            // GetEnumerator
            public IEnumerator<Thing> GetEnumerator() => items.GetEnumerator();

            // IndexOf
            public int IndexOf(Thing item) => items.IndexOf(item);

            // Insert
            public void Insert(int index, Thing item)
            {
                if (Contains(item))
                    return;

                item.Unparent();
                items.Insert(index, item);
                Owner.NotifyChildAdded(item);
            }

            // Indexer
            public Thing this[int index]
            {
                get => items[index];
                set
                {
                    if (value != items[index])
                    {
                        Remove(items[index]);
                        Insert(index, value);
                    }
                }
            }

            // IsReadOnly
            public bool IsReadOnly => false;

            // Owner
            public Entity Owner { get; }

            // Remove
            public bool Remove(Thing item)
            {
                var index = items.IndexOf(item);

                if (index != -1)
                {
                    items.RemoveAt(index);
                    Owner.NotifyChildRemoved(item);
                }

                return index != -1;
            }

            // RemoveAt
            public void RemoveAt(int index) => items.RemoveAt(index);

            // RemoveRange
            public void RemoveRange(IList<Thing> list)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    Remove(list[i]);
                }
            }

            // Sort
            public void Sort(IComparer<Thing> comparer) => items.Sort(comparer);

            // Swap
            public void Swap(Thing item1, Thing item2) => items.Swap(item1, item2);

            // Swap
            public void Swap(int indexA, int indexB) => items.Swap(indexA, indexB);

            // SwapNext
            public bool SwapNext(Thing item) => items.SwapToNext(item);

            // SwapPrevious
            public bool SwapPrevious(Thing item) => items.SwapToPrevious(item);

            // ToArray
            public Thing[] ToArray() => items.ToArray();
        }

        /// <summary>
        /// EntitySprite
        /// </summary>
        public sealed class EntitySprite : AnimatedSprite
        {
            private readonly Entity entity;

            // Constructor
            internal EntitySprite(EngendroGame game, Entity entity)
                : base(game)
            {
                this.entity = entity;
            }

            // OnTransform
            protected override void OnTransform(TransformChange transformChange)
            {
                entity.OnTransform(transformChange);
            }
        }
    }
}