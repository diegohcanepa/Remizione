using Engendro.Audio;

namespace ScaryCastle
{
    /// <summary>
    /// Pottery
    /// </summary>
    public class Pottery : BreakableProp
    {
        private const string CrackedSuffix = "_Cracked";

        // Constructor
        public Pottery(GameSession session, string name)
            : base(session, name)
        {
            ApproachBehavior = ApproachBehavior.ClosestSide;
            Verb = Verb.Lift;
            IsHittable = true;
            DeathSound = Sound.Find(SoundNames.PotteryBreak);
            DepthOffset = -2;
            DisplayNameKey = "Prop.Pottery";
            //HurtSound = Sound.Find(SoundNames.ImpactA);
            IsLiftable = true;
        }

        // OnTakeDamage
        protected override void OnTakeDamage(GameThing attacker, int amount, DamageType damageType)
        {
            base.OnTakeDamage(attacker, amount, damageType);

            if (!IsDead && HPRatio < 1)
            {
                Sprite.RenderImage = Atlas?.FindImage($"{DeclaredName}{CrackedSuffix}");
                PlaySound(SoundNames.Break);
            }
        }

        // GetThrowableImageName
        public override string GetThrowableImageName()
        {
            var result = base.GetThrowableImageName();
            if (HPRatio < 1)
                result += CrackedSuffix;

            return result;
        }
    }
}
