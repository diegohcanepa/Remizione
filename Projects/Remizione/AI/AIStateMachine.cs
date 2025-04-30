using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// AIStateMachine
    /// </summary>
    public sealed class AIStateMachine
    {
        private AIState currentState;
        private bool isTurnActive;
        private Dictionary<AIStateName, AIState> states = [];

        // Constructor
        public AIStateMachine(Actor actor)
        {
            this.Actor = actor;

            states[AIStateName.Charge] = new AIChargeState(this);
            states[AIStateName.CloseAttack] = new AICloseAttackState(this);
            states[AIStateName.Idle] = new AIIdleState(this);

            currentState = states[AIStateName.Idle];
            currentState.Enter();
        }

        // Actor
        public Actor Actor { get; }

        // ChangeState
        public void ChangeState(AIStateName newStateName)
        {
            currentState?.Exit();
            currentState = states[newStateName];
            currentState?.Enter();
        }

        // EndCombat
        public void EndCombat()
        {
            isTurnActive = false;
            //stateMachine.FireSignal(AIStateSignal.ExitCombat);
        }

        // EndTurn
        public void EndTurn()
        {
            isTurnActive = false;
            ChangeState(AIStateName.Idle);
            Actor.Session.CombatManager.EndCurrentTurn();
        }

        // ExecuteAction
        public void ExecuteAction(AIStateSignal signal)
        {
            // Attack
            if (signal == AIStateSignal.Attack)
            {
                if (Actor.AttackSkill is Item skill)
                {
                    if (skill.Range == 0)
                    {
                        ChangeState(AIStateName.Charge);
                    }
                    else
                    {
                        // Range attack
                    }
                }

                return;
            }

            // Close attack
            if (signal == AIStateSignal.CloseAttack)
            {
                ChangeState(AIStateName.CloseAttack);
                return;
            }

            // Este método puede ser llamado desde la AI o desde el input del jugador
            // por ejemplo: "ChargeAndAttack", "UseSkill", etc.
            //stateMachine.Reset();
            //stateMachine.FireSignal(signal);

            // Cuando la acción termine (desde el estado final, por ejemplo "AttackDone"), llamará:
            // EndTurn();
        }

        // Reset
        public void Reset()
        {
            currentState = states[AIStateName.Idle];
            currentState.Enter();
        }

        // StartTurn
        public void StartTurn()
        {
            if (Actor.IsDead)
            {
                Actor.Session.CombatManager.EndCurrentTurn();
                return;
            }

            isTurnActive = true;

            if (Actor.IsPlayer)
            {
                // El jugador espera input del usuario (p. ej., clic derecho para atacar)
                // El GameUI debería habilitar los botones para permitir acciones
            }
            else
            {
                // NPC: ejecutar comportamiento de combate usando la AI
                ExecuteAction(AIStateSignal.Attack);
            }
        }

        // Update
        public void Update(GameTime gameTime)
        {
            if (!isTurnActive)
                return;

            currentState?.Update(gameTime);
        }
    }
}