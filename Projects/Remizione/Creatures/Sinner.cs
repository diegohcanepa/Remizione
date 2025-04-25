using Engendro;

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
            this.Affinity = Affinity.Evil;

            var attack = new AIAttackState(this);
            var chase = new AIChaseState(this);
            var idle = new AIIdleState(this);

            /*

            AIStateMachine = new AIStateMachine(this, idle);

            AIStateMachine.AddTransition<AIIdleState>(AIStateSignal.SawTarget, chase);
            AIStateMachine.AddTransition<AIChaseState>(AIStateSignal.TargetInRange, attack);
            AIStateMachine.AddTransition<AIAttackState>(AIStateSignal.TargetOutOfRange, chase);
            AIStateMachine.AddTransition<AIAttackState>(AIStateSignal.TargetLost, idle);

            */
        }

        // OnHurt
        protected override void OnHurt(GameThing attacker)
        {
            base.OnHurt(attacker);

            if (attacker is Actor actor)
                Target = actor;
        }
    }
}
