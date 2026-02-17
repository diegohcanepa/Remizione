using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// BrainInvestigateState
    /// </summary>
    public class BrainInvestigateState : BrainState
    {
        #region Constants

        private const float INVESTIGATION_TIME = 2000; // Se queda 2 segundos mirando "confundido"
        private const float ARRIVAL_THRESHOLD = 30;    // 30 pixeles de margen para considerar que llegó

        #endregion

        #region Private fields

        private float confusionTimer;
        private bool hasArrived;

        #endregion

        // Enter
        public override void Enter()
        {
            hasArrived = false;
            confusionTimer = INVESTIGATION_TIME;
        }

        // TargetLocation
        public Vector2 TargetLocation { get; set; }

        // Update
        public override void Update(GameTime gameTime)
        {
            // -----------------------------------------------------------
            // 1. PRIORIDAD MÁXIMA: Si recuperamos contacto visual
            // -----------------------------------------------------------
            if (Owner.Sensor.CanSeeTarget)
            {
                TransitionTo<CombatDecisionState>();
                return;
            }

            // -----------------------------------------------------------
            // 2. FASE DE MOVIMIENTO (Ir al punto)
            // -----------------------------------------------------------
            if (!hasArrived)
            {
                Owner.MoveTo(TargetLocation);

                // Chequear distancia (al cuadrado para optimizar)
                float distSq = Vector2.DistanceSquared(Owner.Position, TargetLocation);
                if (distSq < ARRIVAL_THRESHOLD * ARRIVAL_THRESHOLD)
                {
                    hasArrived = true;
                    Owner.StopMoving();
                }
            }

            // -----------------------------------------------------------
            // 3. FASE DE CONFUSIÓN (Mirar alrededor)
            // -----------------------------------------------------------
            else
            {
                confusionTimer -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                // (Opcional) Aquí podrías hacer que el actor gire de izquierda a derecha
                // if (_confusionTimer % 500 < 250) Context.FacingDirection = 1 else -1;

                if (confusionTimer <= 0)
                {
                    // Se acabó el tiempo y no encontramos nada.

                    // CRÍTICO: Borrar la memoria del sensor.
                    // Si no hacemos esto, al volver a Patrol, el sensor seguirá diciendo 
                    // "Recuerdo algo" y volveremos a entrar en bucle a este estado.
                    Owner.Sensor.Forget();

                    TransitionTo<BrainPatrolState>();
                }
            }
        }
    }
}