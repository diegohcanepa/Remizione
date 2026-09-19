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

        private readonly List<string> assetNames = [];
        private static readonly List<string> availableTagList = [];
        private int indexOfLastNamePopped = -1;
        private readonly List<SoundInstance?> instancePool;
        private static readonly Dictionary<string, Sound> instancesByName = [];
        private static readonly NamedCollection<Sound> instanceList = [];
        private readonly Dictionary<string, SoundEffect?> soundEffects = [];

        #endregion

        #region Constructor

        // Constructor
        private Sound(string name, SoundCategory category, IList<string> soundNames, string[]? tags, int maxInstances, float volume, float pan, float pitch, Ratio pitchVariance, SoundPopMode popMode, bool transitionAware, SoundScope scope, bool pauseAware, string caption)
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
            this.PitchVariance = pitchVariance;
            this.Caption = caption;
            this.Scope = scope;

            if (soundNames == null || soundNames.Count == 0)
                soundNames = [Name];

            for (var i = 0; i < soundNames.Count; i++)
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

        // AvailableTags
        public static ReadOnlyCollection<string> AvailableTags { get; } = new(availableTagList);

        // Caption
        public string Caption { get; }

        // Category
        public SoundCategory Category { get; }

        // Create
        public static Sound Create(string name, SoundSettings settings)
        {
            var tags = string.IsNullOrWhiteSpace(settings.Tags) ? null : settings.Tags.Split(',');

            return new Sound(name, settings.Category,
                                               settings.Sounds,
                                               tags,
                                               settings.MaxInstances,
                                               settings.Volume,
                                               settings.Pan,
                                               settings.Pitch,
                                               settings.PitchVariance,
                                               settings.PopMode,
                                               settings.TransitionAware,
                                               settings.Scope,
                                               settings.PauseAware,
                                               settings.Caption);
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
            var result = Find(name);
            if (result == null)
                throw new InvalidOperationException($"The sound '{name}' does not exist.");
            else
                return result;
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
                    instancePool[availableIndex] = new SoundInstance(this, soundEffect);
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
