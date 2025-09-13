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
            AllowInteraction = false;
            DeathSound = Sound.Find(SoundNames.PotteryBreak);
            DepthOffset = -4;
            DisplayNameKey = "@Prop.Pottery";
            HitTestSource = HitTestSource.Hotspot;
            HurtShake = new(1.5f, 0);
            HurtSound = Sound.Find(SoundNames.ImpactA);
            LootTableName = nameof(Pottery);
            MaxHealth = 6;
            PreventKnockback = true;
            ShakeOnHit = true;
        }
    }
}
