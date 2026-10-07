using Microsoft.Xna.Framework;

namespace Engendro.Audio
{
    /// <summary>
    /// AudioCategory
    /// </summary>
    public sealed class AudioCategory
    {
        // Constructor
        internal AudioCategory(AudioCategoryName category)
        {
            this.Name = category;
            this.Volume = new Volume(Name.ToString());
        }

        #region Internal members

        // Update
        internal void Update(GameTime gameTime)
        {
            Volume.Update(gameTime);
        }

        #endregion

        // ContentPath
        public string ContentPath { get; set; } = string.Empty;

        // Name
        public AudioCategoryName Name { get; }

        // Volume
        public Volume Volume { get; }

        // ToString
        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
