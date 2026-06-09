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
        #region Private members

        // GetFallbackMovement
        private static CombatDecision GetFallbackMovement(CombatArchetype archetype, GameThing target)
        {
            if (archetype.AllowRandomMove)
                return new CombatDecision(CombatDecisionType.RandomMove, null, target);

            return new CombatDecision(CombatDecisionType.MoveNearby, null, target);
        }

        #endregion

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
                if (Random.Shared.NextDouble() < archetype.AttackChance || isCornered)
                {
                    // Selecciona el ataque basado en los pesos del arquetipo
                    var intent = archetype.SelectIntent(actor, actor.CombatBehavior.Intents, 0f);

                    if (intent != null)
                    {
                        // CASE 1: CUERPO A CUERPO (Espada, Mordisco, Tajo)
                        if (intent.UsageMode == ItemUsageMode.ProximityAction)
                        {
                            float distance = DistanceToTarget(actor, target);

                            // El AttackRange del arquetipo es el único tapón para el Melee
                            if (distance <= archetype.MeleeAttackRange)
                            {
                                return new CombatDecision(CombatDecisionType.Attack, intent, target);
                            }
                            else
                            {
                                // Está muy lejos para activar el ataque. 
                                // Camina un poco o se mueve random según el tipo de bicho.
                                return GetFallbackMovement(archetype, target);
                            }
                        }

                        // CASE 2: ATAQUE A DISTANCIA (Flechas, Magia, Escupitajo)
                        if (intent.UsageMode == ItemUsageMode.ProjectileAction)
                        {
                            // En rooms chicos el proyectil siempre viaja y pega. Dispara directo.
                            return new CombatDecision(CombatDecisionType.Attack, intent, target);
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