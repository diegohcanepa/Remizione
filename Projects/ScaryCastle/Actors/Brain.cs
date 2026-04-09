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
        // Decide
        public static CombatDecision Decide(Actor actor, GameThing? target)
        {
            if (actor.CombatBehavior is not { Archetype: var arch })
                return new CombatDecision(CombatDecisionType.None, null);

            bool isCornered = target != null && actor.IsCornered(target);

            // 1. Decisión de Huida: Solo si NO está atrapado
            if (!isCornered && actor.HPRatio <= arch.FleeHPThreshold && Random.Shared.NextDouble() < arch.FleeChance)
                return new CombatDecision(CombatDecisionType.Flee, null);

            // 2. Decisión de Ataque: 
            // Si está acorralado, la probabilidad es 1.0 (100%). Si no, es la del arquetipo.
            if (target != null)
            {
                float chance = isCornered ? 1.0f : arch.AttackChance;
                float distance = actor.DistanceTo(target);

                if (Random.Shared.NextDouble() < chance)
                {
                    var intent = arch.SelectIntent(actor, actor.CombatBehavior.Intents, distance);
                    if (intent != null)
                        return new CombatDecision(arch.GetDecisionType(intent), intent);
                }
            }

            // 3. Fallback (Si no atacó y no huyó)
            // Ojo: Si es un cobarde acorralado y el SelectIntent falló, va a intentar Flee igual.
            return new CombatDecision(arch.IdleMoveType, null);
        }
    }
}