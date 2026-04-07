using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

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
        public static CombatDecision? Decide(Actor actor, GameThing? target)
        {
            if (actor.CombatBehavior is not CombatBehavior behavior || actor.CombatBehavior.Archetype == CombatBehaviorArchetype.Lurker)
                return null;

            // 1. Logica de Supervivencia (Generalizada)
            if (ShouldAttemptFlee(actor, behavior))
                return new CombatDecision(CombatDecisionType.Flee, null);

            // 2. Logica de Ataque con Filtro de Rango
            // Calculamos distancia al cuadrado (más rápido, sin raíz cuadrada)
            if (target != null)
            {
                float dist = actor.DistanceTo(target);

                // Pasamos el target y la distancia para que el selector sepa qué elegir
                var intent = SelectWeightedIntent(actor, behavior, dist);

                if (intent != null)
                    return new CombatDecision(CombatDecisionType.Attack, intent);
            }

            // Si llegamos acá es porque el jugador está lejos 
            // y el NPC no tiene ataques que lleguen (NoResponse)
            return new CombatDecision(CombatDecisionType.None, null);
        }

        // SelectWeightedIntent
        // Calcula los pesos segun el arquetipo (Berserk, Tactical, etc)
        private static CombatIntent? SelectWeightedIntent(Actor actor, CombatBehavior behavior, float distSq)
        {
            var intents = new List<CombatIntent>();
            for (var i = 0; i < behavior.Intents.Count; i++)
            {
                // Skip contact intent because it's not a direct attack.
                if (behavior.Intents[i].Name == nameof(EffectContext.Contact))
                    continue;

                intents.Add(behavior.Intents[i]);
            }

            if (intents.Count == 0)
                return null;

            // Usamos Spans para evitar el Garbage Collector
            Span<float> weights = stackalloc float[intents.Count];
            float totalWeight = 0;
            bool anyInRange = false;

            for (int i = 0; i < intents.Count; i++)
            {
                var intent = intents[i];

                // FILTRO CRÍTICO: Si el ataque no llega, el peso es 0
                float range = intent.Range;
                if (distSq > (range * range))
                {
                    weights[i] = 0;
                    continue;
                }

                anyInRange = true;
                float w = intent.SpawnWeight;

                // BERSERK: "Si estoy muriendo, tiro los ataques fuertes"
                if (behavior.Archetype == CombatBehaviorArchetype.Berserk && actor.HPRatio < .4f)
                {
                    if (intent.Category == CombatIntentCategory.Special) w *= 3.0f;
                }

                weights[i] = w;
                totalWeight += w;
            }

            // Si el jugador está fuera de rango para TODOS los ataques
            if (!anyInRange) return null;

            // Selección Aleatoria Ponderada
            float roll = (float)Random.Shared.NextDouble() * totalWeight;
            float cumulative = 0;

            for (int i = 0; i < intents.Count; i++)
            {
                cumulative += weights[i];
                if (roll <= cumulative) return intents[i];
            }

            return null;
        }
    }
}