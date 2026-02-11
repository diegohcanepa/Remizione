using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// ActorActionState
    /// </summary>
    public abstract class ActorActionState : ActorAnimatedState
    {
        private bool actionDone;

        // Constructor
        protected ActorActionState(Actor owner, string animationName)
            : base(owner, animationName, false)
        {
        }

        #region Protected members

        // OnExecuteAction
        protected virtual void OnExecuteAction()
        {
        }

        #endregion

        // CheckTransitions
        public override string? CheckTransitions()
        {
            if (!Owner.AnimationPlayer.IsPlaying)
                return ActorStateNames.Stand;
            else
                return base.CheckTransitions();
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            actionDone = false;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (!actionDone && Owner.AnimationPlayer.Frame?.IsEvent == true)
            {
                actionDone = true;
                OnExecuteAction();
            }
        }
    }
}