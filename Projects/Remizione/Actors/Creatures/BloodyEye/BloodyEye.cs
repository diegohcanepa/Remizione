namespace Remizione
{
    /// <summary>
    /// BloodyEye
    /// </summary>
    public sealed class BloodyEye : Actor
    {
        // Constructor
        public BloodyEye(GameSession session, string name)
            : base(session, name)
        {
            AllowHeadAnimation = false;
            AllowMoveTween = false;
            AllowMoveBalancingTween = false;
            CollisionDamage = DamageKind.Lightning;
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
