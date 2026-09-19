using System;
using System.Collections.Generic;

namespace Engendro.Audio
{
    /// <summary>
    /// SoundSettings
    /// </summary>
    internal sealed class SoundSettings
    {
        // Caption
        internal string Caption { get; set; } = string.Empty;

        // Category
        internal SoundCategory Category { get; set; } = AudioManager.FXCategory;

        // MaxInstances
        internal int MaxInstances
        {
            get;
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 1);
                field = value;
            }
        } = 6;

        // Pan
        internal float Pan
        {
            get;
            set
            {
                if (value is < -1.0f or > 1.0f)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                field = value;
            }
        }

        // PauseAware
        internal bool PauseAware { get; set; } = true;

        // Pitch
        internal float Pitch
        {
            get;
            set
            {
                if (value is < -1.0f or > 1.0f)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                field = value;
            }
        }

        // PitchVariance
        internal Ratio PitchVariance { get; set; }

        // PopMode
        internal SoundPopMode PopMode { get; set; }

        // Scope
        internal SoundScope Scope { get; set; }

        // Sounds
        internal List<string> Sounds { get; set; } = [];

        // Tags
        internal string? Tags { get; set; }

        // TransitionAware
        internal bool TransitionAware { get; set; } = true;

        // Volume
        internal float Volume
        {
            get;
            set
            {
                if (value is < 0.0f or > 1.0f)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                field = value;
            }
        } = 1;
    }
}
