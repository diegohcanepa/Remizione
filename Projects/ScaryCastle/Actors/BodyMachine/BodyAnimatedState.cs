namespace ScaryCastle
{
    /// <summary>
    /// BodyAnimatedState
    /// </summary>
    public class BodyAnimatedState : BodyState
    {
        private readonly string defaultAnimationName;
        private readonly bool autoPlayAnimation;
        private readonly bool loopAnimation;

        // Constructor
        public BodyAnimatedState(string animationName, bool loopAnimation, bool autoPlayAnimation = true)
            : base()
        {
            this.defaultAnimationName = animationName;
            this.loopAnimation = loopAnimation;
            this.autoPlayAnimation = autoPlayAnimation;
        }

        #region Protected members

        // GetAnimationName
        protected virtual string GetAnimationName()
        {
            return defaultAnimationName;
        }

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
