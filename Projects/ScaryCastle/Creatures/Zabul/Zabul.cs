namespace ScaryCastle
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
            Guts = 0;
            FloatingForce = 1;
            ShadowSpotSize = 5;

            _ = new ZabulDecideState(AIStateMachine);
        }

        // OnStart
        protected override void OnStart()
        {
            //AIStateMachine.ChangeState(AIStateName.Decide);
        }
    }
}
