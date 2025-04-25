namespace Remizione
{
    /// <summary>
    /// ActorActState
    /// </summary>
    public sealed class ActorActState : ActorState
    {
        // Constructor
        public ActorActState(Actor owner)
            : base(owner, ActorStateNames.Act, ActorStateSettings.None, false)
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
