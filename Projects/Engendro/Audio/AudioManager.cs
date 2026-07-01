using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.IO;
using System.Linq;
using System.Xml;

namespace Engendro.Audio
{
    /// <summary>
    /// AudioManager
    /// </summary>
    public static class AudioManager
    {
        private static readonly string[] validAttributes = ["Caption", "MaxInstances", "Name", "Pan", "Pitch", "PitchVariance", "PopMode", "Sounds", "Tags", "TransitionAware", "PauseAware", "Volume"];

        #region Private members

        // LoadCore
        private static void LoadCore(XmlDocument doc, SoundCategoryName category)
        {
            foreach (XmlNode? node in doc.GetElementsByTagName(category.ToString()))
            {
                var attributes = node?.Attributes;
                if (attributes == null)
                    continue;

                for (var i = 0; i < attributes.Count; i++)
                {
                    if (!validAttributes.Contains(attributes[i].Name))
                        throw new InvalidOperationException($"'{attributes[i].Name}' attribute in sound data file is not valid.");
                }

                // Name
                var name = attributes["Name"]?.Value;
                if (name == null)
                    continue;

                SoundSettings settings = new()
                {
                    Category = GetCategory(category)
                };

                // Caption
                if (attributes["Caption"]?.Value is string caption)
                    settings.Caption = caption;

                // MaxInstances
                if (attributes["MaxInstances"]?.Value is string maxInstances)
                    settings.MaxInstances = XmlConvert.ToInt32(maxInstances);

                // Pan
                if (attributes["Pan"]?.Value is string pan)
                    settings.Pan = XmlConvert.ToSingle(pan);

                // Pitch
                if (attributes["Pitch"]?.Value is string pitch)
                    settings.Pitch = XmlConvert.ToSingle(pitch);

                // PitchVariance
                if (attributes["PitchVariance"]?.Value is string pitchVariance)
                    settings.PitchVariance = XmlConvert.ToSingle(pitchVariance);

                // PopMode
                if (attributes["PopMode"]?.Value is string popMode)
                    settings.PopMode = Enum.Parse<SoundPopMode>(popMode);

                // Sounds
                if (attributes["Sounds"]?.Value is string soundNames)
                {
                    soundNames = soundNames.Replace("..", Int32Range.Separator);

                    // Range format
                    if (Int32Range.TryParse(soundNames, out var result))
                    {
                        var names = new string[result.Delta + 1];
                        for (var i = 0; i < names.Length; i++)
                        {
                            names[i] = name + XmlConvert.ToString(i + 1);
                        }

                        settings.SoundNames = string.Join(',', names);
                    }
                    else
                    {
                        settings.SoundNames = soundNames;
                    }
                }

                // Tags
                if (attributes["Tags"]?.Value is string tags)
                    settings.Tags = tags;

                // TransitionAware
                if (attributes["TransitionAware"]?.Value is string transitionAware)
                    settings.TransitionAware = XmlConvert.ToBoolean(transitionAware);
                else
                    settings.TransitionAware = settings.Category != MusicCategory;

                // PauseAware
                if (attributes["PauseAware"]?.Value is string pauseAware)
                    settings.PauseAware = XmlConvert.ToBoolean(pauseAware);
                else
                    settings.PauseAware = settings.Category != MusicCategory && settings.Category != AmbienceCategory;

                // Volume
                if (attributes["Volume"]?.Value is string volume)
                    settings.Volume = XmlConvert.ToSingle(volume);

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

        // LoadAllSounds
        public static void LoadAll()
        {
            // Sounds
            foreach (var sound in Sound.Sounds)
            {
                sound.Load(DefaultContent);
            }
        }

        // Load
        public static void Load(string fileName)
        {
            try
            {
                using var input = TitleContainer.OpenStream(fileName);
                XmlDocument doc = new();
                doc.Load(input);

                LoadCore(doc, SoundCategoryName.Ambience);
                LoadCore(doc, SoundCategoryName.Music);
                LoadCore(doc, SoundCategoryName.FX);
                LoadCore(doc, SoundCategoryName.Voice);
            }
            catch (FileNotFoundException)
            {
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