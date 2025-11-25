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
            Atlas = Atlases.Environment;
            DeathSound = Sound.Find(SoundNames.PotteryBreak);
            DepthOffset = -4;
            DisplayNameKey = "Prop.Pottery";
            HitEffect = HitEffect.Shake;
            HitTestPolygon = TestPolygon.Hotspot;
            HurtSound = Sound.Find(SoundNames.ImpactA);
            MaxHP = 5;
            PreventKnockback = true;
        }
    }
}
