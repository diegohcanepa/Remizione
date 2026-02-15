using System;

namespace ScaryCastle
{
    /// <summary>
    /// Brain
    /// </summary>
    public static class Brain
    {
        // Decide
        public static CombatIntent? Decide(ProceduralActor actor, GameThing target)
        {
            if (actor.CombatBehavior is not CombatBehavior behavior)
                return null;

            var intents = behavior.IntentDescriptors;
            int count = intents.Count;

            if (count == 0)
                return null;

            if (count == 1)
                return intents[0];

            Span<float> weights = stackalloc float[count];
            float totalWeight = 0;
            float hpPercent = (float)actor.HP / actor.MaxHP;

            for (int i = 0; i < count; i++)
            {
                var intent = intents[i];
                float w = intent.SpawnWeight;

                switch (behavior.Archetype)
                {
                    // BERSERK: "Si estoy muriendo, tiro los ataques fuertes"
                    case CombatBehaviorArchetype.Berserk:
                        if (hpPercent < 0.40f) // Menos del 40% de vida
                        {
                            if (intent.Category == CombatIntentCategory.Special)
                            {
                                // Multiplicamos x3 la chance de tirar el especial
                                // Esto acelera el final del combate (para bien o para mal)
                                w *= 3.0f;
                            }
                        }
                        break;

                    // TACTICAL: Respeta el diseño original (Default)
                    case CombatBehaviorArchetype.Tactical:
                    default:
                        break;
                }

                weights[i] = w;
                totalWeight += w;
            }

            // Selección Aleatoria Ponderada (Weighted Random)
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