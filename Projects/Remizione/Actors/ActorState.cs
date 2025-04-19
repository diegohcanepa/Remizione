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
        public ActorState(Actor owner, string name, ActorStateSettings settings)
            : base(owner, name)
        {
            this.settings = settings;
        }

        #region Protected members

        // GetAnimationName
        protected virtual string GetAnimationName() => Name;

        // AutoPlayAnimation
        protected virtual bool AutoPlayAnimation => true;

        #endregion

        // Enter
        public override void Enter()
        {
            if (AutoPlayAnimation)
                Owner.AnimationPlayer.Play(GetAnimationName(), settings.HasFlag(ActorStateSettings.LoopAnimation));
        }
    }
}
