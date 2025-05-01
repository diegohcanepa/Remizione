namespace Remizione
{
    /// <summary>
    /// ActorAnimatedState
    /// </summary>
    public class ActorAnimatedState : ActorState
    {
        private readonly bool autoPlayAnimation;
        private readonly bool loopAnimation;

        // Constructor
        public ActorAnimatedState(Actor owner, string name, bool loopAnimation, bool autoPlayAnimation = true)
            : base(owner, name)
        {
            this.loopAnimation = loopAnimation;
            this.autoPlayAnimation = autoPlayAnimation;
        }

        #region Protected members

        // GetAnimationName
        protected virtual string GetAnimationName() => Name;

        // PlayAnimation
        protected void PlayAnimation()
        {
            Owner.AnimationPlayer.Play(GetAnimationName(), loopAnimation);
        }

        #endregion

        // Enter
        public override void Enter()
        {
            if (autoPlayAnimation)
                PlayAnimation();
        }
    }
}
