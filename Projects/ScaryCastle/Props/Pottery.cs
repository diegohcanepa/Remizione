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
            CanBeHit = true;
            DeathSound = Sound.Find(SoundNames.PotteryBreak);
            DepthOffset = -2;
            DisplayNameKey = "Prop.Pottery";
            HurtSound = Sound.Find(SoundNames.ImpactA);
            MaxHP = 1;
            Verb = Verb.Lift;
        }
    }
}
