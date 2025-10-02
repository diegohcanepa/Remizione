using Engendro.Audio;

namespace Remizione
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
            DepthOffset = -4;
            HitTestPolygon = TestPolygon.Hotspot;
            HurtSound = Sound.Find(SoundNames.ImpactA);
            LootTableName = nameof(Pottery);
            MaxHP = 6;
            PreventKnockback = true;
            ShakeOnHit = true;
        }
    }
}
