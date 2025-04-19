using Microsoft.Xna.Framework;

namespace Engendro.Audio
{
    /// <summary>
    /// SoundCategory
    /// </summary>
    public sealed class SoundCategory
    {
        // Constructor
        internal SoundCategory(string name)
        {
            this.Name = name;
            this.Volume = new Volume(name);
        }

        #region Internal members

        // Update
        internal void Update(GameTime gameTime) => Volume.Update(gameTime);

        #endregion

        // ContentPath
        public string ContentPath { get; set; } = string.Empty;

        // Name
        public string Name { get; }

        // Volume
        public Volume Volume { get; }

        // ToString
        public override string ToString() => Name;
    }
}
