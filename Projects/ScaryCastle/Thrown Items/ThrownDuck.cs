using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// ThrownDuck
    /// </summary>
    public sealed class ThrownDuck : ThrownItem
    {
        // Constructor
        public ThrownDuck(GameSession session)
            : base(session, new Vector2(110, -50), .8f, .6f, 500, 10)
        {
            const string basePrefix = "ThrowableDuck";

            ImpactSound = Sound.Find(basePrefix);
            Shadow.Image = Atlas?.GetImage(basePrefix + "Shadow");

            var animation = AddAnimation(AnimationNames.Default);
            animation.AddFrame(basePrefix, 1000);
        }
    }
}
