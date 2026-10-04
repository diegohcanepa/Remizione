using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// CombatIdleState
    /// </summary>
    public sealed class CombatIdleState : State<Actor>
    {
        private float wanderTimer;

        #region Private members

        // ResetWanderTimer
        private void ResetWanderTimer()
        {
            wanderTimer = ((float)Random.Shared.NextDouble() * 3f) + 2f;
        }

        // UpdateWander
        private void UpdateWander(GameTime gameTime)
        {
            if (Owner.IsMoving)
                return;

            wanderTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (wanderTimer <= 0f)
            {
                float leash = Owner.CombatBehavior?.LeashRadius ?? 0f;

                if (leash > 0f)
                {
                    Owner.MoveRandomlyAround(Owner.HomePosition, leash);
                }
                else
                {
                    Owner.MoveRandomly(80f);
                }

                ResetWanderTimer();
            }
        }

        #endregion

        // Enter
        public override void Enter()
        {
            ResetWanderTimer();
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (Owner.CombatBehavior?.Archetype is not { } arch)
                return;

            // Búsqueda dinámica de objetivos usando FactionMatrix
            Actor? target = TargetAcquisition.FindBestTarget(Owner, arch.AwarenessRange);

            if (target != null)
            {
                var stepState = Machine.FindOrCreateState<CombatStepState>();
                stepState.Target = target;
                Machine.ChangeState(stepState.GetType());
            }
            else
            {
                UpdateWander(gameTime);
            }
        }
    }
}