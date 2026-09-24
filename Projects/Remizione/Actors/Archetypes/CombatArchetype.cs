using Engendro;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// CombatArchetype
    /// Define el rol táctico base y la conducta de un tipo de enemigo.
    /// </summary>
    public abstract class CombatArchetype
    {
        // Probabilidad (0 a 1) de que el NPC intente un ataque en su turno.
        public abstract Ratio AttackChance { get; }

        // Distancia a la que detecta al jugador y entra en hostilidad.
        public virtual float AwarenessRange => 100;

        // Movimiento por defecto cuando está tranquilo o no ataca.
        public virtual FallbackMovementKind FallbackMovement => FallbackMovementKind.None;

        // Probabilidad de huir una vez alcanzado el umbral de vida.
        public abstract Ratio FleeChance { get; }

        // Umbral de vida (0 a 1) por debajo del cual el NPC considera huir.
        public abstract Ratio FleeHPThreshold { get; }

        // Distancia MÁXIMA que camina en un solo turno de persecución
        public virtual float MaxStepPerTurn => 10;

        // Distancia máxima a la que se considera en rango cuerpo a cuerpo.
        public virtual float MeleeRange => 30;

        // Distancia a la que el enemigo pierde de vista al jugador y se desactiva.
        public virtual float LoseSightRange => 200;

        /// <summary>
        /// Calcula el peso de un ataque según su rango de distancia.
        /// </summary>
        public virtual float GetIntentWeight(CombatIntent intent, Actor actor, float distance)
        {
            if (distance < intent.MinRange || distance > intent.MaxRange)
                return 0;

            return intent.SpawnWeight;
        }

        /// <summary>
        /// Selecciona un ataque de la lista disponible usando selección ponderada en Stack (sin GC).
        /// </summary>
        public virtual CombatIntent? SelectIntent(Actor actor, IList<CombatIntent> intents, float distance)
        {
            if (intents == null || intents.Count == 0)
                return null;

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

            if (!anyInRange || totalWeight <= 0)
                return null;

            float roll = (float)Random.Shared.NextDouble() * totalWeight;
            float cumulative = 0;

            for (int i = 0; i < intents.Count; i++)
            {
                cumulative += weights[i];
                if (roll <= cumulative)
                    return intents[i];
            }

            return null;
        }
    }
}