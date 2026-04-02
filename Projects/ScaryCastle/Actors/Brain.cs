using System;

namespace ScaryCastle
{
    // CombatDecision
    public record struct CombatDecision(CombatDecisionType Type, CombatIntent? Intent);

    /// <summary>
    /// Brain
    /// </summary>
    public static class Brain
    {
        #region Private members

        // ShouldAttemptFlee
        private static bool ShouldAttemptFlee(Actor actor, CombatBehavior behavior)
        {
            // Definimos umbral de vida y chance de exito por arquetipo
            var (hpThreshold, fleeChance) = behavior.Archetype switch
            {
                CombatBehaviorArchetype.Coward => (.35f, .70f),   // Huye rapido y casi siempre
                CombatBehaviorArchetype.Tactical => (.15f, .40f),  // Huye solo si es critico y con cautela
                CombatBehaviorArchetype.Berserk => (.05f, .10f),   // Casi nunca huye, es un suicida
                _ => (0f, 0f)
            };

            return actor.HPRatio > hpThreshold ? false : Random.Shared.NextDouble() < fleeChance;
        }

        // ShouldAttack
        private static bool ShouldAttack(Actor actor, CombatBehavior behavior)
        {
            // Definimos umbral de vida y chance de exito por arquetipo
            var chance = behavior.Archetype switch
            {
                CombatBehaviorArchetype.Coward => .5f,   // Huye rapido y casi siempre
                _ => 1f
            };

            return Random.Shared.NextDouble() < chance;
        }

        #endregion

        // Decide
        public static CombatDecision? Decide(Actor actor)
        {
            if (actor.CombatBehavior is not CombatBehavior behavior || actor.CombatBehavior.Archetype == CombatBehaviorArchetype.Lurker)
                return null;

            // 1. Logica de Supervivencia (Generalizada)
            if (ShouldAttemptFlee(actor, behavior))
                return new CombatDecision(CombatDecisionType.Flee, null);

            // 2. Reacciona?
            if (!ShouldAttack(actor, behavior))
                return new CombatDecision(CombatDecisionType.Passive, null);

            // 3. Logica de Ataque
            if (SelectWeightedIntent(actor, behavior) is not CombatIntent intent)
                return null;
            else
                return new CombatDecision(CombatDecisionType.Attack, intent);
        }

        // SelectWeightedIntent
        // Calcula los pesos segun el arquetipo (Berserk, Tactical, etc)
        private static CombatIntent? SelectWeightedIntent(Actor actor, CombatBehavior behavior)
        {
            var intents = behavior.Intents;
            int count = intents.Count;
            if (count == 0) return null;

            Span<float> weights = stackalloc float[count];
            float totalWeight = 0;

            for (int i = 0; i < count; i++)
            {
                var intent = intents[i];
                float w = intent.SpawnWeight;

                // BERSERK: Potencia especiales al morir
                if (behavior.Archetype == CombatBehaviorArchetype.Berserk && actor.HPRatio < .4f)
                {
                    if (intent.Category == CombatIntentCategory.Special)
                        w *= 3.0f;
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