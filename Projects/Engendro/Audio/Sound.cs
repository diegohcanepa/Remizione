using Engendro.Collections;
using Microsoft.Xna.Framework.Audio;
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
    public sealed class Sound : INamedObject
    {
        #region Private fields

        private static readonly HashSet<string> allTags = [];
        private readonly List<string> assetNames = [];
        private int indexOfLastNamePopped = -1;
        private readonly List<SoundInstance?> instancePool;
        private static readonly Dictionary<string, Sound> instancesByName = [];
        private static readonly NamedCollection<Sound> instanceList = [];
        private readonly Dictionary<string, SoundEffect?> soundEffects = [];

        #endregion

        #region Constructor

        // Constructor
        private Sound(string name, SoundSettings settings)
        {
            // Name cannot be empty
            CodeContract.NotEmpty(name, nameof(name));

            // Name must be unique among instances
            if (instancesByName.ContainsKey(name))
                CodeContract.ThrowDuplicatedNameException(nameof(name));

            this.Name = name;
            this.Category = settings.Category;
            this.MaxInstances = settings.MaxInstances;
            this.instancePool = [];

            for (var i = 0; i < MaxInstances; i++)
            {
                this.instancePool.Add(null);
            }

            this.PopMode = settings.PopMode;
            this.Volume = settings.Volume;
            this.Pan = settings.Pan;
            this.Pitch = settings.Pitch;
            this.PitchVariance = settings.PitchVariance;
            this.Caption = settings.Caption;
            this.Scope = settings.Scope;

            if (settings.Sounds.Count == 0)
            {
                assetNames.Add(Name);
                soundEffects.Add(Name, null);
            }
            else
            {
                for (var i = 0; i < settings.Sounds.Count; i++)
                {
                    assetNames.Add(settings.Sounds[i]);
                    soundEffects.Add(settings.Sounds[i], null);
                }
            }

            if (string.IsNullOrWhiteSpace(settings.Tags))
                Tags = [];
            else
                Tags = new ReadOnlyCollection<string>(settings.Tags.Split(','));

            TransitionAware = settings.TransitionAware;

            PauseAware = settings.PauseAware;

            instancesByName.Add(name, this);
            instanceList.Add(this);

            for (var i = 0; i < Tags.Count; i++)
            {
                var tag = Tags[i];
                allTags.Add(tag);
            }
        }

        #endregion

        #region Private members

        // IsInstancePaused
        private bool IsInstancePaused(int index)
        {
            return instancePool[index] is SoundInstance inst && inst.State == SoundState.Paused;
        }

        // IsInstancePlaying
        private bool IsInstancePlaying(int index)
        {
            return instancePool[index] is SoundInstance inst && inst.State == SoundState.Playing;
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

        #endregion

        // AllTags
        public static ReadOnlySet<string> AllTags { get; } = allTags.AsReadOnly();

        // Caption
        public string Caption { get; }

        // Category
        public SoundCategory Category { get; }

        // Create
        internal static Sound Create(string name, SoundSettings settings)
        {
            return new Sound(name, settings);
        }

        // EncodeAssetName
        public static string EncodeAssetName(SoundCategory category, string name)
        {
            return EncodeAssetName(category, string.Empty, name);
        }

        // EncodeAssetName
        public static string EncodeAssetName(SoundCategory category, string subfolder, string name)
        {
            if (!string.IsNullOrWhiteSpace(category.ContentPath))
            {
                List<string> paths =
                [
                   category.ContentPath,
                   subfolder
                ];

                paths.Add(name);

                name = Path.Combine([.. paths]);
            }

            return name;
        }

        // FadeIn
        public void FadeIn(int duration)
        {
            for (var i = 0; i < instancePool.Count; i++)
            {
                instancePool[i]?.Volume.FadeIn(duration);
            }
        }

        // FadeOut
        public void FadeOut(int duration)
        {
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
            if (Find(name) is not Sound sound)
                throw new InvalidOperationException($"The sound '{name}' does not exist.");

            return sound;
        }

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
        public void Load()
        {
            if (IsLoaded)
                return;

            for (var i = 0; i < soundEffects.Count; i++)
            {
                var assetName = EncodeAssetName(Category, assetNames[i]);
                soundEffects[assetNames[i]] = EngendroGame.Instance.Content.Load<SoundEffect>(assetName);
            }

            IsLoaded = true;
        }

        // MaxInstances
        public int MaxInstances { get; } = 6;

        // Name
        public string Name { get; }

        // Looped
        public bool Looped { get; }

        // Pan
        public float Pan { get; }

        // Pause
        public SoundInstance[] Pause()
        {
            List<SoundInstance> result = [];

            for (var i = 0; i < instancePool.Count; i++)
            {
                if (IsInstancePlaying(i) && instancePool[i] is SoundInstance soundInstance)
                {
                    soundInstance.Pause();
                    result.Add(soundInstance);
                }
            }

            return [.. result];
        }

        // PauseAware
        public bool PauseAware { get; }

        // Pitch
        public float Pitch { get; }

        // PitchVariance
        public float PitchVariance { get; }

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
            if (PopInstance() is SoundInstance instance)
            {
                instance.Emitter = emitter;
                instance.Looped = looped;
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
                instance.Looped = looped;
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
            Load();

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
                    instancePool[availableIndex] = instance;
                }
            }

            instance?.ResetToDefault();

            return instance;
        }

        // Reset
        public void Reset()
        {
            Stop();
            for (var i = 0; i < instancePool.Count; i++)
            {
                instancePool[i]?.ResetToDefault();
            }
        }

        // Resume
        public void Resume()
        {
            for (var i = 0; i < instancePool.Count; i++)
            {
                if (IsInstancePaused(i))
                {
                    instancePool[i]?.Resume();
                }
            }
        }

        // Scope
        public SoundScope Scope { get; }

        // Sounds
        public static NamedReadOnlyCollection<Sound> Sounds { get; } = new(instanceList);

        // Stop
        public void Stop()
        {
            Stop(0);
        }

        // Stop
        public void Stop(int fadeOut)
        {
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

        // Volume
        public float Volume { get; }
    }
}
