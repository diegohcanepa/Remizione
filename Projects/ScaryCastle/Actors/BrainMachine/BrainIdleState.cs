using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// Estado inicial y de reposo absoluto. 
    /// Desde aquí el cerebro decide su rutina (Patrullar, quedarse quieto, etc.)
    /// </summary>
    public class BrainIdleState : BrainState
    {
        public override void Enter()
        {
            // Regla de oro: al entrar en reposo, el cuerpo obedece y frena.
            Owner.StopMoving();

            // Si tenías la ira cargada (ej: el jugador se fue lejos y volviste a reposo)
            // acá es un buen lugar para asegurarte de que la ira baje a 0.
            Owner.CurrentRage = 0;
            Owner.IsAngry = false;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            // 1. REACCIÓN INMEDIATA (Prioridad Alta)
            // ¿Nací y tengo al jugador en la cara? o ¿Estaba descansando y escuché un ruido?
            if (Owner.Sensor.IsAlerted)
            {
                TransitionTo<CombatDecisionState>();
                return;
            }

            // 2. RUTINA DE REPOSO (Data-Driven)
            // Aquí es donde el diseño se vuelve robusto. 
            // Evaluamos qué tipo de monstruo es para saber qué hace cuando está aburrido.

            // TODO (Diseño Futuro): Idealmente leerías esto de Owner.Definition
            // ej: if (Owner.Definition.IdleRoutine == Routine.Patrol)

            // Por ahora, como tu juego asume que todos patrullan, hacemos 
            // la transición directa para mantener la funcionalidad actual:
            TransitionTo<BrainPatrolState>();

            // El día que hagas una "Gárgola Estática", simplemente agregarás un if 
            // que evite esta transición, y el actor se quedará clavado en este Update vacío.
        }
    }
}