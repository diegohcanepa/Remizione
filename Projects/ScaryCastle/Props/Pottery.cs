using Engendro.Audio;

namespace ScaryCastle
{
    /// <summary>
    /// Pottery
    /// </summary>
    public class Pottery : BreakableProp
    {
        // Constructor
        public Pottery(GameSession session, string name)
            : base(session, name)
        {
            DeathSound = Sound.Find(SoundNames.PotteryBreak);
            DepthOffset = -2;
            HurtSound = Sound.Find(SoundNames.ImpactA);
            MaxHP = 1;
        }
    }
}
