namespace Remizione
{
    /// <summary>
    /// ActorThrowItemState
    /// </summary>
    public sealed class ActorThrowItemState : ActorAnimatedState
    {
        // Constructor
        public ActorThrowItemState(Actor owner)
            : base(owner, AnimationNames.ThrowObject, false)
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

        // Exit
        public override void Exit()
        {
            if (Item != null && Owner.Session.ObjectPools.GetThrowable(Item.Name) is Throwable throwable)
            {
                throwable.Launch(Item);
                Item = null;
            }

            base.Exit();
        }

        // Item
        public Item? Item { get; set; }
    }
}