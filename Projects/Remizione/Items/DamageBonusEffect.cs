using Engendro;

namespace Remizione.Items
{
    /// <summary>
    /// DamageBonusEffect
    /// </summary>
    internal class DamageBonusEffect : ItemEffect
    {
        // Constructor
        public DamageBonusEffect(float bonus, bool percentage)
            : base(ItemEffectTiming.AfterAllEffects)
        {
            this.Bonus = bonus;
            this.Percentage = percentage;
        }

        // OnApply
        protected override void OnApply(GameThing target)
        {
            if (Percentage)
                target.CumulativeDamage *= Bonus;
            else
                target.CumulativeDamage += Bonus;
        }

        // Bonus
        public float Bonus { get; }

        // GetLocalizedDescription
        public override string GetLocalizedDescription()
        {
            const string bonusArg = "Bonus";

            var value = "+" + Bonus.ToString();
            if (Percentage)
                value += "%";

            var text = TextRepository.GetValue(TextRepositoryKey, (bonusArg, value));
            return text;
        }

        // Percentage
        public bool Percentage { get; }
    }
}
