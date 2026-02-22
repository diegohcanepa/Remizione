using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// Bible
    /// </summary>
    public sealed class Bible : ThrownObject
    {
        // Constructor
        public Bible(GameSession session)
            : base(session, .7f, .75f, new Vector2(110, -50), .8f, .6f, 500, 10)
        {
            const string prefix = nameof(Bible);

            BreakSound = Sound.Find(prefix + "Break");
            ImpactSound = Sound.Find(prefix);
            Shadow.Image = Atlas?.FindImage(prefix + "Shadow");
            Scale = new Vector2(.5f);

            var animation = AddAnimation(AnimationNames.Default);
            animation.AddFrame(prefix, 1000);
        }
    }
}
