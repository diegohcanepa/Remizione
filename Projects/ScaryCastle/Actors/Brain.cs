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
            // Evaluamos el tipo de movimiento de fallback que dicta el arquetipo
            switch (archetype.FallbackMovement)
            {
                case FallbackMovementKind.Random:
                    return new CombatDecision(CombatDecisionType.RandomMove, null, target, PositioningMode.Move);

                case FallbackMovementKind.Lurk:
                    return new CombatDecision(CombatDecisionType.LurkMove, null, target, PositioningMode.Move);

                case FallbackMovementKind.None:
                default:
                    // Si no se mueve o es el fallback del fallback, se queda en el molde
                    return new CombatDecision(CombatDecisionType.None, null, target, PositioningMode.None);
            }
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

                var aggressive = archetype.AttackChance.Roll();

                // 2. Procesamiento de la Intención de Ataque / Persecución
                // Instinto de supervivencia: Si ya te tiene a tiro de Melee, ataca sí o sí ignorando la chance.
                if (isInMeleeRange || aggressive)
                {
                    var intent = archetype.SelectIntent(source, source.CombatBehavior.Intents, distance);

                    if (intent != null)
                    {
                        // CUERPO A CUERPO
                        if (intent.ActionKind == ActionKind.Proximity)
                        {
                            if (distance <= archetype.MeleeRange)
                                return new CombatDecision(CombatDecisionType.Attack, intent, target, PositioningMode.Move);
                            else
                                return new CombatDecision(CombatDecisionType.MoveNearby, null, target, PositioningMode.Move);
                        }

                        // ATAQUE A DISTANCIA
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