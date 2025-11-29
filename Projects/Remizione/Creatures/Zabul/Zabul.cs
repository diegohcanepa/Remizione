using System;

namespace Remizione
{
    /// <summary>
    /// Zabul
    /// </summary>
    public sealed class Zabul : Enemy
    {
        // Constructor
        public Zabul(GameSession session, string name)
            : base(session, name)
        {
            BodySize = ActorSize.Small;
            AnimationSettings.SupressAll();
            ContactDamagePolygon = TestPolygon.Hotspot;
            Guts = 0;
            FloatingForce = 1;
            ShadowSpotSize = 5;

            PerceptionSensor.ViewDistance = 200;
            PerceptionSensor.ViewAngle = 360;

            _ = new ChargeState(AIStateMachine);
            _ = new ZabulDecideState(AIStateMachine);
        }

        // OnStart
        protected override void OnStart()
        {
            PerceptionSensor.RefreshRate = Random.Shared.Next(PerceptionSensor.DefaultRefreshRate / 2, PerceptionSensor.DefaultRefreshRate + 1);
            AIStateMachine.ChangeState(AIStateName.Decide);
        }
    }
}
