using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// CombatStateMachine
    /// </summary>
    public sealed class CombatStateMachine
    {
        private CombatState currentState;
        private bool isTurnActive;
        private Dictionary<CombatStateName, CombatState> states = [];

        // Constructor
        public CombatStateMachine(Actor actor)
        {
            this.Actor = actor;

            states[CombatStateName.Charge] = new CombatChargeState(this);
            states[CombatStateName.CloseAttack] = new CombatCloseAttackState(this);
            states[CombatStateName.Idle] = new CombatIdleState(this);

            currentState = states[CombatStateName.Idle];
            currentState.Enter();
        }

        // Actor
        public Actor Actor { get; }

        // ChangeState
        public void ChangeState(CombatStateName newStateName)
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
            ChangeState(CombatStateName.Idle);
            Actor.Session.CombatManager.EndCurrentTurn();
        }

        // ExecuteAction
        public void ExecuteAction(CombatStateSignal signal)
        {
            // Idle
            if (signal == CombatStateSignal.Idle)
            {
                ChangeState(CombatStateName.Idle);
                return;
            }

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
        }

        // Reset
        public void Reset()
        {
            currentState = states[CombatStateName.Idle];
            currentState.Enter();
        }

        // StartTurn
        public void StartTurn()
        {
            if (isTurnActive)
                return;

            if (Actor.IsDead)
            {
                Actor.Session.CombatManager.EndCurrentTurn();
                return;
            }

            Actor.Session.CombatManager.Add(Actor);
            isTurnActive = true;

            if (Actor.IsPlayer)
            {
                // El jugador espera input del usuario (p. ej., clic derecho para atacar)
                // El GameUI debería habilitar los botones para permitir acciones
            }
            else
            {
                // NPC: ejecutar comportamiento de combate usando la AI
                ExecuteAction(CombatStateSignal.Idle);
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