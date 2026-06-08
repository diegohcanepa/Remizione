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

                if (Random.Shared.NextDouble() < archetype.AttackChance || isCornered)
                {
                    // Selecciona el ataque según pesos.
                    var intent = archetype.SelectIntent(actor, actor.CombatBehavior.Intents, distance);

                    if (intent != null)
                    {
                        // UNIFICACIÓN: ¿Es un ataque que requiere proximidad física? (Melee, Espada, Mordisco, Contact)
                        if (intent.UsageMode == ItemUsageMode.ProximityAction)
                        {
                            // El alcance total de este turno es lo que camina + el largo de su arma/cuerpo
                            float totalReach = archetype.MoveRange + intent.Range;

                            if (distance <= totalReach)
                            {
                                // Le da la nafta: se mueve los píxeles necesarios, se te pega y te ejecuta el tajo/mordisco
                                return new CombatDecision(CombatDecisionType.Attack, intent, target);
                            }
                            else
                            {
                                // Está demasiado lejos para llegar a pegarte en este turno: 
                                // simplemente gasta su MoveRange para acortar distancia y quedar mejor posicionado
                                return new CombatDecision(CombatDecisionType.MoveNearby, intent, target);
                            }
                        }
                        else
                        {
                            // ATAQUE A DISTANCIA PURO (Proyectiles, Magias)
                            // No gasta movimiento para atacar. Si estás en su rango de fuego, dispara.
                            if (distance <= intent.Range)
                            {
                                return new CombatDecision(CombatDecisionType.Attack, intent, target);
                            }
                            else
                            {
                                // Si el arquero está lejos, avanza para intentar ponerte en rango el turno que viene
                                return new CombatDecision(CombatDecisionType.MoveNearby, intent, target);
                            }
                        }
                    }
                }
            }

            // 3. Fallback Determinista (Si falló la chance de ataque o no hay intents válidos)
            // Si es un obstáculo ambiental (Rata), se mueve al azar de forma caótica.
            if (archetype.AllowRandomMove)
                return new CombatDecision(CombatDecisionType.RandomMove, null, target);

            // Si es un enemigo inteligente, usa el pulso para ganar terreno y achicarte el pasillo.
            return new CombatDecision(CombatDecisionType.MoveNearby, null, target);
        }
    }
}