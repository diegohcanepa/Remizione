using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// AIStateMachine
    /// </summary>
    public sealed class AIStateMachine
    {
        private readonly Dictionary<CombatStateName, AIState> states = [];

        // Constructor
        public AIStateMachine(Actor actor)
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
        public AIState? CurrentState { get; private set; }

        // Destination
        public Vector2? Destination { get; private set; }

        // ExecuteAction
        public void ExecuteAction(AIStateSignal signal, Vector2? destination = null)
        {
            this.Destination = destination;

            /*
            // Attack
            if (signal == CombatStateSignal.Attack)
            {
                if (Actor.GetAttackItem() is Item attackItem)
                {
                    if (attackItem.Range == 0)
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
            */

            // Close attack
            if (signal == AIStateSignal.CloseAttack)
            {
                ChangeState(CombatStateName.CloseAttack);
                return;
            }

            // Decide
            if (signal == AIStateSignal.Decide)
            {
                ChangeState(CombatStateName.Decide);
                return;
            }

            // Move
            if (signal == AIStateSignal.Move)
            {
                ChangeState(CombatStateName.Move);
                return;
            }
        }

        // Update
        public void Update(GameTime gameTime)
        {
            CurrentState?.Update(gameTime);
        }
    }
}