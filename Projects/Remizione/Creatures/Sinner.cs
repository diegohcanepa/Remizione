using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            var charge = new AIChargeState(this);
            var idle = new AIIdleState(this);

            AIStateMachine = new AIStateMachine(this, idle);
            AIStateMachine.AddTransition<AIIdleState>(AIStateSignal.SawPlayer, charge);
            AIStateMachine.AddTransition<AIChargeState>(AIStateSignal.ChargeComplete, idle);
        }

        // OnHurt
        protected override void OnHurt()
        {
            base.OnHurt();
            Hostile = true;
        }
    }
}
