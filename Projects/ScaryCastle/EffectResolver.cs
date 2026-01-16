using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// EffectResolver
    /// </summary>
    public static class EffectResolver
    {
        // Apply
        public static void Apply(IList<EffectDefinition> effects, GameThing source, GameThing target)
        {
            if (effects.Count == 0)
                return;

            foreach (var effect in effects)
            {
                // Play sound
                if (effect.Sound != null)
                    source.PlaySound(effect.Sound);

                // Calculate amount
                int amount = effect.Amount == null ? 0 : effect.Amount.Roll();

                // Apply logic to target
                switch (effect.EffectType)
                {
                    // Heal
                    case EffectType.Heal:
                        target.HP += amount;
                        break;

                    // Damage
                    case EffectType.Damage:
                        // TODO: Check impact word arg
                        target.TakeDamage(source, amount, effect.DamageType, ImpactWordName.None);
                        break;
                }
            }
        }
    }
}