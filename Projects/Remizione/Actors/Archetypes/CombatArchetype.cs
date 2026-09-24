using System;
using System.Collections.Generic;

namespace Remizione
{
    public abstract class CombatArchetype
    {
        // Rangos de Percepción
        public virtual float AwarenessRange => 100f;
        public virtual float LoseSightRange => 200f;

        // Tiempo de pausa/enfriamiento del enemigo post-ataque (en segundos)
        public virtual float CooldownDuration => 1.5f;

        // ExposedPauseDuration
        public virtual float ExposedPauseDuration => 4;

        // ExecuteMovement
        public virtual void ExecuteMovement(Actor source, GameThing target)
        {
            source.MoveNearby(target);
        }

        // Velocidad o tramo máximo de movimiento por paso
        public virtual float MaxStepPerTurn => 20f;

        // Tipo de movimiento cuando no ataca
        public virtual FallbackMovementKind FallbackMovement => FallbackMovementKind.None;

        /// <summary>
        /// Selecciona un ataque disponible basándose puramente en la distancia actual.
        /// Si hay varios válidos, elige uno de forma simple sin cálculos pesados.
        /// </summary>
        public virtual CombatIntent? SelectIntent(Actor source, IList<CombatIntent> intents, float distance)
        {
            if (intents == null || intents.Count == 0)
                return null;

            // 1. Filtramos cuáles ataques están dentro del rango permitido (MinRange <= distance <= MaxRange)
            // Como las listas son de 1 a 3 elementos máximo, una iteración directa en Stack es ultra rápida.
            CombatIntent? primaryCandidate = null;
            CombatIntent? secondaryCandidate = null;

            for (int i = 0; i < intents.Count; i++)
            {
                var intent = intents[i];
                if (distance >= intent.MinRange && distance <= intent.MaxRange)
                {
                    if (primaryCandidate == null)
                        primaryCandidate = intent;
                    else if (secondaryCandidate == null)
                        secondaryCandidate = intent;
                }
            }

            // 2. Si solo hay uno en rango (el 90% de los casos), lo devuelve directo
            if (secondaryCandidate == null)
                return primaryCandidate;

            // 3. Si hay 2 ataques válidos al mismo tiempo (ej. Mordisco Básico vs Manotazo Fuerte),
            // hace una elección 50/50 instantánea sin pesos ni matrices.
            return Random.Shared.Next(2) == 0 ? primaryCandidate : secondaryCandidate;
        }
    }
}