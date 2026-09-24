namespace Remizione
{
    /// <summary>
    /// Brain
    /// </summary>
    public static class Brain
    {
        private static CombatDecision GetFallbackMovement(CombatArchetype archetype, GameThing? target)
        {
            return archetype.FallbackMovement switch
            {
                FallbackMovementKind.Random => new CombatDecision(CombatDecisionType.RandomMove, null, target, PositioningMode.Move),
                FallbackMovementKind.Lurk => new CombatDecision(CombatDecisionType.LurkMove, null, target, PositioningMode.Move),
                _ => new CombatDecision(CombatDecisionType.None, null, target, PositioningMode.None),
            };
        }

        public static CombatDecision? Decide(Actor source, GameThing? target)
        {
            if (source.CombatBehavior?.Archetype is not { } archetype)
                return null;

            if (target == null)
            {
                source.IsHostile = false;
                return GetFallbackMovement(archetype, target);
            }

            // 1. CÁLCULO DE DISTANCIA
            float distance = source.DistanceToTarget(target);

            // 2. HISTÉRESIS DE HOSTILIDAD Y VISIÓN
            if (source.IsHostile)
            {
                // Si ya te vio y te está persiguiendo, mantenemos el aggro usando solo la distancia.
                // (Diseño: Si te escondés detrás de una piedra, el bicho no se olvida mágicamente de vos,
                // te sigue buscando hasta que salgas del LoseSightRange).
                if (distance > archetype.LoseSightRange)
                    source.IsHostile = false;
            }
            else
            {
                // Está pacífico. Para activarse, primero debe estar en rango de distancia.
                if (distance <= archetype.AwarenessRange)
                {
                    // 1. Chequeo de obstáculos (Raycast contra el nivel)
                    bool hasLineOfSight = source.Room?.WalkArea?.InLineOfSight(source.Position, target.Position, source, out _) == true;

                    if (hasLineOfSight)
                    {
                        // 2. Chequeo de espaldas
                        bool isFacingPlayer = source.IsFacingTarget(target);

                        // 3. Chequeo de proximidad/ruido (un 30% del rango total)
                        bool closeEnoughToHear = distance <= (archetype.AwarenessRange * 0.3f);

                        // Se vuelve hostil si lo está mirando de frente, o si le respiraste en la nuca
                        if (isFacingPlayer || closeEnoughToHear)
                        {
                            source.IsHostile = true;
                        }
                    }
                }
            }

            // 3. EARLY EXIT: FALLBACK PACÍFICO
            if (!source.IsHostile)
                return GetFallbackMovement(archetype, target);

            // 4. METRICAS CRÍTICAS
            bool isInMeleeRange = distance <= archetype.MeleeRange;

            // 5. INSTINTO DE HUIDA
            if (isInMeleeRange && source.HPRatio <= archetype.FleeHPThreshold && archetype.FleeChance.Roll())
                return new CombatDecision(CombatDecisionType.RandomMove, null, target, PositioningMode.Move);

            // 6. PROCESAMIENTO DE ATAQUE
            var aggressive = archetype.AttackChance.Roll();

            if (isInMeleeRange || aggressive)
            {
                var intent = archetype.SelectIntent(source, source.CombatBehavior.Intents, distance);

                if (intent != null)
                {
                    // Como SelectIntent ya validó que estamos dentro de los Min/Max Range del ataque, ejecutamos directo.
                    return intent.ActionKind switch
                    {
                        ActionKind.Proximity => new CombatDecision(CombatDecisionType.Attack, intent, target, PositioningMode.Move),
                        ActionKind.Projectile => new CombatDecision(CombatDecisionType.Attack, intent, target, PositioningMode.MoveOnY),
                        ActionKind.InPlace => new CombatDecision(CombatDecisionType.Attack, intent, target, PositioningMode.None),
                        _ => new CombatDecision(CombatDecisionType.None, null, target, PositioningMode.None)
                    };
                }
            }

            // 7. PERSECUCIÓN (Es hostil, pero no había ataques en rango o falló la agresividad)
            return new CombatDecision(CombatDecisionType.MoveNearby, null, target, PositioningMode.Move);
        }

        // UpdatePerception
        public static void UpdatePerception(Actor source, GameThing? target)
        {
            if (target == null || source.CombatBehavior?.Archetype is not { } archetype)
                return;

            float distance = source.DistanceToTarget(target);

            // Si ya es hostil, solo pierde el foco si se aleja más allá de LoseSightRange
            if (source.IsHostile)
            {
                if (distance > archetype.LoseSightRange)
                {
                    source.IsHostile = false;
                }
            }
            else
            {
                // Si está tranquilo, se activa en tiempo real al cruzar la barrera de visión
                if (distance <= archetype.AwarenessRange)
                {
                    // (Opcional: aquí podrías sumar chequeo de línea de visión o espalda si lo usas)
                    source.IsHostile = true;
                }
            }
        }
    }
}