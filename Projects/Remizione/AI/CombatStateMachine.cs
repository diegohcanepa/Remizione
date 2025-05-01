using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// CombatStateMachine
    /// </summary>
    public sealed class CombatStateMachine
    {
        private Dictionary<CombatStateName, CombatState> states = [];

        // Constructor
        public CombatStateMachine(Actor actor)
        {
            this.Actor = actor;

            states[CombatStateName.Charge] = new CombatChargeState(this);
            states[CombatStateName.CloseAttack] = new CombatCloseAttackState(this);
            states[CombatStateName.Decide] = new CombatDecideState(this);
            states[CombatStateName.Move] = new CombatMoveState(this);
        }

        // Actor
        public Actor Actor { get; }

        // ChangeState
        public void ChangeState(CombatStateName newStateName)
        {
            CurrentState?.Exit();
            CurrentState = states[newStateName];
            CurrentState?.Enter();
        }

        // CurrentState
        public CombatState? CurrentState { get; private set; }

        // Destination
        public Vector2? Destination { get; private set; }

        // ExecuteAction
        public void ExecuteAction(CombatStateSignal signal, Vector2? destination = null)
        {
            this.Destination = destination;

            // Attack
            if (signal == CombatStateSignal.Attack)
            {
                if (Actor.AttackSkill is Item skill)
                {
                    if (skill.Range == 0)
                    {
                        ChangeState(CombatStateName.Charge);
                    }
                    else
                    {
                        // Range attack
                    }
                }

                return;
            }

            // Close attack
            if (signal == CombatStateSignal.CloseAttack)
            {
                ChangeState(CombatStateName.CloseAttack);
                return;
            }

            // Decide
            if (signal == CombatStateSignal.Decide)
            {
                ChangeState(CombatStateName.Decide);
                return;
            }

            // EndTurn
            if (signal == CombatStateSignal.EndTurn)
            {
                Actor.EndTurn();
                return;
            }

            // Move
            if (signal == CombatStateSignal.Move)
            {
                ChangeState(CombatStateName.Move);
                return;
            }
        }

        // Update
        public void Update(GameTime gameTime)
        {
            if (Actor.Session.CombatManager.CurrentActor == Actor && Actor.TurnState != CombatTurnState.WaitingInput)
                CurrentState?.Update(gameTime);
        }
    }
}