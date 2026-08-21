using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Text.Json;

namespace Engendro.Audio
{
    /// <summary>
    /// AudioManager
    /// </summary>
    public static class AudioManager
    {
        #region Private members

        // ParseCategory
        private static void ParseCategory(JsonElement root, SoundCategoryName category)
        {
            if (!root.TryGetProperty(category.ToString(), out var array) || array.ValueKind != JsonValueKind.Array)
                return;

            foreach (var element in array.EnumerateArray())
            {
                var name = element.GetProperty("name").GetString()!;

                SoundSettings settings = new()
                {
                    Category = GetCategory(category)
                };

                // Caption
                if (element.GetString("caption") is string caption && !string.IsNullOrWhiteSpace(caption))
                    settings.Caption = caption;

                // MaxInstances
                if (element.GetInt32("maxInstances") is int maxInstances)
                    settings.MaxInstances = maxInstances;

                // Pan
                if (element.GetFloat("pan") is float pan)
                    settings.Pan = pan;

                // PauseAware
                if (element.GetBool("pauseAware") is bool pauseAware)
                    settings.PauseAware = pauseAware;

                // Pitch
                if (element.GetFloat("pitch") is float pitch)
                    settings.Pitch = pitch;

                // PitchVariance
                if (element.GetFloat("pitchVariance") is float pitchVariance)
                    settings.PitchVariance = pitchVariance;

                // PopMode
                if (element.GetEnum<SoundPopMode>("popMode") is SoundPopMode popMode)
                    settings.PopMode = popMode;

                // SoundCount
                if (element.GetInt32("soundCount") is int soundCount)
                {
                    for (var i = 0; i < soundCount; i++)
                    {
                        settings.Sounds.Add($"{name}{i + 1}");
                    }
                }

                // Tags
                settings.Tags = element.GetString("tags");

                // TransitionAware
                if (element.GetBool("transitionAware") is bool transitionAware)
                    settings.TransitionAware = transitionAware;

                // Volume
                if (element.GetFloat("volume") is float volume)
                    settings.Volume = volume;

                Sound.Create(name, settings);
            }
        }

        #endregion

        #region Internal members

        // IsPaused
        internal static bool IsPaused { get; private set; }

        // Pause
        internal static void Pause()
        {
            if (IsPaused)
                return;

            for (var i = 0; i < Sound.Sounds.Count; i++)
            {
                Sound.Sounds[i].Pause();
            }

            IsPaused = true;
        }

        // Resume
        internal static void Resume()
        {
            if (!IsPaused)
                return;

            IsPaused = false;

            for (var i = 0; i < Sound.Sounds.Count; i++)
            {
                Sound.Sounds[i].Resume();
            }
        }

        // Update
        internal static void Update(GameTime gameTime)
        {
            AmbienceCategory.Update(gameTime);
            FXCategory.Update(gameTime);
            VoiceCategory.Update(gameTime);
            MusicCategory.Update(gameTime);
            SoundInstance.Update(gameTime);
            Music.Update(gameTime);
        }

        #endregion

        // AmbienceCategory
        public static SoundCategory AmbienceCategory { get; } = new SoundCategory(SoundCategoryName.Ambience.ToString());

        // DefaultContent
        public static ContentManager DefaultContent
        {
            get
            {
                if (EngendroGame.Instance == null)
                    throw new InvalidOperationException();

                return EngendroGame.Instance.Content;
            }
        }

        // FXCategory
        public static SoundCategory FXCategory { get; } = new SoundCategory(SoundCategoryName.FX.ToString());

        // GetCategory
        public static SoundCategory GetCategory(SoundCategoryName name)
        {
            return name switch
            {
                SoundCategoryName.Ambience => AmbienceCategory,
                SoundCategoryName.Music => MusicCategory,
                SoundCategoryName.Voice => VoiceCategory,
                SoundCategoryName.FX => FXCategory,
                _ => throw new NotImplementedException(),
            };
        }

        // Load
        public static void Load(string fileName)
        {
            using var input = TitleContainer.OpenStream(fileName);
            using JsonDocument doc = JsonDocument.Parse(input);
            var root = doc.RootElement;

            ParseCategory(root, SoundCategoryName.Ambience);
            ParseCategory(root, SoundCategoryName.FX);
            ParseCategory(root, SoundCategoryName.Music);
            ParseCategory(root, SoundCategoryName.Voice);
        }

        // LoadAllSounds
        public static void LoadAll()
        {
            // Sounds
            foreach (var sound in Sound.Sounds)
            {
                sound.Load(DefaultContent);
            }
        }

        // MasterVolume
        public static float MasterVolume
        {
            get => SoundEffect.MasterVolume;
            set => SoundEffect.MasterVolume = MathHelper.Clamp(value, 0, 1);
        }

        // Music
        public static Music Music { get; } = new Music();

        // MusicCategory
        public static SoundCategory MusicCategory { get; } = new SoundCategory(SoundCategoryName.Music.ToString());

        // Reset
        public static void Reset()
        {
            Music.Reset();

            for (var i = 0; i < Sound.Sounds.Count; i++)
            {
                Sound.Sounds[i].Reset();
            }
        }

        // VoiceCategory
        public static SoundCategory VoiceCategory { get; } = new SoundCategory(SoundCategoryName.Voice.ToString());
    }
}