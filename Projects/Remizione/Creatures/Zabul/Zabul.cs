namespace Remizione
{
    /// <summary>
    /// Zabul
    /// </summary>
    public sealed class Zabul : Creature
    {
        // Constructor
        public Zabul(GameSession session, string name)
            : base(session, name)
        {
            BodySize = ActorSize.Small;
            AnimationSettings.SupressAll();
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
            AIStateMachine.ChangeState(AIStateName.Decide);
        }
    }
}
