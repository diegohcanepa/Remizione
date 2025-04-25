using Engendro;

namespace Remizione
{
    /// <summary>
    /// ActorState
    /// </summary>
    public class ActorState : State<Actor>
    {
        private readonly ActorStateSettings settings;

        // Constructor
        public ActorState(Actor owner, string name, ActorStateSettings settings, bool autoPlayAnimation = true)
            : base(owner, name)
        {
            this.settings = settings;
            this.AutoPlayAnimation = autoPlayAnimation;
        }

        #region Protected members

        // GetAnimationName
        protected virtual string GetAnimationName() => Name;

        // AutoPlayAnimation
        protected bool AutoPlayAnimation { get; }

        // PlayAnimation
        protected void PlayAnimation()
        {
            Owner.AnimationPlayer.Play(GetAnimationName(), settings.HasFlag(ActorStateSettings.LoopAnimation));
        }

        #endregion

        // Enter
        public override void Enter()
        {
            if (AutoPlayAnimation)
                PlayAnimation();
        }
    }
}
