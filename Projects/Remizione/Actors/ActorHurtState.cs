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

        // CheckTransitions
        public override string? CheckTransitions()
        {
            if (!Owner.AnimationPlayer.IsPlaying)
                return ActorStateNames.Stand;
            else
                return base.CheckTransitions();
        }
    }
}
