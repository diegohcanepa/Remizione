namespace Remizione.Creatures
{
    /// <summary>
    /// Sinner
    /// </summary>
    public sealed class Sinner : Actor
    {
        // Constructor
        public Sinner(GameSession session, string name)
            : base(session, name)
        {
            this.BodySize = ActorSize.Small;

            var attack = new AIAttackState(this);
            var chase = new AIChaseState(this);
            var idle = new AIIdleState(this);

            CombatAIStateMachine = new AIStateMachine(this, idle);
            CombatAIStateMachine.AddTransition<AIIdleState>(AIStateSignal.TargetOutOfRange, chase);
            CombatAIStateMachine.AddTransition<AIIdleState>(AIStateSignal.TargetInRange, attack);
            CombatAIStateMachine.AddTransition<AIChaseState>(AIStateSignal.ChaseComplete, idle);
            CombatAIStateMachine.AddTransition<AIAttackState>(AIStateSignal.AttackComplete, idle);
            CombatAIStateMachine.AddTransition<AIAttackState>(AIStateSignal.TargetLost, idle);
        }

        // OnPlayCombatTurn
        protected override void OnPlayCombatTurn()
        {
            CombatAIStateMachine?.Reset();
        }
    }
}
