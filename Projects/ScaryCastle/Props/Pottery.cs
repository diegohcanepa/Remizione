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
            ApproachBehavior = ApproachBehavior.FaceToFace;
            CanBeHit = true;
            DeathSound = Sound.Find(SoundNames.PotteryBreak);
            DepthOffset = -2;
            DisplayNameKey = "Prop.Pottery";
            Faction = Faction.Evil;
            HurtSound = Sound.Find(SoundNames.ImpactA);
            IsLiftable = true;
            MaxHP = 1;
        }
    }
}
