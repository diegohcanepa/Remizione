using System;

namespace ScaryCastle
{
    /// <summary>
    /// Paquete de datos liviano (Stack allocated, 0 GC) para la decisión de combate.
    /// </summary>
    public readonly record struct CombatDecision(CombatDecisionType Type, CombatIntent? Intent);

    /// <summary>
    /// Motor de decisiones de combate. 
    /// Solo procesa lógica usando los datos del actor y su arquetipo.
    /// </summary>
    public static class Brain
    {
        /// <summary>
        /// Determina la acción a realizar en el frame actual.
        /// </summary>
        public static CombatDecision Decide(Actor actor, GameThing? target)
        {
            // C# 14 Pattern Matching para extraer el arquetipo
            if (actor.CombatBehavior == null)
                return new CombatDecision(CombatDecisionType.None, null);

            var archetype = actor.CombatBehavior.Archetype;

            // 1. PRIORIDAD: Supervivencia (Check de huida)
            if (actor.HPRatio <= archetype.FleeHPThreshold && Random.Shared.NextDouble() < archetype.FleeChance)
                return new CombatDecision(CombatDecisionType.Flee, null);

            // 2. LÓGICA DE COMBATE / MOVIMIENTO
            if (target != null)
            {
                float distance = actor.DistanceTo(target);

                // El arquetipo decide si "se anima" a atacar según su AttackChance
                if (Random.Shared.NextDouble() < archetype.AttackChance)
                {
                    // Delegamos la selección del ataque al arquetipo
                    var intent = archetype.SelectIntent(actor, actor.CombatBehavior.Intents, distance);

                    if (intent != null)
                    {
                        // Crítica de diseño: El Lurker usa Charge para representar su salto/mordida repentina.
                        // El resto usa Attack normal.
                        var type = archetype is LurkerArchetype ? CombatDecisionType.Charge : CombatDecisionType.Attack;
                        return new CombatDecision(type, intent);
                    }
                }

                // 3. PLAN B: Si no hay ataque o falló el azar, se mueve según el estilo del bicho.
                // El Lurker devolverá 'Move' (para merodear cerca).
                return new CombatDecision(archetype.IdleMoveType, null);
            }

            return new CombatDecision(CombatDecisionType.None, null);
        }
    }
}