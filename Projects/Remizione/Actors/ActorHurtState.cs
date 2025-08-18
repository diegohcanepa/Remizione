namespace Remizione
{
    /// <summary>
    /// ActorHurtState
    /// </summary>
    public sealed class ActorHurtState : ActorAnimatedState
    {
        // Constructor
        public ActorHurtState(Actor owner)
            : base(owner, ActorStateNames.Hurt, false)
        {
        }

        // Enter
        public override void Enter()
        {
            if (Owner.DamageStyle == DamageStyle.Animation)
                base.Enter();
        }

        // CheckTransitions
        public override string? CheckTransitions()
        {
            if (Owner.DamageStyle == DamageStyle.Animation && !Owner.AnimationPlayer.IsPlaying)
                return ActorStateNames.Stand;

            else if (Owner.DamageStyle == DamageStyle.Blink && !Owner.IsBlinking)
                return ActorStateNames.Stand;

            else
                return base.CheckTransitions();
        }
    }
}
