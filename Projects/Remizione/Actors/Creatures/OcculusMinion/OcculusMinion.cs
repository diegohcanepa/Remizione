namespace Remizione
{
    /// <summary>
    /// OcculusMinion
    /// </summary>
    public sealed class OcculusMinion : Actor
    {
        // Constructor
        public OcculusMinion(GameSession session, string name)
            : base(session, name)
        {
            AllowHeadAnimation = false;
            AllowMoveTween = false;
            AllowMoveBalancingTween = false;
            CollisionDamage = DamageKind.Lightning;
            PreventBlink = true;
            PreventKnockback = true;
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            
            AIStateMachine.RegisterState(new BloodyEyePatrolState(AIStateMachine));
            AIStateMachine.ChangeState(AIStateName.Patrol);
        }
    }
}
