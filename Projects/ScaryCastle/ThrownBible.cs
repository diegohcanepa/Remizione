using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// ThrownBible
    /// </summary>
    public sealed class ThrownBible : ThrownObject
    {
        // Constructor
        public ThrownBible(GameSession session)
            : base(session, .75f, new Vector2(110, -50), .8f, .6f, 500, 10)
        {
            const string basePrefix = "Bible";

            ImpactSound = Sound.Find(basePrefix);
            Shadow.Image = Atlas?.FindImage(basePrefix + "Shadow");
            Scale = new Vector2(.5f);

            var animation = AddAnimation(AnimationNames.Default);
            animation.AddFrame(basePrefix, 1000);
        }
    }
}
