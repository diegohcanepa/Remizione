using System;

namespace Engendro.Audio
{
    /// <summary>
    /// SoundSettings
    /// </summary>
    public sealed class SoundSettings
    {

        // Caption
        public string Caption { get; set; } = string.Empty;

        // Category
        public SoundCategory Category { get; set; } = AudioManager.FXCategory;

        // MaxInstances
        public int MaxInstances
        {
            get;
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 1);
                field = value;
            }
        } = 1;

        // Pan
        public float Pan
        {
            get;
            set
            {
                if (value < -1.0f || value > 1.0f)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                field = value;
            }
        }

        // PauseAware
        public bool PauseAware { get; set; } = true;

        // Pitch
        public float Pitch
        {
            get;
            set
            {
                if (value < -1.0f || value > 1.0f)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                field = value;
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
            get;
            set
            {
                if (value < 0.0f || value > 1.0f)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                field = value;
            }
        } = 1;
    }
}
