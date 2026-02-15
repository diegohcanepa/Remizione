using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// BrainChaseState
    /// </summary>
    public class BrainChaseState : BrainState
    {
        // Update
        public override void Update(GameTime gameTime)
        {
            // 1. OBTENER TARGET
            // Usamos el helper seguro de ProceduralActor que ya valida si es null o IsDead
            var target = Owner.GetTarget();
            if (target == null)
            {
                TransitionTo<BrainPatrolState>();
                return;
            }

            // 2. CHECK SENSORIAL (¿Lo sigo viendo?)
            // Si perdemos línea de visión, abortamos la persecución directa.
            // Volvemos al DecisionState, que seguramente nos mandará a BrainInvestigateState.
            if (!Owner.Sensor.CanSeeTarget)
            {
                TransitionTo<CombatDecisionState>();
                return;
            }

            // 3. MOVIMIENTO (El Cerebro ordena, el Cuerpo obedece)
            // Actualizamos la orden de movimiento hacia la posición actual del enemigo
            Owner.MoveTo(target.Position);

            // 4. CHECK DE RANGO DE ATAQUE
            float distSq = Vector2.DistanceSquared(Owner.Position, target.Position);
            float attackRangeSq = Owner.AttackRange * Owner.AttackRange;

            // Si estamos lo suficientemente cerca para atacar...
            if (distSq <= attackRangeSq)
            {
                // Importante: Frenar antes de cambiar de estado para no "deslizar" mientras ataca
                Owner.StopMoving();

                // Devolvemos el control al Router.
                // Él verá que estamos en rango y cambiará a BrainAttackState.
                TransitionTo<CombatDecisionState>();
            }
        }
    }
}