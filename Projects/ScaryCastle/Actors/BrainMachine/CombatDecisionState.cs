using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// CombatDecisionState
    /// </summary>
    public sealed class CombatDecisionState : BrainState
    {
        private const float MIN_REACTION_TIME = 400;
        private const float MAX_REACTION_TIME = 1200;
        private float _thinkingTimer;

        #region Private members

        // DecideCombatManeuver
        private void DecideCombatManeuver()
        {
            if (Owner.GetTarget() is not { } target)
                return;

            bool inRange = Owner.IsInAttackRange(target.Position);

            // Decisión basada en el Arquetipo de Combate (Data-Driven)
            switch (Owner.CombatBehavior.Archetype)
            {
                // BERSERK: Agresividad suicida
                case CombatBehaviorArchetype.Berserk:
                    if (inRange)
                        TransitionTo<BrainAttackState>();
                    else
                        TransitionTo<BrainChaseState>();
                    break;

                // COWARD
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

                case CombatBehaviorArchetype.Sniper:
                    break;

                case CombatBehaviorArchetype.Swarmer:
                    break;

                // TACTICAL: Comportamiento estándar (puedes refinado luego)
                case CombatBehaviorArchetype.Tactical:
                default:
                    // Aquí podrías agregar lógica de "Strafe" o esperar
                    if (inRange)
                        TransitionTo<BrainAttackState>();
                    else
                        TransitionTo<BrainChaseState>();
                    break;
            }
        }

        #endregion

        // Enter
        public override void Enter()
        {
            // 1. FRENAR TODO
            // Lo primero que hace al entrar a decidir es detenerse.
            // Esto elimina el "patinado" y pone al Body en Idle.
            Owner.StopMoving();

            // 2. CALCULAR TIEMPO DE PENSAMIENTO
            // Un poco de random para que no parezcan robots sincronizados.
            _thinkingTimer = MIN_REACTION_TIME + ((float)Random.Shared.NextDouble() * (MAX_REACTION_TIME - MIN_REACTION_TIME));
        }

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

            // --- FASE DE PENSAMIENTO (La Pausa Dramática) ---

            _thinkingTimer -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Mientras esté pensando, no hacemos NADA. 
            // El enemigo se queda quieto mirándote.
            if (_thinkingTimer > 0)
            {
                // Opcional: Hacer que mire al jugador mientras piensa
                if (Owner.Sensor.CanSeeTarget && Owner.GetTarget() is { } t)
                    Owner.FaceTo(t);

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