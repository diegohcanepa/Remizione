using Adberration.Scripting;
using Engendro.Audio;

namespace Remizione
{
    /// <summary>
    /// Pottery
    /// </summary>
    public class Pottery : Prop
    {
        private const string CrackedSuffix = "_Cracked";

        // Constructor
        public Pottery(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Props;
            ApproachBehavior = ApproachBehavior.ClosestSide;
            Verb = Verb.Attack;
            IsHittable = true;
            DeathSound = Sound.Find(SoundNames.PotteryBreak);
            DepthOffset = -2;
            DisplayNameKey = "Prop.Pottery";
        }

        #region Protected members

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

        #endregion

        // CanHideLoot
        [ScriptProperty]
        public bool CanHideLoot { get; set; }

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
