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

        // FindSoundInstance
        private static SoundInstance? FindSoundInstance(string soundName)
        {
            if (string.IsNullOrWhiteSpace(soundName))
                return null;

            SoundInstance? result = SoundInstance.FindRunningInstance(soundName);
            if (result == null)
                result = Sound.Find(soundName)?.PopInstance();
            else
                result.ResetToDefault();

            if (result != null)
            {
                result.Looped = false;
                return result;
            }

            return null;
        }

        // FindSoundInstanceByTag
        private static SoundInstance? FindSoundInstanceByTag(string tag)
        {
            if (!Sound.AvailableTags.Contains(tag))
                return null;

            SoundInstance? result;
            List<Sound> instances = [.. Sound.FindByTag(tag)];
            if (instances.Count > 0)
                result = instances.GetRandomItem()?.PopInstance();
            else
                result = Sound.Find(tag)?.PopInstance();

            result?.Looped = false;

            return result;
        }

        // PlayTagCore
        private bool PlayTagCore(string tag, int fadeIn)
        {
            if (FindSoundInstanceByTag(tag) is SoundInstance newSoundInstance)
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
                if (CurrentTag.Length > 0)
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
                                PlayTagCore(CurrentTag, 2000);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        // CurrentSoundName
        public string CurrentSoundName
        {
            get
            {
                if (soundInstance == null || soundInstance.State == SoundState.Stopped)
                    return string.Empty;
                else
                    return soundInstance.Sound.Name;
            }
        }

        // CurrentTag
        public string CurrentTag { get; set; } = string.Empty;

        // IsLooped
        public bool IsLooped { get; set; }

        // Play
        public bool Play(string soundName, bool looped)
        {
            return Play(soundName, looped, 0);
        }

        // Play
        public bool Play(string soundName, bool looped, int fadeIn)
        {
            return Play(soundName, looped, fadeIn, 1);
        }

        // Play
        public bool Play(string soundName, bool looped, int fadeIn, float volume)
        {
            return Play(soundName, looped, fadeIn, volume, 0);
        }

        // Play
        public bool Play(string soundName, bool looped, int fadeIn, float volume, float pitch)
        {
            if (string.IsNullOrWhiteSpace(soundName) || soundName == CurrentSoundName)
                return false;

            if (FindSoundInstance(soundName) is SoundInstance newSoundInstance)
            {
                if (newSoundInstance.Sound.Category != AudioManager.MusicCategory)
                    throw new ArgumentException("Not a music sound.", nameof(soundName));

                PlaySoundCore(newSoundInstance, looped, fadeIn, volume, pitch);
            }

            return true;
        }

        // PlayTag
        public void PlayTag(string tag)
        {
            PlayTag(tag, 0);
        }

        // PlayTag
        public bool PlayTag(string tag, int fadeIn)
        {
            if (Sound.AvailableTags.Contains(tag) && tag != CurrentTag)
            {
                PlayTagCore(tag, fadeIn);
                CurrentTag = tag;
                return true;
            }

            return false;
        }

        // Reset
        public void Reset()
        {
            cooldown = 0;
            CurrentTag = string.Empty;
            Stop();
        }

        // State
        public SoundState State => soundInstance == null ? SoundState.Stopped : soundInstance.State;

        // Stop
        public bool Stop()
        {
            return Stop(0);
        }

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
