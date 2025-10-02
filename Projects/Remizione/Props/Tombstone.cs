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
            HitTestPolygon = TestPolygon.Hotspot;
            HurtSound = Sound.Find(SoundNames.ImpactA);
            MaxHP = 8;
            PlacementPhase = PlacementPhase.ArtificialObject;
            ResistanceTableName = "ExplosiveOnly";
            PreventKnockback = true;
            ShakeOnHit = true;
        }
    }
}
