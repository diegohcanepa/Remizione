using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// Paquete de datos liviano (Stack allocated, 0 GC) para la decisión de combate.
    /// </summary>
    public readonly record struct CombatDecision(CombatDecisionType Type, CombatIntent? Intent, GameThing? Target);

    /// <summary>
    /// Motor de decisiones de combate. 
    /// Solo procesa lógica usando los datos del actor y su arquetipo.
    /// </summary>
    public static class Brain
    {
        // DistanceToTarget
        private static float DistanceToTarget(GameThing source, GameThing target)
        {
            if (target.X < source.X)
                return Vector2.Distance(target.RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.RightBottom), source.RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.LeftBottom));
            else
                return Vector2.Distance(target.RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.LeftBottom), source.RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.RightBottom));
        }

        // Decide
        public static CombatDecision Decide(Actor actor, GameThing? target)
        {
            if (actor.CombatBehavior?.Archetype is not { } archetype)
                return new CombatDecision(CombatDecisionType.None, null, target);

            bool isCornered = target != null && actor.IsCornered(target);

            // 1. Decisión de Huida: Solo si NO está atrapado
            if (!isCornered && actor.HPRatio <= archetype.FleeHPThreshold && Random.Shared.NextDouble() < archetype.FleeChance)
                return new CombatDecision(CombatDecisionType.Flee, null, target);

            // 2. Decisión de Ataque: 
            // Si está acorralado, la probabilidad es 1.0 (100%). Si no, es la del arquetipo.
            if (target != null)
            {
                float chance = isCornered ? 1.0f : archetype.AttackChance;
                float distance = DistanceToTarget(actor, target);

                if (Random.Shared.NextDouble() < chance)
                {
                    var intent = archetype.SelectIntent(actor, actor.CombatBehavior.Intents, distance);
                    if (intent != null)
                        return new CombatDecision(archetype.GetDecisionType(intent), intent, target);
                }
            }

            // 3. Fallback (Si no atacó y no huyó)
            // Ojo: Si es un cobarde acorralado y el SelectIntent falló, va a intentar Flee igual.
            return new CombatDecision(archetype.IdleMoveType, null, target);
        }
    }
}