using Engendro;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// CombatArchetype
    /// </summary>
    public abstract class CombatArchetype
    {
        // AttackChance
        // Probabilidad (0 a 1) de que el NPC intente un ataque en su turno.
        // Un valor bajo (0.2) crea un comportamiento de "acecho".
        public abstract Ratio AttackChance { get; }

        // FallbackMovement
        public virtual FallbackMovementKind FallbackMovement => FallbackMovementKind.None;

        // FleeChance (Probabilidad de que efectivamente huya una vez herido.)
        public abstract Ratio FleeChance { get; }

        // FleeHPThreshold
        // Umbral de vida (0 a 1) por debajo del cual el NPC considera huir.
        public abstract Ratio FleeHPThreshold { get; }

        // GetDecisionType
        public virtual CombatDecisionType GetDecisionType(CombatIntent intent)
        {
            return CombatDecisionType.Attack;
        }

        // GetIntentWeight
        // Calcula el peso específico de un intent sin descartar proyectiles por distancia.
        public virtual float GetIntentWeight(CombatIntent intent, Actor actor, float distance)
        {
            // Si el jugador está fuera del rango operativo de este ataque en particular, el peso es cero.
            if (distance < intent.MinRange || distance > intent.MaxRange)
                return 0;

            // Si está dentro de sus límites físicos, se respeta su peso base.
            return intent.SpawnWeight;
        }

        // IsInMeleeRange
        public bool IsInMeleeRange(GameThing source, GameThing target)
        {
            return source.DistanceToTarget(target) <= MeleeRange;
        }

        // MeleeRange
        public virtual int MeleeRange => 30;

        // SelectIntent
        // Selecciona un ataque de la lista disponible basándose en pesos y distancia.
        public virtual CombatIntent? SelectIntent(Actor actor, IList<CombatIntent> intents, float distance)
        {
            if (intents == null || intents.Count == 0)
                return null;

            // Usamos stackalloc para evitar asignaciones en el Heap (GC Friendly)
            Span<float> weights = stackalloc float[intents.Count];
            float totalWeight = 0;
            bool anyInRange = false;

            for (int i = 0; i < intents.Count; i++)
            {
                var intent = intents[i];
                float weight = GetIntentWeight(intent, actor, distance);

                if (weight > 0)
                    anyInRange = true;

                weights[i] = weight;
                totalWeight += weight;
            }

            // Si nada está en rango o todos los pesos son 0
            if (!anyInRange || totalWeight <= 0)
                return null;

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