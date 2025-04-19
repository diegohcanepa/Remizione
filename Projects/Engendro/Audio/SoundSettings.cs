using System;

namespace Engendro.Audio
{
    /// <summary>
    /// SoundSettings
    /// </summary>
    public sealed class SoundSettings
    {
        private int maxInstances = 1;
        private float pan;
        private float pitch;
        private float volume = 1;

        // Caption
        public string Caption { get; set; } = string.Empty;

        // Category
        public SoundCategory Category { get; set; } = AudioManager.FXCategory;

        // MaxInstances
        public int MaxInstances
        {
            get => maxInstances;
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 1);
                this.maxInstances = value;
            }
        }

        // Pan
        public float Pan
        {
            get => pan;
            set
            {
                if (value < -1.0f || value > 1.0f)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                this.pan = value;
            }
        }

        // PauseAware
        public bool PauseAware { get; set; } = true;

        // Pitch
        public float Pitch
        {
            get => pitch;
            set
            {
                if (value < -1.0f || value > 1.0f)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                this.pitch = value;
            }
        }

        // PopMode
        public SoundPopMode PopMode { get; set; }

        // SoundNames
        public string? SoundNames { get; set; }

        // SubPath
        public string SubPath { get; set; } = string.Empty;

        // Tags
        public string? Tags { get; set; }

        // TransitionAware
        public bool TransitionAware { get; set; } = true;

        // Volume
        public float Volume
        {
            get => volume;
            set
            {
                if (value < 0.0f || value > 1.0f)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                this.volume = value;
            }
        }
    }
}
