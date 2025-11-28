using Engendro.Audio;

namespace Remizione
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
            HitEffect = HitEffect.Shake;
            HitTestPolygon = TestPolygon.Collider;
            HurtSound = Sound.Find(SoundNames.ImpactA);
            MaxHP = 8;
            ResistanceTableName = "ExplosiveOnly";
            PreventKnockback = true;
        }
    }
}
