namespace Remizione
{
    /// <summary>
    /// ActorDeathState
    /// </summary>
    public sealed class ActorDeathState : ActorAnimatedState
    {
        // Constructor
        public ActorDeathState(Actor owner)
            : base(owner, ActorStateNames.Death, false)
        {
        }

        // CheckTransitions
        public override string? CheckTransitions()
        {
            if (Owner.AnimationPlayer.IsPlaying == false)
            {
                if (Owner.IsPlayer)
                    Owner.Session.AwaitRoutine(RoutineNames.GameOver);
                else
                    return ActorStateNames.Stand;
            }

            return base.CheckTransitions();
        }
    }
}
