namespace Remizione
{
    /// <summary>
    /// ActorAnimateState
    /// </summary>
    public sealed class ActorAnimateState : ActorState
    {
        // Constructor
        public ActorAnimateState(Actor owner)
            : base(owner, ActorStateNames.Animate)
        {
        }

        // CheckTransitions
        public override string? CheckTransitions()
        {
            if (!Preserve && !Owner.AnimationPlayer.IsPlaying)
                return ActorStateNames.Stand;
            else
                return base.CheckTransitions();
        }

        // Preserve
        public bool Preserve { get; set; }
    }
}
