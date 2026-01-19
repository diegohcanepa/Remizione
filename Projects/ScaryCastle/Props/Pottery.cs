using Engendro.Audio;
using Microsoft.Xna.Framework;

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
            Atlas = Atlases.Environment;
            DeathSound = Sound.Find(SoundNames.PotteryBreak);
            DepthOffset = -2;
            DisplayNameKey = "Prop.Pottery";
            HitEffect = HitEffect.Shake;
            HitTestPolygon = TestPolygon.Hotspot;
            HurtSound = Sound.Find(SoundNames.ImpactA);
            MaxHP = 5;
        }
        
        // OnDie
        protected override void OnDie()
        {
            base.OnDie();
            ShowImpactWord(ImpactWordName.CrackBlue);
        }
    }
}
