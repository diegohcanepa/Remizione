using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    public sealed class CombatDecisionState : BrainState
    {
        private const int MIN_REACTION_TIME = 400;
        private const int MAX_REACTION_TIME = 1200;
        private int _thinkingTimer;

        public override void Enter()
        {
            Owner.StopMoving();
            _thinkingTimer = Random.Shared.Next(MIN_REACTION_TIME, MAX_REACTION_TIME + 1);
        }

        public override void Update(GameTime gameTime)
        {
            if (Owner.Target == null)
            {
                Owner.IsAngry = false;
                Owner.CurrentRage = 0;
                TransitionTo<BrainIdleState>();
                return;
            }

            if (!Owner.IsAngry)
            {
                UpdateAnger(gameTime, Owner.Target);
                if (!Owner.IsAngry) return;
            }

            if (_thinkingTimer > 0)
            {
                _thinkingTimer -= gameTime.ElapsedGameTime.Milliseconds;
                if (Owner.Sensor.CanSeeTarget) Owner.FaceTo(Owner.Target);
                return;
            }

            if (!Owner.Sensor.CanSeeTarget && Owner.Sensor.LastKnownTargetPos.HasValue)
            {
                var investigateState = Machine.FindOrCreateState<BrainInvestigateState>();
                investigateState.TargetLocation = Owner.Sensor.LastKnownTargetPos.Value;
                Machine.ChangeState<BrainInvestigateState>();
                return;
            }

            if (Owner.Sensor.CanSeeTarget)
                DecideCombatManeuver(Owner.Target);
        }

        #region Private members

        // UpdateAnger
        private void UpdateAnger(GameTime gameTime, Actor target)
        {
            if (Owner.Brain.RageChargeTime == -1) return;

            float dist = Vector2.Distance(Owner.Position, target.Position);

            if (dist <= Owner.Brain.DetectionRadius)
                Owner.CurrentRage += gameTime.ElapsedGameTime.Milliseconds;
            else
                Owner.CurrentRage = 0;

            if (Owner.CurrentRage >= Owner.Brain.RageChargeTime || Owner.Brain.RageChargeTime == 0)
            {
                Owner.IsAngry = true;
                // Pausa dramática aleatoria en MS
                _thinkingTimer = Random.Shared.Next(MIN_REACTION_TIME, MAX_REACTION_TIME);
            }
        }

        private void DecideCombatManeuver(Actor target)
        {
            bool inRange = Owner.IsInAttackRange(target);
            bool canAttack = Owner.AttackTimer <= 0;

            switch (Owner.CombatBehavior.Archetype)
            {
                case CombatBehaviorArchetype.Berserk:
                case CombatBehaviorArchetype.Tactical:
                default:
                    if (inRange && canAttack)
                        TransitionTo<BrainAttackState>();
                    else if (inRange && !canAttack)
                        Owner.FaceTo(target);
                    else
                        TransitionTo<BrainChaseState>();
                    break;
            }
        }

        #endregion
    }
}