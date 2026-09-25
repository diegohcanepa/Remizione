using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// CombatArchetype
    /// </summary>
    public abstract class CombatArchetype
    {
        // AwarenessRange (perception range)
        public virtual float AwarenessRange => 100f;

        // CooldownDuration (post-attack cooldown in seconds)
        public virtual float CooldownDuration => 1.5f;

        // CounterAttackChance
        public virtual float CounterAttackChance => .35f;

        // ExecuteStepMovement
        public virtual void ExecuteStepMovement(Actor source, GameThing target, float stepDistance, float currentDistance)
        {
            // 1. EVALUACIÓN DE HUIDA (FALLBACK)
            // Acá podés sumar condiciones, por ejemplo: si la vida de source < 20% y Fallback == Flee
            if (FallbackMovement == FallbackMovementKind.Flee)
            {
                // Calcular vector opuesto para alejarse
                Vector2 directionAway = Vector2.Normalize(source.Position - target.Position);
                Vector2 escapePoint = source.Position + (directionAway * stepDistance);
                source.MoveTo(escapePoint);
                return;
            }

            // 2. EVALUACIÓN DE KITING (RANGO)
            // Si el NPC solo tiene ataques a distancia y el jugador está demasiado cerca
            // (Opcional, pero vital si tenés enemigos con escopetas/magia)
            /*
            float minAttackRange = GetMinAttackRange(source.CombatBehavior.Intents);
            if (currentDistance < minAttackRange)
            {
                Vector2 directionAway = Vector2.Normalize(source.Position - target.Position);
                source.MoveTo(source.Position + (directionAway * stepDistance));
                return;
            }
            */

            // 3. DEFAULT: AVANZAR AL COMBATE
            source.MoveTowards(target.Position, stepDistance);
        }

        // ExposedPauseDuration
        public virtual float ExposedPauseDuration => 4;

        // FallbackMovement
        public virtual FallbackMovementKind FallbackMovement => FallbackMovementKind.None;

        // LoseSightRange
        public virtual float LoseSightRange => 200;

        // MaxStepPerTurn
        public virtual float MaxStepPerTurn => 20;

        // SelectIntent
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