using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// Paquete de datos liviano (Stack allocated, 0 GC) para la decisión de combate.
    /// </summary>
    public sealed class CombatDecision(CombatDecisionType Type, CombatIntent? Intent, GameThing? Target)
    {
        public CombatDecisionType Type { get; } = Type;
        public CombatIntent? Intent { get; } = Intent;
        public GameThing? Target { get; } = Target;
    }

    /// <summary>
    /// Brain
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
        public static CombatDecision? Decide(Actor actor, GameThing? target)
        {
            if (actor.CombatBehavior?.Archetype is not { } archetype)
                return null;

            bool isCornered = target != null && actor.IsCornered(target);

            // 1. Decisión de Huida: Filtro de pánico si está reventado y hay espacio físico para escapar
            if (!isCornered && actor.HPRatio <= archetype.FleeHPThreshold && Random.Shared.NextDouble() < archetype.FleeChance)
                return new CombatDecision(CombatDecisionType.Flee, null, target);

            // 2. Procesamiento de la Intención de Ataque / Persecución
            if (target != null)
            {
                float distance = DistanceToTarget(actor, target);

                // Tiramos el dado de AttackChance para ver si el bicho quiere ir a buscarte voluntariamente en este pulso
                if (Random.Shared.NextDouble() < archetype.AttackChance || isCornered)
                {
                    // SelectIntent selecciona el ataque SIN descartar por distancia 
                    var intent = archetype.SelectIntent(actor, actor.CombatBehavior.Intents, distance);

                    if (intent != null)
                    {
                        if (intent.Contact)
                        {
                            // Cuerpo a cuerpo: Evaluamos si la brecha física (distancia - rango) entra en su MoveRange
                            if ((distance - intent.Range) <= archetype.MoveRange)
                            {
                                // Le da la nafta para llegar: se mueve y te emboca en el mismo pulso
                                return new CombatDecision(CombatDecisionType.ApproachAndAttack, intent, target);
                            }
                            else
                            {
                                // Quiere morderte pero está lejos: usa el pulso para acortar distancia hacia Edmundo
                                return new CombatDecision(CombatDecisionType.Approach, intent, target);
                            }
                        }
                        else
                        {
                            // Ataque a distancia: Si está en rango ejecuta, si no, se acerca
                            if (distance <= intent.Range)
                                return new CombatDecision(CombatDecisionType.ApproachAndAttack, intent, target);
                            else
                                return new CombatDecision(CombatDecisionType.Approach, intent, target);
                        }
                    }
                }
            }

            // 3. Fallback Determinista (Si falló la chance de ataque o no hay intents válidos)
            // Si es un obstáculo ambiental (Rata), se mueve al azar de forma caótica.
            if (archetype.AllowRandomMove)
                return new CombatDecision(CombatDecisionType.RandomMove, null, target);

            // Si es un enemigo inteligente, usa el pulso para ganar terreno y achicarte el pasillo.
            return new CombatDecision(CombatDecisionType.Approach, null, target);
        }
    }
}