using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// BrainPatrolState
    /// </summary>
    public class BrainPatrolState : BrainState
    {
        private float _timer;
        private Vector2? _destination;
        private Vector2 _homePosition; // Para que no se aleje infinitamente del spawn

        // Configuración (Podría venir del ActorDefinition)
        private const float PATROL_RADIUS = 50;
        private const float MIN_WAIT_TIME = 1000f; // 1 segundo
        private const float MAX_WAIT_TIME = 4000f; // 4 segundos

        // Enter
        public override void Enter()
        {
            if (_homePosition == Vector2.Zero)
                _homePosition = Owner.Position;

            StartWaiting();
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            // ---------------------------------------------------------
            // 1. EL OJO QUE TODO LO VE (Salida de Emergencia)
            // ---------------------------------------------------------
            if (Owner.Sensor.IsAlerted)
            {
                // ¡Vio algo! Cortamos la patrulla y vamos al cerebro
                Owner.StopMoving();
                TransitionTo<CombatDecisionState>();
                return;
            }

            // ---------------------------------------------------------
            // 2. COMPORTAMIENTO DE PATRULLA
            // ---------------------------------------------------------

            // CASO A: Estamos esperando (Idle)
            if (!_destination.HasValue)
            {
                _timer -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_timer <= 0)
                    PickNewDestination();
            }
            // CASO B: Estamos caminando hacia un punto
            else
            {
                // Chequear si llegamos
                float distSq = Vector2.DistanceSquared(Owner.Position, _destination.Value);

                // Margen de 5px para considerar que llegó
                if (distSq < 25f)
                {
                    StartWaiting();
                }
                else
                {
                    // Fail-safe: Si se queda trabado contra una pared mucho tiempo,
                    // deberíamos abortar y elegir otro punto (puedes agregar un timer aquí).
                    Owner.MoveTo(_destination.Value);
                }
            }
        }

        // --- Helpers ---

        // StartWaiting
        private void StartWaiting()
        {
            _destination = null;
            Owner.StopMoving();

            // Tiempo aleatorio para que los enemigos no se muevan todos en sincronía
            _timer = MIN_WAIT_TIME + ((float)Random.Shared.NextDouble() * (MAX_WAIT_TIME - MIN_WAIT_TIME));
        }

        private void PickNewDestination()
        {
            // Elegir un punto aleatorio dentro del radio alrededor del Home
            // (No de la posición actual, para evitar "drift" infinito)

            float angle = (float)Random.Shared.NextDouble() * MathHelper.TwoPi;
            float distance = (float)Random.Shared.NextDouble() * PATROL_RADIUS;

            Vector2 offset = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * distance;
            Vector2 potentialDest = _homePosition + offset;

            // TODO: VALIDACIÓN DE MAPA CRÍTICA
            // Aquí deberías llamar a tu sistema de WalkArea para ver si el punto es caminable.
            // if (Context.Room.IsWalkable(potentialDest)) ...

            _destination = potentialDest;
            Owner.MoveTo(_destination.Value);
        }
    }
}