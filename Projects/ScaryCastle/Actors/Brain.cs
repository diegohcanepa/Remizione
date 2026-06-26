using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// Brain
    /// </summary>
    public static class Brain
    {
        #region Private members

        // GetFallbackMovement
        private static CombatDecision GetFallbackMovement(CombatArchetype archetype, GameThing target)
        {
            if (archetype.AllowRandomMove)
                return new CombatDecision(CombatDecisionType.RandomMove, null, target, PositioningMode.Move);

            return new CombatDecision(CombatDecisionType.MoveNearby, null, target, PositioningMode.Move);
        }

        #endregion

        // Decide
        public static CombatDecision? Decide(Actor source, GameThing? target)
        {
            if (source.CombatBehavior?.Archetype is not { } archetype)
                return null;

            bool isCornered = target != null && source.IsCornered(target);

            // 1. Decisión de Huida: Filtro de pánico si está reventado y hay espacio físico para escapar
            if (!isCornered && source.HPRatio <= archetype.FleeHPThreshold && Random.Shared.NextDouble() < archetype.FleeChance)
                return new CombatDecision(CombatDecisionType.Flee, null, target, PositioningMode.Move);

            // 2. Procesamiento de la Intención de Ataque / Persecución
            if (target != null)
            {
                float distance = CombatArchetype.DistanceToTarget(source, target);

                // Flag crítico: ¿Ya está físicamente en distancia de meter un viaje cuerpo a cuerpo?
                bool isInMeleeRange = archetype.IsInMeleeRange(source, target);

                // Si está acorralado O si ya te tiene en rango de Melee, IGNORAMOS el AttackChance y va a buscarte sí o sí.
                if (isInMeleeRange || isCornered || Random.Shared.NextDouble() < archetype.AttackChance)
                {
                    // Selecciona el ataque basado en los pesos del arquetipo
                    var intent = archetype.SelectIntent(source, source.CombatBehavior.Intents, distance);

                    if (intent != null)
                    {
                        // CASE 1: CUERPO A CUERPO (Espada, Mordisco, Tajo)
                        if (intent.ActionKind == ActionKind.Proximity)
                        {
                            // Como validamos arriba, si entró acá y es melee, ya sabemos que distance <= MeleeAttackRange
                            if (distance <= archetype.MeleeRange)
                            {
                                return new CombatDecision(CombatDecisionType.Attack, intent, target, PositioningMode.Move);
                            }
                            else
                            {
                                return GetFallbackMovement(archetype, target);
                            }
                        }

                        // CASE 2: ATAQUE A DISTANCIA (Flechas, Magia, Escupitajo)
                        if (intent.ActionKind == ActionKind.Projectile)
                        {
                            return new CombatDecision(CombatDecisionType.Attack, intent, target, PositioningMode.MoveOnY);
                        }
                        else if (intent.ActionKind == ActionKind.InPlace)
                        {
                            return new CombatDecision(CombatDecisionType.Attack, intent, target, PositioningMode.None);
                        }
                    }
                }
            }

            // 3. Fallback Determinista (Si falló la chance de ataque o no hay intents válidos)
            // Si es un obstáculo ambiental (Rata), se mueve al azar de forma caótica.
            if (archetype.AllowRandomMove)
                return new CombatDecision(CombatDecisionType.RandomMove, null, target, PositioningMode.Move);

            // Si es un enemigo inteligente, usa el pulso para ganar terreno y achicarte el pasillo.
            return new CombatDecision(CombatDecisionType.MoveNearby, null, target, PositioningMode.Move);
        }
    }
}