using Engendro.Audio;

namespace Remizione
{
    /// <summary>
    /// PostClock
    /// </summary>
    public class PostClock : BreakableProp
    {
        // Constructor
        public PostClock(GameSession session, string name)
            : base(session, name)
        {
            DeathSound = Sound.Find(SoundNames.PotteryBreak);
            DepthOffset = -8;
            HitTestSource = HitTestSource.Hotspot;
            HurtShake = new(1.5f, 0);
            HurtSound = Sound.Find(SoundNames.ImpactA);
            LootTableName = nameof(Pottery);
            MaxHealth = 26;
            PreventKnockback = true;
            ShakeOnHit = true;
        }
    }
}
