using System;

namespace ScaryCastle
{
    /// <summary>
    /// Brain
    /// </summary>
    public static class Brain
    {
        // Decide
        public static CombatIntentDescriptor? Decide(CombatBehavior behavior, int currentHp, int maxHp)
        {
            var intents = behavior.IntentDescriptors;
            int count = intents.Count;

            if (count == 0)
                return null;

            if (count == 1)
                return intents[0];

            Span<float> weights = stackalloc float[count];
            float totalWeight = 0;
            float hpPercent = (float)currentHp / maxHp;

            for (int i = 0; i < count; i++)
            {
                var intent = intents[i];
                float w = intent.SpawnWeight;

                switch (behavior.Archetype)
                {
                    // Aggresive
                    case CombatBehaviorArchetype.Aggressive:
                        if (intent.Category == IntentCategory.Attack)
                            w *= 2.5f;
                        break;

                    // Coward
                    case CombatBehaviorArchetype.Coward:
                        if (hpPercent < 0.35f && (intent.Category == IntentCategory.Defense ||
                            intent.Category == IntentCategory.Healing))
                            w *= 4.0f;
                        break;

                    // Erratic
                    case CombatBehaviorArchetype.Erratic:
                        w = 1;
                        break;

                    case CombatBehaviorArchetype.Berserk:
                        if (intent.Category == IntentCategory.Attack)
                        {
                            // Multiplicador agresivo: a menor HP, mayor peso.
                            // A 10% de HP, el peso del ataque se multiplica por ~10.
                            float berserkMult = 1.0f + ((1.0f - hpPercent) * 10.0f);
                            w *= berserkMult;
                        }
                        else if (intent.Category is IntentCategory.Defense or IntentCategory.Healing)
                        {
                            // El Berserk desprecia la defensa a medida que se descontrola
                            w *= hpPercent;
                        }
                        break;

                    case CombatBehaviorArchetype.Simple:
                    default:
                        break;
                }

                weights[i] = w;
                totalWeight += w;
            }

            float roll = (float)Random.Shared.NextDouble() * totalWeight;
            float cumulative = 0;

            for (int i = 0; i < count; i++)
            {
                cumulative += weights[i];
                if (roll <= cumulative)
                    return intents[i];
            }

            return intents[0];
        }
    }
}