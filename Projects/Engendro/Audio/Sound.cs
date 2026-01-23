using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Engendro.Audio
{
    /// <summary>
    /// Sound
    /// </summary>
    // TODO: Hay un bug cuando un sonido toca loopeado y te vas con el cursor al otro monitor y queda loopeando.
    public sealed partial class Sound : IDisposable, INamedObject
    {
        #region Private fields

        private readonly List<string> assetNames = [];
        private static readonly List<string> availableTagList = [];
        private ContentManager? content;
        private int indexOfLastNamePopped = -1;
        private readonly List<SoundInstance?> instancePool;
        private static readonly Dictionary<string, Sound> instancesByName = [];
        private static readonly NamedObjectCollection<Sound> instanceList = [];
        private readonly Dictionary<string, SoundEffect?> soundEffects = [];

        #endregion

        #region Constructor

        // Static constructor
        static Sound()
        {
            AvailableTags = new ReadOnlyCollection<string>(availableTagList);
            Sounds = new NamedObjectReadOnlyCollection<Sound>(instanceList);
        }

        // Constructor
        private Sound(string name, SoundCategory category, string[]? soundNames, string[]? tags, int maxInstances, float volume, float pan, float pitch, SoundPopMode popMode, bool transitionAware, bool pauseAware, string caption)
        {
            // Name cannot be empty
            CodeContract.NotEmpty(name, nameof(name));

            // Name must be unique among instances
            if (instancesByName.ContainsKey(name))
                CodeContract.ThrowDuplicatedNameException(nameof(name));

            this.Name = name;
            this.Category = category;

            if (maxInstances < 1)
                throw new ArgumentOutOfRangeException(nameof(maxInstances), "Maximum instances value must be greater than zero.");

            CodeContract.ValidRatio(volume, nameof(volume));
            CodeContract.ValidRange(pan, -1, 1, nameof(pan));
            CodeContract.ValidRange(pitch, -1, 1, nameof(pitch));

            this.Category = category;
            this.MaxInstances = Math.Max(1, maxInstances);
            this.instancePool = [];

            for (var i = 0; i < maxInstances; i++)
            {
                this.instancePool.Add(null);
            }

            this.PopMode = popMode;
            this.Volume = volume;
            this.Pan = pan;
            this.Pitch = pitch;
            this.Caption = caption;

            if (soundNames == null || soundNames.Length == 0)
                soundNames = [Name];

            for (var i = 0; i < soundNames.Length; i++)
            {
                assetNames.Add(soundNames[i]);
                soundEffects.Add(soundNames[i], null);
            }

            Tags = new ReadOnlyCollection<string>(tags ?? []);
            TransitionAware = transitionAware;
            PauseAware = pauseAware;

            instancesByName.Add(name, this);
            instanceList.Add(this);

            if (!availableTagList.Contains(Name))
                availableTagList.Add(Name);

            for (var i = 0; i < Tags.Count; i++)
            {
                var tag = Tags[i];
                if (!availableTagList.Contains(tag))
                {
                    availableTagList.Add(tag);
                }
            }
        }

        #endregion

        #region Private members

        // EncodeAssetName
        private static string EncodeAssetName(SoundCategory category, string name)
        {
            if (!string.IsNullOrWhiteSpace(category.ContentPath))
            {
                List<string> paths =
                [
                   category.ContentPath
                ];

                paths.Add(name);

                name = Path.Combine([.. paths]);
            }

            return name;
        }

        // IsInstancePaused
        private bool IsInstancePaused(int index)
        {
            return instancePool[index] is SoundInstance inst && !inst.IsDisposed && inst.State == SoundState.Paused;
        }

        // IsInstancePlaying
        private bool IsInstancePlaying(int index)
        {
            return instancePool[index] is SoundInstance inst && !inst.IsDisposed && inst.State == SoundState.Playing;
        }

        // LoadCore
        private void LoadCore(ContentManager content)
        {
            if (IsLoaded)
                return;

            this.content = content;

            for (var i = 0; i < soundEffects.Count; i++)
            {
                soundEffects[assetNames[i]] = LoadSoundEffect(content, i);
            }

            IsLoaded = true;
        }

        // LoadSoundEffect
        private SoundEffect LoadSoundEffect(ContentManager content, int assetNameIndex)
        {
            var assetName = EncodeAssetName(Category, assetNames[assetNameIndex]);
            return content.Load<SoundEffect>(assetName);
        }

        // NextSoundName
        private string NextSoundName(int index)
        {
            if (index >= 0 && index < assetNames.Count)
                return assetNames[index];

            if (PopMode == SoundPopMode.Random)
            {
                return assetNames.Count == 1 ? assetNames[0] : assetNames[Random.Shared.Next(0, assetNames.Count)];
            }
            else
            {
                indexOfLastNamePopped++;
                if (indexOfLastNamePopped == assetNames.Count)
                    indexOfLastNamePopped = 0;

                return assetNames[indexOfLastNamePopped];
            }
        }

        // UnloadCore
        private void UnloadCore()
        {
            if (!IsLoaded)
                return;

            for (var i = 0; i < instancePool.Count; i++)
            {
                if (instancePool[i] is SoundInstance soundInstance)
                {
                    soundInstance.Dispose();
                    instancePool[i] = null;
                }
            }

            instanceList.Remove(this);
            instancesByName.Remove(Name);

            this.content = null;

            IsLoaded = false;
        }

        #endregion

        // AvailableTags
        public static ReadOnlyCollection<string> AvailableTags { get; }

        // Caption
        public string Caption { get; }

        // Category
        public SoundCategory Category { get; }

        // Create
        public static Sound Create(string name, SoundSettings settings)
        {
            var soundNames = string.IsNullOrWhiteSpace(settings.SoundNames) ? null : settings.SoundNames.Split(',');
            var tags = string.IsNullOrWhiteSpace(settings.Tags) ? null : settings.Tags.Split(',');

            return new Sound(name, settings.Category,
                                               soundNames,
                                               tags,
                                               settings.MaxInstances,
                                               settings.Volume,
                                               settings.Pan,
                                               settings.Pitch,
                                               settings.PopMode,
                                               settings.TransitionAware,
                                               settings.PauseAware,
                                               settings.Caption);
        }

        // Dispose
        public void Dispose()
        {
            if (IsDisposed)
                return;

            UnloadCore();

            IsDisposed = true;
        }

        // FadeIn
        public void FadeIn(int duration)
        {
            if (IsDisposed)
                return;

            for (var i = 0; i < instancePool.Count; i++)
            {
                instancePool[i]?.Volume.FadeIn(duration);
            }
        }

        // FadeOut
        public void FadeOut(int duration)
        {
            if (IsDisposed)
                return;

            for (var i = 0; i < instancePool.Count; i++)
            {
                instancePool[i]?.Volume.FadeOut(duration);
            }
        }

        // Find
        public static Sound? Find(string name)
        {
            return instancesByName.TryGetValue(name, out var result) ? result : null;
        }

        // FindByTag
        public static IEnumerable<Sound> FindByTag(string tag)
        {
            for (var i = 0; i < instanceList.Count; i++)
            {
                if (instanceList[i].Tags.Contains(tag))
                    yield return instanceList[i];
            }
        }

        // Get
        public static Sound Get(string name)
        {
            var result = Find(name);
            if (result == null)
                throw new InvalidOperationException($"The sound '{name}' does not exist.");
            else
                return result;
        }

        // IsDisposed
        public bool IsDisposed { get; private set; }

        // IsLoaded
        public bool IsLoaded { get; private set; }

        // IsPlaying
        public bool IsPlaying
        {
            get
            {
                for (var i = 0; i < instancePool.Count; i++)
                {
                    if (IsInstancePlaying(i))
                        return true;
                }

                return false;
            }
        }

        // Load
        public void Load(ContentManager content)
        {
            if (this.content == null)
                LoadCore(content);

            else if (this.content != content)
                throw new InvalidOperationException("Sound has been loaded from another content manager.");
        }

        // MaxInstances
        public int MaxInstances { get; }

        // Name
        public string Name { get; }

        // Pan
        public float Pan { get; }

        // Pause
        public SoundInstance[] Pause()
        {
            List<SoundInstance> result = [];

            if (!IsDisposed)
            {
                for (var i = 0; i < instancePool.Count; i++)
                {
                    if (IsInstancePlaying(i) && instancePool[i] is SoundInstance soundInstance)
                    {
                        soundInstance.Pause();
                        result.Add(soundInstance);
                    }
                }
            }

            return [.. result];
        }

        // PauseAware
        public bool PauseAware { get; }

        // Pitch
        public float Pitch { get; }

        // Play
        public SoundInstance? Play()
        {
            return Play(false, null);
        }

        // Play
        public SoundInstance? Play(bool looped)
        {
            return Play(looped, null);
        }

        // Play
        public SoundInstance? Play(bool looped, ISoundEmitter? emitter)
        {
            CodeContract.NotDisposed(nameof(Sound), IsDisposed);

            if (PopInstance() is SoundInstance instance)
            {
                instance.Emitter = emitter;
                instance.IsLooped = looped;
                instance.Play();
                return instance;
            }
            else
            {
                return null;
            }
        }

        // Play
        public static SoundInstance? Play(string name, bool looped = false)
        {
            return Play(name, looped, true);
        }

        // Play
        public static SoundInstance? Play(string name, bool looped, bool transitionAware)
        {
            if (Find(name)?.PopInstance() is SoundInstance instance)
            {
                instance.IsLooped = looped;
                instance.TransitionAware = transitionAware;
                instance.Play();
                return instance;
            }
            else
            {
                return null;
            }
        }

        // PopMode
        public SoundPopMode PopMode { get; }

        // PopInstance
        public SoundInstance? PopInstance()
        {
            return PopInstance(-1);
        }

        // PopInstance
        public SoundInstance? PopInstance(int index)
        {
            if (!IsLoaded && AudioManager.DefaultContent != null)
                LoadCore(AudioManager.DefaultContent);

            CodeContract.NotDisposed(nameof(Sound), IsDisposed);
            CodeContract.NotLoaded(nameof(Sound), IsLoaded);

            // Pick next sound name
            var name = NextSoundName(index);

            // Try to reuse existing sound instance
            SoundInstance? instance = null;
            var availableIndex = -1;
            for (var i = 0; i < instancePool.Count; i++)
            {
                if (instancePool[i] is SoundInstance inst && inst.Sound.Name == name && inst.State == SoundState.Stopped && inst.AllowReuse)
                {
                    instance = instancePool[i];
                    break;
                }

                if (availableIndex == -1)
                {
                    var soundInstance = instancePool[i];
                    if (soundInstance == null || (soundInstance.State == SoundState.Stopped && soundInstance.AllowReuse))
                    {
                        availableIndex = i;
                    }
                }
            }

            if (instance == null && availableIndex != -1)
            {
                if (soundEffects[name] is SoundEffect soundEffect)
                {
                    instance = new SoundInstance(this, soundEffect);
                    instancePool[availableIndex]?.Dispose();
                    instancePool[availableIndex] = instance;
                }
            }

            instance?.ResetToDefault();

            return instance;
        }

        // Reset
        public void Reset()
        {
            if (IsDisposed)
                return;

            Stop();
            for (var i = 0; i < instancePool.Count; i++)
            {
                instancePool[i]?.ResetToDefault();
            }
        }

        // Resume
        public void Resume()
        {
            if (IsDisposed)
                return;

            for (var i = 0; i < instancePool.Count; i++)
            {
                if (IsInstancePaused(i))
                {
                    instancePool[i]?.Resume();
                }
            }
        }

        // Sounds
        public static NamedObjectReadOnlyCollection<Sound> Sounds { get; }

        // Stop
        public void Stop()
        {
            Stop(0);
        }

        // Stop
        public void Stop(int fadeOut)
        {
            if (IsDisposed)
                return;

            for (var i = 0; i < instancePool.Count; i++)
            {
                if (IsInstancePlaying(i))
                {
                    instancePool[i]?.Stop(fadeOut);
                }
            }
        }

        // Tags
        public ReadOnlyCollection<string> Tags { get; }

        // ToString
        public override string ToString()
        {
            return Name;
        }

        // TransitionAware
        public bool TransitionAware { get; }

        // TryLoad
        public bool TryLoad(ContentManager content)
        {
            if (this.content == null)
            {
                try
                {
                    LoadCore(content);
                    return true;
                }
                catch (ContentLoadException)
                {
                }
            }

            return false;
        }

        // UnloadFromContent
        public static int UnloadFromContent(ContentManager content)
        {
            List<Sound> list = [];

            for (var i = 0; i < instanceList.Count; i++)
            {
                if (instanceList[i].content == content)
                    list.Add(instanceList[i]);
            }

            for (var i = 0; i < list.Count; i++)
            {
                list[i].UnloadCore();
            }

            return list.Count;
        }

        // Volume
        public float Volume { get; }
    }
}
