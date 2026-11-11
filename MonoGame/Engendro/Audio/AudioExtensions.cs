using System;

namespace Engendro.Audio
{
    /// <summary>
    /// AudioExtensions
    /// </summary>
    public static class AudioExtensions
    {
        // ToAudioCategories
        public static AudioCategories ToAudioCategories(this AudioCategoryName name)
        {
            return name switch
            {
                AudioCategoryName.FX => AudioCategories.FX,
                AudioCategoryName.Ambience => AudioCategories.Ambience,
                AudioCategoryName.Voice => AudioCategories.Voice,
                AudioCategoryName.Music => AudioCategories.Music,
                _ => throw new ArgumentOutOfRangeException(nameof(name), name, null)
            };
        }
    }
}