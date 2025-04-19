using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace Engendro.Audio
{
    /// <summary>
    /// Music
    /// </summary>
    public sealed class Music
    {
        #region Private fields

        private int cooldown;
        private SoundInstance? soundInstance;

        #endregion

        #region Constructor

        // Constructor
        internal Music()
        {
        }

        #endregion

        #region Private members

        // GetSoundInstance
        private static SoundInstance? GetSoundInstance(string soundName)
        {
            if (string.IsNullOrWhiteSpace(soundName))
                return null;

            SoundInstance? result = SoundInstance.GetRunningInstance(soundName);
            if (result == null)
                result = Sound.Find(soundName)?.PopInstance();
            else
                result.ResetToDefault();

            if (result != null)
            {
                result.IsLooped = false;
                return result;
            }

            return null;
        }

        // GetSoundInstanceByTag
        private static SoundInstance? GetSoundInstanceByTag(string tag)
        {
            if (!Sound.AvailableTags.Contains(tag))
                return null;

            SoundInstance? result;
            List<Sound> instances = new(Sound.FindByTag(tag));
            if (instances.Count > 0)
                result = instances.GetRandomElement()?.PopInstance();
            else
                result = Sound.Find(tag)?.PopInstance();

            if (result != null)
                result.IsLooped = false;

            return result;
        }

        // PlayTagCore
        private bool PlayTagCore(string tag, int fadeIn)
        {
            if (GetSoundInstanceByTag(tag) is SoundInstance newSoundInstance)
            {
                PlaySoundCore(newSoundInstance, false, fadeIn, 1, 0);
                return true;
            }
            else
            {
                return false;
            }
        }

        // PlaySoundCore
        private void PlaySoundCore(SoundInstance instance, bool looped, int fadeIn, float volume, float pitch)
        {
            Stop(2000);
            soundInstance = instance;
            soundInstance.Pitch = pitch;
            soundInstance.Volume.Master = volume;
            soundInstance.Play(fadeIn);
            IsLooped = looped;
            cooldown = 0;
        }

        #endregion

        #region Internal members

        // Update
        internal void Update(GameTime gameTime)
        {
            if (soundInstance != null && !soundInstance.IsPlaying && IsLooped)
            {
                if (soundInstance.State != SoundState.Paused)
                {
                    soundInstance.Play();
                }
            }
            else
            {
                if (PlayingTag.Length > 0)
                {
                    if (soundInstance == null || !soundInstance.IsPlaying)
                    {
                        if (cooldown <= 0)
                        {
                            cooldown = 1000;
                        }
                        else
                        {
                            cooldown -= gameTime.ElapsedGameTime.Milliseconds;
                            if (cooldown <= 0)
                            {
                                PlayTagCore(PlayingTag, 2000);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        // IsLooped
        public bool IsLooped { get; set; }

        // Play
        public bool Play(string soundName, bool looped) => Play(soundName, looped, 0);

        // Play
        public bool Play(string soundName, bool looped, int fadeIn) => Play(soundName, looped, fadeIn, 1);

        // Play
        public bool Play(string soundName, bool looped, int fadeIn, float volume) => Play(soundName, looped, fadeIn, volume, 0);

        // Play
        public bool Play(string soundName, bool looped, int fadeIn, float volume, float pitch)
        {
            if (string.IsNullOrWhiteSpace(soundName) || soundName == PlayingSoundName)
                return false;

            if (GetSoundInstance(soundName) is SoundInstance newSoundInstance)
            {
                if (newSoundInstance.Sound.Category != AudioManager.MusicCategory)
                    throw new ArgumentException("Not a music sound.", nameof(soundName));

                PlaySoundCore(newSoundInstance, looped, fadeIn, volume, pitch);
            }

            return true;
        }

        // PlayingSoundName
        public string PlayingSoundName
        {
            get
            {
                if (soundInstance == null || soundInstance.State == SoundState.Stopped)
                    return string.Empty;
                else
                    return soundInstance.Sound.Name;
            }
        }

        // PlayingTag
        public string PlayingTag { get; set; } = string.Empty;

        // PlayTag
        public void PlayTag(string tag) => PlayTag(tag, 0);

        // PlayTag
        public bool PlayTag(string tag, int fadeIn)
        {
            if (Sound.AvailableTags.Contains(tag) && tag != PlayingTag)
            {
                PlayTagCore(tag, fadeIn);
                PlayingTag = tag;
                return true;
            }

            return false;
        }

        // Reset
        public void Reset()
        {
            cooldown = 0;
            PlayingTag = string.Empty;
            Stop();
        }

        // State
        public SoundState State => soundInstance == null ? SoundState.Stopped : soundInstance.State;

        // Stop
        public bool Stop() => Stop(0);

        // Stop
        public bool Stop(int fadeOut)
        {
            if (soundInstance != null && soundInstance.IsPlaying)
            {
                soundInstance.Stop(fadeOut);
                soundInstance = null;
                return true;
            }

            return false;
        }
    }
}
