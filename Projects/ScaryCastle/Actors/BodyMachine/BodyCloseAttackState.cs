namespace ScaryCastle
{
    /// <summary>
    /// BodyCloseAttackState
    /// </summary>
    public sealed class BodyCloseAttackState : BodyAttackState
    {
        // Constructor
        public BodyCloseAttackState()
            : base(AnimationNames.CloseAttack)
        {
        }

        // CanInflictDamage
        protected override bool CanInflictDamage(GameThing target)
        {
            if (Owner.AnimationPlayer.Frame?.IsEvent == true)
            {
                if (Owner.AnimationPlayer.GetFrameSubArea().Intersects(target.RuntimeHotspot.BoundingRectangleF))
                    return true;
            }

            return false;
        }
    }
}