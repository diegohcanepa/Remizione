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
        private static CombatDecision GetFallbackMovement(CombatArchetype archetype, GameThing? target)
        {
            return new CombatDecision(archetype.AllowRandomMove ? CombatDecisionType.RandomMove : CombatDecisionType.MoveNearby, null, target, PositioningMode.Move);
        }

        #endregion

        // Decide
        public static CombatDecision? Decide(Actor source, GameThing? target)
        {
            if (source.CombatBehavior?.Archetype is not { } archetype)
                return null;

            if (target != null)
            {
                // Calculamos la métrica espacial de entrada
                float distance = source.DistanceToTarget(target);
                bool isInMeleeRange = archetype.IsInMeleeRange(source, target);

                // 1. Decisión de Huida: Filtro de pánico por poca vida + proximidad del jugador
                bool isPlayerClose = distance <= archetype.MeleeRange;

                if (isPlayerClose && source.HPRatio <= archetype.FleeHPThreshold && archetype.FleeChance.Roll())
                    return new CombatDecision(CombatDecisionType.RandomMove, null, target, PositioningMode.Move);

                // 2. Procesamiento de la Intención de Ataque / Persecución
                // Instinto de supervivencia: Si ya te tiene a tiro de Melee, ataca sí o sí ignorando la chance.
                if (isInMeleeRange || archetype.AttackChance.Roll())
                {
                    var intent = archetype.SelectIntent(source, source.CombatBehavior.Intents, distance);

                    if (intent != null)
                    {
                        // CASE 1: CUERPO A CUERPO
                        if (intent.ActionKind == ActionKind.Proximity)
                        {
                            if (distance <= archetype.MeleeRange)
                                return new CombatDecision(CombatDecisionType.Attack, intent, target, PositioningMode.Move);
                            else
                                return GetFallbackMovement(archetype, target);
                        }

                        // CASE 2: ATAQUE A DISTANCIA
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

            // 3. Fallback Determinista (Si no hay target, falló la chance de ataque, o no hay intents válidos)
            return GetFallbackMovement(archetype, target);
        }
    }
}