using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// CombatDecisionState
    /// </summary>
    public sealed class CombatDecisionState : BrainState
    {
        #region Private members

        // DecideCombatManeuver
        private void DecideCombatManeuver()
        {
            var target = Owner.GetTarget();
            if (target == null)
                return;

            // Usamos distancia al cuadrado para evitar Math.Sqrt (Optimización)
            float distSq = Vector2.DistanceSquared(Owner.Position, target.Position);
            float attackRangeSq = Owner.AttackRange * Owner.AttackRange;

            // Decisión basada en el Arquetipo de Combate (Data-Driven)
            switch (Owner.CombatBehavior.Archetype)
            {
                // BERSERK: Agresividad suicida
                case CombatBehaviorArchetype.Berserk:
                    if (distSq <= attackRangeSq)
                        TransitionTo<BrainAttackState>();
                    else
                        TransitionTo<BrainChaseState>(); // Corre directo hacia el jugador
                    break;

                // TACTICAL: Comportamiento estándar (puedes refinado luego)
                case CombatBehaviorArchetype.Tactical:
                default:
                    // Aquí podrías agregar lógica de "Strafe" o esperar
                    if (distSq <= attackRangeSq)
                        TransitionTo<BrainAttackState>();
                    else
                        TransitionTo<BrainChaseState>();
                    break;

                    // COWARD (Ejemplo por si lo agregas al Enum luego)
                    /*
                    case CombatBehaviorArchetype.Coward:
                        float panicDistSq = 100 * 100;
                        if (distSq < panicDistSq)
                            TransitionTo<FleeState>(); // Huir
                        else if (distSq <= attackRangeSq)
                            TransitionTo<AttackState>(); // Atacar de lejos
                        else
                             TransitionTo<WaitState>(); // No acercarse
                        break;
                    */
            }
        }

        #endregion

        // Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // 1. FAILSAFE: Si el jugador no existe o murió, modo pasivo.
            if (Owner.GetTarget() == null)
            {
                TransitionTo<BrainPatrolState>();
                return;
            }

            // 2. PERCEPCIÓN: ¿Qué está sintiendo el enemigo?

            // CASO A: Calma total (Ni ve, ni escucha, ni recuerda)
            if (!Owner.Sensor.IsAlerted)
            {
                TransitionTo<BrainPatrolState>();
                return;
            }

            // CASO B: Búsqueda (No lo ve ahora, pero recuerda dónde estaba)
            if (!Owner.Sensor.CanSeeTarget && Owner.Sensor.LastKnownTargetPos.HasValue)
            {
                // Recuperamos la instancia del estado para pasarle el dato
                var investigateState = Machine.FindOrCreateState<BrainInvestigateState>();

                // Le pasamos la posición a investigar
                investigateState.TargetLocation = Owner.Sensor.LastKnownTargetPos.Value;

                // Ejecutamos el cambio
                Machine.ChangeState<BrainInvestigateState>();
                return;
            }

            // CASO C: Combate (Contacto Visual Directo)
            if (Owner.Sensor.CanSeeTarget)
                DecideCombatManeuver();
        }
    }
}