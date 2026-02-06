using Engendro.Audio;

namespace ScaryCastle
{
    /// <summary>
    /// Tombstone
    /// </summary>
    public class Tombstone : BreakableProp
    {
        // Constructor
        public Tombstone(GameSession session, string name)
            : base(session, name)
        {
            DeathSound = Sound.Find(SoundNames.PotteryBreak);
            HurtSound = Sound.Find(SoundNames.ImpactA);
            MaxHP = 8;
            ResistanceTableName = "ExplosiveOnly";
        }
    }
}
