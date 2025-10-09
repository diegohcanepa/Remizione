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

            _ = new ZabulChargeState(AIStateMachine);
            _ = new ZabulDecideState(AIStateMachine);
        }

        // OnFindEnemy
        protected override GameThing? OnFindEnemy() => Session.Player;

        // OnStart
        protected override void OnStart()
        {
            AIStateMachine.ChangeState(AIStateName.Decide);
        }
    }
}
