namespace ScaryCastle
{
    /// <summary>
    /// BodyCloseAttackState
    /// </summary>
    public sealed class BodyCloseAttackState : BodyAttackState
    {
        // Constructor
        public BodyCloseAttackState()
            : base()
        {
        }

        // CanInflictDamage
        protected override bool CanInflictDamage(GameThing target)
        {
            return target.CanBeHit && Owner.AnimationPlayer.Frame?.IsEvent == true;
        }
    }
}